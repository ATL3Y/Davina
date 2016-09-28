Shader "Hidden/BrokenEffectShader"
{
	Properties
	{
		_MainTex ("-", 2D) = "" {}
	} 

	CGINCLUDE
	
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;

	sampler2D _ColorOverflowTexture;
	uniform float3 _ColorOverflowTexture_TexelSize;

	float _GrayScale;
	float _ColorOverflowRate;

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	v2f vert(appdata_img v) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	float4 Copy(v2f i) : SV_Target
	{
		float4 col = tex2D(_MainTex,i.uv);
		return col;	
	}

	float4 ToGray(v2f i ) : SV_Target
	{
		float4 col = tex2D(_MainTex,i.uv);
		float gray = (col.r / 3  + col.g / 3  + col.b / 3 );
		float4 toCol;
		toCol.r = toCol.g = toCol.b = gray;
		toCol.a = 1;
		fixed4 res = lerp(col , toCol , _GrayScale);

		return res;
	}

	float overflow( float from , float to , float rate )
	{
		float diff = to - from;
		return from - diff * rate;
	}

	float4 ColorOverflow(v2f i ) : SV_Target
	{
		float4 col = tex2D(_MainTex , i.uv);
		float4 over = tex2D(_ColorOverflowTexture , i.uv);

		float4 result;
		result.r = overflow( col.r , over.r , _ColorOverflowRate );
		result.g = overflow( col.g , over.g , _ColorOverflowRate );
		result.b = overflow( col.b , over.b , _ColorOverflowRate );
		result.a = 1;
		return result;
	}

	ENDCG

	SubShader
	{
		// pass 0
		Pass {
			ZTest Always Cull Off ZWrite On Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment Copy

			ENDCG
		}

		// pass 1
		Pass {
			ZTest Always Cull Off ZWrite On Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment ToGray

			ENDCG
		}

		// pass 2
		Pass {
			ZTest Always Cull Off ZWrite On Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment ColorOverflow

			ENDCG
		}

	}


Fallback off
	

}
