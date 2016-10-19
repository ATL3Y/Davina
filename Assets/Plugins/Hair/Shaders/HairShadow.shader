// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/HairShadow"
{
	Properties
	{

	}
		
	SubShader
	{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "DisableBatching" = "True" }
		LOD 100

		ZWrite On ZTest LEqual Cull Back

		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }

			Fog{ Mode Off }
			ZWrite On ZTest Less Cull Off
			Offset 1, 1

			CGPROGRAM
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"
			#include "Include/HairGrowing.cginc"
			#include "Include/HairLighting.cginc"
			#include "Include/HairTypes.cginc"

			#pragma target 5.0
			#pragma multi_compile_fog
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_fwdbase
			#pragma shader_feature _ _ALPHATEST_OFF _ALPHABLEND_OFF _ALPHAPREMULTIPLY_OFF
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

			float _WavinessFrequency;
			float _WavinessScale;

			float _Interpolation;

			struct GS_OUTPUT_SHADOW
			{
				float4 pos   : SV_Position;
			};

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

				float u = pow(amplitude*uv.x, 2);
				p.x += cos(a)*u;
				p.z += sin(a)*u;

				return p;
			}

			float3 GetBarycentric(float3 a, float3 b, float3 c, float d)
			{
				float3 k = _BarycentricBuffer[d * 64];
				return a*k.x + b*k.y + c*k.z;
			}

			float3 GetPosition(OutputPatch<HS_OUTPUT, 3> op, float2 uv)
			{
				float3 p1 = GetSplinePoint(op[0].id, uv.x, _BodiesBuffer, _Size.y);
				float3 p2 = GetSplinePoint(op[1].id, uv.x, _BodiesBuffer, _Size.y);
				float3 p3 = GetSplinePoint(op[2].id, uv.x, _BodiesBuffer, _Size.y);

				float3 position = GetBarycentric(p1, p2, p3, uv.y);
				position = lerp(position, p1, saturate(uv.x - _Interpolation));
				position = curve(position, float3(uv.x, uv.y, op[0].id), _WavinessScale, _WavinessFrequency);
				return position;
			}

			[domain("isoline")]
			DS_OUTPUT_SHADOW DS(HS_CONSTANT_OUTPUT input, OutputPatch<HS_OUTPUT, 3> op, float2 uv : SV_DomainLocation)
			{
				DS_OUTPUT_SHADOW output;

				float uvStepX = 1.0 / _TessFactor.x;

				float3 position1 = GetPosition(op, uv);
				float3 position2 = GetPosition(op, float2(uv.x + uvStepX, uv.y));

				output.position = float4(position1, 1);
				output.tangent = normalize(position2 - position1);

				return output;
			}

			GS_OUTPUT_SHADOW CopyToFragment(DS_OUTPUT_SHADOW i, float4 position)
			{
				GS_OUTPUT_SHADOW output;
				output.pos = position;
				TRANSFER_VERTEX_TO_FRAGMENT(output);
				return output;
			}

			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(4)]
			void GS(line DS_OUTPUT_SHADOW p[2], inout TriangleStream<GS_OUTPUT_SHADOW> triStream)
			{
				float3 look = normalize((_WorldSpaceCameraPos - p[0].position).xyz);
				float3 right1 = cross(p[0].tangent, look);
				float3 right2 = cross(p[1].tangent, look);

				float3 rightDirection1 = right1*_StandWidth;
				float3 rightDirection2 = right2*_StandWidth;

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
			}

			fixed4 FS(GS_OUTPUT_SHADOW i) : COLOR
			{
				//UNITY_INITIALIZE_OUTPUT(GS_OUTPUT_SHADOW, i);
				SHADOW_CASTER_FRAGMENT(fin)

				float attenuation = LIGHT_ATTENUATION(i);
				return fixed4(1.0, 1.0, 1.0, 1.0) * attenuation;
			}

			ENDCG
		}

		Pass
		{
			Name "ShadowCollector"
			Tags{ "LightMode" = "ShadowCollector" }

			Fog{ Mode Off }
			LOD 200

			ZWrite On ZTest LEqual Cull Off

			Offset 1, 1

			CGPROGRAM
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"
			#include "Include/HairGrowing.cginc"
			#include "Include/HairLighting.cginc"
			#include "Include/HairTypes.cginc"

			#define SHADOW_COLLECTOR_PASS
			#pragma target 5.0
			//#pragma multi_compile_fog
			#pragma multi_compile_shadowcollector
			//#pragma multi_compile_fwdbase
			//#pragma shader_feature _ _ALPHATEST_OFF _ALPHABLEND_OFF _ALPHAPREMULTIPLY_OFF
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

			float _WavinessFrequency;
			float _WavinessScale;

			float _Interpolation;

			struct GS_OUTPUT_SHADOW
			{
				float4 pos   : SV_Position;
				V2F_SHADOW_COLLECTOR;
			};

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

			float3 curve(float3 p, float2 uv, float amplitude, float frequency)
			{
				float a = pow((p.y - _LightCenter.y - 1)*frequency, 2);

				float u = pow(amplitude*uv.x, 2);
				p.x += cos(a)*u;
				p.z += sin(a)*u;

				return p;
			}

			float3 GetBarycentric(float3 a, float3 b, float3 c, float d)
			{
				float3 k = _BarycentricBuffer[d * 64];
				return a*k.x + b*k.y + c*k.z;
			}

			float3 GetPosition(OutputPatch<HS_OUTPUT, 3> op, float2 uv)
			{
				float3 p1 = GetSplinePoint(op[0].id, uv.x, _BodiesBuffer, _Size.y);
				float3 p2 = GetSplinePoint(op[1].id, uv.x, _BodiesBuffer, _Size.y);
				float3 p3 = GetSplinePoint(op[2].id, uv.x, _BodiesBuffer, _Size.y);

				float3 position = GetBarycentric(p1, p2, p3, uv.y);
				position = lerp(position, p1, saturate(uv.x - _Interpolation));
				position = curve(position, uv, _WavinessScale, _WavinessFrequency);
				return position;
			}

			[domain("isoline")]
			DS_OUTPUT_SHADOW DS(HS_CONSTANT_OUTPUT input, OutputPatch<HS_OUTPUT, 3> op, float2 uv : SV_DomainLocation)
			{
				DS_OUTPUT_SHADOW output;

				float uvStepX = 1.0 / _TessFactor.x;

				float3 position1 = GetPosition(op, uv);
				float3 position2 = GetPosition(op, float2(uv.x + uvStepX, uv.y));

				output.position = float4(position1, 1);
				output.tangent = normalize(position2 - position1);

				return output;
			}

			GS_OUTPUT_SHADOW CopyToFragment(DS_OUTPUT_SHADOW i, float4 position)
			{
				GS_OUTPUT_SHADOW output;
				output.pos = position;
				//UNITY_INITIALIZE_OUTPUT(GS_OUTPUT_SHADOW, output);
				TRANSFER_SHADOW_COLLECTOR(o)
				return output;
			}

			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(4)]
			void GS(line DS_OUTPUT_SHADOW p[2], inout TriangleStream<GS_OUTPUT_SHADOW> triStream)
			{
				float3 look = normalize((_WorldSpaceCameraPos - p[0].position).xyz);
				float3 right1 = cross(p[0].tangent, look);
				float3 right2 = cross(p[1].tangent, look);

				float3 rightDirection1 = right1*_StandWidth;
				float3 rightDirection2 = right2*_StandWidth;

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
			}

			fixed4 FS(GS_OUTPUT_SHADOW i) : COLOR
			{

				SHADOW_COLLECTOR_FRAGMENT(i)
			}

			ENDCG
		}
	}

	Fallback "VertexLit"
}
