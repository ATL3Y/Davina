// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Hair"
{
	Properties
	{
		_ColorTex("Color Tex (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "DisableBatching" = "True" }
		LOD 100

		ZWrite On ZTest LEqual Cull Back 

		Pass
		{
			Name "ForwardBase"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"
			#include "Include/HairGrowing.cginc"
			#include "Include/HairLighting.cginc"
			#include "Include/HairTypes.cginc"
		

			#pragma target 3.0
			#pragma multi_compile_fog

			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex VS
			#pragma hull HS
			#pragma domain DS
			#pragma geometry GS
			#pragma fragment FS

			float4 _Size;
			float4 _TessFactor;
			float3 _LightCenter;

			uniform StructuredBuffer<Body> _BodiesBuffer;
			uniform StructuredBuffer<float3> _BarycentricBuffer;

			float _StandWidth;

			float4 _Length;

			float4 _TipColor;
			float4 _RootColor;
			float _ColorBlend;

			float _SpecularShift;
			float _PrimarySpecular;
			float _SecondarySpecular;
			float4 _SpecularColor;

			float _WavinessFrequency;
			float _WavinessScale;
			float _Volume;

			float _Interpolation;

			sampler2D _ColorTex;

			// ***************************************************************
			// Programs
			// ***************************************************************

			VS_OUTPUT VS(uint id:SV_VertexID)
			{
				VS_OUTPUT o;
				o.id = id;
				return o;
			}

			HS_CONSTANT_OUTPUT HSConst()
			{
				HS_CONSTANT_OUTPUT output;

				output.edges[0] = _TessFactor.x; // Detail factor
				output.edges[1] = _TessFactor.y; // Density factor

				return output;
			}

			[domain("isoline")]
			[partitioning("integer")]
			[outputtopology("line")]
			[outputcontrolpoints(3)]
			[patchconstantfunc("HSConst")]
			HS_OUTPUT HS(InputPatch<VS_OUTPUT, 3> ip, uint id : SV_OutputControlPointID)
			{
				HS_OUTPUT output;
				output.id = ip[id].id;
				return output;
			}

			float3 curve(float3 p, float3 uv, float amplitude, float frequency)
			{
				float a = pow(uv.x*frequency, 2) + uv.z;

				float u = pow(amplitude*uv.x,2);
				p.x += cos(a)*u;
				p.z += sin(a)*u;

				return p;
			}

			float3 GetBarycentric(float3 a, float3 b, float3 c, float2 uv)
			{
				float3 k = _BarycentricBuffer[uv.y*64];
				//k *= 1 + k.y*_Volume*uv.x;
				return a*k.x + b*k.y + c*k.z;
			}

			float GetBarycentricFloat(float a, float b, float c, float d)
			{
				float3 k = _BarycentricBuffer[d * 64];
				return a*k.x + b*k.y + c*k.z;
			}

			float3 GetPosition(OutputPatch<HS_OUTPUT, 3> op, float2 uv)
			{
				float length = GetBarycentricFloat(_Length.x, _Length.y, _Length.z, uv.y);

				float3 p1 = GetSplinePoint(op[0].id, uv.x*length, _BodiesBuffer, _Size.y);
				float3 p2 = GetSplinePoint(op[1].id, uv.x*length, _BodiesBuffer, _Size.y);
				float3 p3 = GetSplinePoint(op[2].id, uv.x*length, _BodiesBuffer, _Size.y);

				float3 position = GetBarycentric(p1, p2, p3, uv);
				position = lerp(position, p1, saturate(uv.x - _Interpolation));
				position = curve(position, float3(uv.x, uv.y, op[0].id), _WavinessScale, _WavinessFrequency);
				return position;
			}

			[domain("isoline")]
			DS_OUTPUT DS(HS_CONSTANT_OUTPUT input, OutputPatch<HS_OUTPUT, 3> op, float2 uv : SV_DomainLocation)
			{
				DS_OUTPUT output;

				float uvStepX = 1.0 / _TessFactor.x;

				float3 position1 = GetPosition(op, uv);
				float3 position2 = GetPosition(op, float2(uv.x - uvStepX, uv.y));

				output.position = float4(position1, 1);
				output.tangent = normalize(position1 - position2);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				float3 shadingNormal = normalize(position1 - _LightCenter);
				float shading = Diffuse(shadingNormal, lightDir, 0.5f);

				float shift = tex2Dlod(_ColorTex, float4(uv.y, uv.x, 0, 0)) - 0.5;

				output.factor = float4(uv.x, shift, shading, 0);

				float3 lightRight = cross(lightDir, output.tangent);
				output.normal = cross(output.tangent, lightRight);
				output.viewDir = normalize(_WorldSpaceCameraPos.xyz - output.position);
				output.lightDir = lightDir;

				return output;
			}

			GS_OUTPUT CopyToFragment(DS_OUTPUT i, float4 position)
			{
				GS_OUTPUT output;				

				output.pos = position;
				output.tangent = i.tangent;
				output.normal = i.normal;
				output.viewDir = i.viewDir;
				output.lightDir = i.lightDir;
				output.factor = i.factor;

				return output;
			}

			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(4)]
			void GS(line DS_OUTPUT p[2], inout TriangleStream<GS_OUTPUT> triStream)
			{
				float3 look = normalize((_WorldSpaceCameraPos - p[0].position).xyz);
				float3 right1 = cross(p[0].tangent, look);
				float3 right2 = cross(p[1].tangent, look);

				float3 thicknessK1 = 1 - pow(2, -10 * (1 - p[0].factor.x));
				float3 thicknessK2 = 1 - pow(2, -10 * (1 - p[1].factor.x));

				float3 rightDirection1 = right1*_StandWidth*thicknessK1;
				float3 rightDirection2 = right2*_StandWidth*thicknessK2;

				float4 v[4];
				v[0] = float4(p[0].position + rightDirection1, 1.0f);
				v[1] = float4(p[1].position + rightDirection2, 1.0f);
				v[2] = float4(p[0].position - rightDirection1, 1.0f);
				v[3] = float4(p[1].position - rightDirection2, 1.0f);;

				float4x4 vp = mul(UNITY_MATRIX_MVP, unity_WorldToObject);

				triStream.Append(CopyToFragment(p[0], mul(vp, v[0])));
				triStream.Append(CopyToFragment(p[1], mul(vp, v[1])));
				triStream.Append(CopyToFragment(p[0], mul(vp, v[2])));
				triStream.Append(CopyToFragment(p[1], mul(vp, v[3])));

				CopyToFragment(p[0], v[0]);
				CopyToFragment(p[1], v[1]);
				CopyToFragment(p[0], v[2]);
				CopyToFragment(p[1], v[3]);
			}

			fixed4 FS (GS_OUTPUT i) : SV_Target
			{
				float4 diffuseColor = lerp(_RootColor, _TipColor, saturate(i.factor.x + _ColorBlend));

				float3 diffuse = Diffuse(i.normal, i.lightDir, 0.25);
				diffuse *= diffuseColor;

				float3 tangent1 = ShiftTangent(i.tangent, i.normal, i.factor.y - _SpecularShift);
				float3 tangent2 = ShiftTangent(i.tangent, i.normal, i.factor.y + _SpecularShift);

				float3 specular1 = Specular(tangent1, i.viewDir, i.lightDir, _PrimarySpecular);
				float3 specular2 = Specular(tangent2, i.viewDir, i.lightDir, _SecondarySpecular);
				float3 specular = specular1*specular2*_SpecularColor;

				float3 color = (diffuse*diffuseColor + specular + UNITY_LIGHTMODEL_AMBIENT)*_LightColor0*i.factor.z;
				return float4(color, 1);
			}
			ENDCG
		}
	}

	Fallback "VertexLit"
}
