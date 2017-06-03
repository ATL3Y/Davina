// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BrokenColorEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	float _ToGray_Rate;
	float _Overflow_Rate;
//	float _Record_Rate;
	float _Fade_Rate;

	sampler2D _MainTex;
	half4 _MainTex_TexelSize;

	// the information of the forward direction of main camera
	// x : the alpha angle of its projection on xz plane to x axiz ( from 0 to 2 )
	// y : the beta angle between the direction and the horizontal plane ( from -1 to 1)
	float4 _Camera_Forward;

	sampler2D _OverflowTex;
	half4 _OverflowTex_TexelSize;

	sampler2D _RecordTex;
	half4 _RecordTex_TexelSize;

	float _Add_Thred;
	float _Add_Rate;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float2 world_uv : TEXCOORD1;
	};

	v2f vert (appdata v)
	{ 
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

        o.world_uv.x = - _Camera_Forward.x  + v.uv.x;
        o.world_uv.y = _Camera_Forward.y  + v.uv.y;

		return o;
	}


	half4 copy(v2f i) : SV_Target
	{
		return tex2D(_MainTex, i.uv);
	}

	half4 toGray (v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, i.uv);

		half gray = (col.r + col.g + col.b) /  3;

		half4 gray_col = half4(gray,gray,gray,1);


		return lerp( col , gray_col , _ToGray_Rate);
	}

	half4 OverflowColor( half4 ori , half4 sample , half4 rate)
	{
		half4 res;
		res.r = ori.r + ( ori.r - sample.r ) * rate;
		res.g = ori.g + ( ori.g - sample.g ) * rate;
		res.b = ori.b + ( ori.b - sample.b ) * rate;
		return res;
	}

	half4 overflow (v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, i.uv);
		half4 sample_col = tex2D( _OverflowTex, i.world_uv.xy );
		half4 over_col = OverflowColor( col , sample_col , _Overflow_Rate );

		return over_col;
	}

	half4 add_with_thred (v2f i ) : SV_Target
	{
		half4 col = tex2D(_MainTex, i.uv);
		half4 record_col = tex2D(_RecordTex , i.uv);

		half gray = (col.r + col.g + col.b) /  3;

		if ( gray > _Add_Thred )
		{
			record_col.r = saturate( record_col.r + lerp( 0 , 1 , col.r * _Add_Rate)); 
			record_col.g = saturate( record_col.g + lerp( 0 , 1 , col.g * _Add_Rate)); 
			record_col.b = saturate( record_col.b + lerp( 0 , 1 , col.b * _Add_Rate)); 
		}

		return record_col;
	}

	half4 fade (v2f i ) : SV_Target
	{
//		return tex2D(_MainTex, i.uv) * _Fade_Rate;
		half4 col = tex2D(_MainTex, i.uv );
		half4 res = col;
		res += half4( 0.1,0.1,0.1,0);
		return res;
	}

	ENDCG

	SubShader
	{

		// pass 0 : copy
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment copy
			ENDCG
		}

		// pass 1 : to gary scale
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment toGray
			ENDCG
		}

		// pass 2 : overflow the color
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment overflow
			ENDCG
		}

		// pass 3 : add the input color to with record color
		// and return the record color
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment add_with_thred
			ENDCG
		}

		// pass 4 : fade the color
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment fade
			ENDCG
		}
	}
}
