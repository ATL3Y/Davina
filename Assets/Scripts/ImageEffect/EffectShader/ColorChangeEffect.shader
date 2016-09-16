Shader "Custom/ColorChangeEffect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("To Color", 2D) = "grayscaleRamp" {}
	_Rate ("Rate " , float ) = 0.5
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off

CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform fixed4 _Color;
uniform float _Rate;


fixed GetGray( fixed4 col )
{
	return (col.r + col.g + col.b)/3;
}

fixed4 frag (v2f_img i) : SV_Target
{
	fixed4 orig = tex2D(_MainTex, i.uv);
//	
//	fixed rr = lerp(orig.r,_Color.r,_Rate);
//	fixed gg = tex2D(_RampTex, orig.gg).g;
//	fixed bb = tex2D(_RampTex, orig.bb).b;
//	
//	fixed4 color = fixed4(rr, gg, bb, orig.a);
	fixed4 color = orig;

	fixed4 v_Color = GetGray(orig) / GetGray(_Color) * _Color;
//	fixed4 v_Color = _Color;
//	fixed4 v_Color = GetGray(orig);

	color.r = lerp(orig.r,v_Color.r,_Rate);
	color.g = lerp(orig.g,v_Color.g,_Rate);
	color.b = lerp(orig.b,v_Color.b,_Rate);


	return color;
}

ENDCG

	}
}

Fallback off

}
