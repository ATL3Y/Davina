Shader "Custom/ToColorEffect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
//	_Color ("To Color", 2D) = "grayscaleRamp" {}
//	_Rate ("Rate " , float ) = 0.5
}



	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;


	sampler2D _CameraDepthNormalsTexture;
	sampler2D_float _CameraDepthTexture;


	uniform half4 _Sensitivity; 
	uniform half _SampleDistance;
	uniform fixed4 _EdgeColor;
	uniform fixed4 _Color;
	uniform half _Rate;

	inline half CheckSame (half2 centerNormal, float centerDepth, half4 theSample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		half2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		int isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG (theSample.zw);
		float zdiff = abs(centerDepth-sampleDepth);
		// scale the required threshold by the distance
		int isSameDepth = (zdiff * _Sensitivity.x < 0.09 * centerDepth ) ;
	
		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise
		
		return isSameNormal * isSameDepth ? 1.0 : 0.0;
	}

	v2f vertThin( appdata_img v )
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
		
		o.uv[1] = uv;
		o.uv[4] = uv;
				
		// offsets for two additional samples
		o.uv[1] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		o.uv[2] = uv + float2(-_MainTex_TexelSize.x, +_MainTex_TexelSize.y) * _SampleDistance;
		o.uv[3] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		o.uv[4] = uv + float2(+_MainTex_TexelSize.x, +_MainTex_TexelSize.y) * _SampleDistance;
//		o.uv[2] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
//		o.uv[3] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		
		return o;
	}


	half4 fragThin (v2f i) : SV_Target
	{
		fixed4 original = tex2D(_MainTex, i.uv[0]);
		
		half4 center = tex2D (_CameraDepthNormalsTexture, i.uv[0]);
		half4 sample1 = tex2D (_CameraDepthNormalsTexture, i.uv[1]);
		half4 sample2 = tex2D (_CameraDepthNormalsTexture, i.uv[2]);
		half4 sample3 = tex2D (_CameraDepthNormalsTexture, i.uv[3]);
		half4 sample4 = tex2D (_CameraDepthNormalsTexture, i.uv[4]);
		
		// encoded normal
		half2 centerNormal = center.xy;
		// decoded depth
		float centerDepth = DecodeFloatRG (center.zw);
		
		half edge = 1.0;
		
		edge *= CheckSame(centerNormal, centerDepth, sample1);
		edge *= CheckSame(centerNormal, centerDepth, sample2);
		edge *= CheckSame(centerNormal, centerDepth, sample3);
		edge *= CheckSame(centerNormal, centerDepth, sample4);


		fixed4 edgeColor = edge * _EdgeColor;

		fixed4 effectColor = (edge < 1 ) ?  lerp ( edgeColor , _Color , _Rate) : _Color;

		return lerp( original , effectColor , _Rate);

//		return edge * lerp(original, _BgColor, _BgFade);
	}
	
	ENDCG 


	Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vertThin
      #pragma fragment fragThin
      ENDCG
  }
//SubShader {
//	Pass {
//		ZTest Always Cull Off ZWrite Off
//
//CGPROGRAM
//#pragma vertex vert_img
//#pragma fragment frag
//#include "UnityCG.cginc"
//
//uniform sampler2D _MainTex;
//uniform fixed4 _Color;
//uniform float _Rate;
//
//
//fixed GetGray( fixed4 col )
//{
//	return (col.r + col.g + col.b)/3;
//}
//
//fixed4 frag (v2f_img i) : SV_Target
//{
//	fixed4 orig = tex2D(_MainTex, i.uv);
//
//	fixed4 color = orig;
//
//	fixed4 v_Color = GetGray(orig) / GetGray(_Color) * _Color;
//
//	color.r = lerp(orig.r,v_Color.r,_Rate);
//	color.g = lerp(orig.g,v_Color.g,_Rate);
//	color.b = lerp(orig.b,v_Color.b,_Rate);
//
//	color.rgb += _Color * _Rate;
//
//
//	return color;
//}
//
//ENDCG
//
//	}
}

Fallback off

}
