Shader "System/ShiftingImage"
{
	Properties
	{
		_MainTex ("Texture front", 2D) = "white" {}
		_Color ("Color" , color ) = (1,1,1,1)
		_BackTex ("Texture back", 2D) = "white" {}
		_BackColor ("back Color" , color ) = (1,1,1,1)
		_Speed ("move speed(x,y)", vector) = (0,0,0,0)
		_Scale ("Scale",vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest off
			Cull off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Speed;
			float4 _Scale;
			float4 _Color;
			sampler2D _BackTex;
			float4 _BackTex_ST;
			float4 _BackColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX( v.uv  * _Scale.xy + _Time.y * _Speed.xy  , _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col_front = tex2D(_MainTex, i.uv ) * _Color ;
				fixed4 col_back = tex2D(_BackTex , i.uv ) * _BackColor;
				fixed4 col;
				if ( _BackColor.a > 0 )
					col.rgb = lerp( col_back , col_front , col_front.a );
				else 
					col.rgb = col_front.rgb;

				col.a = max(col_back.a,col_front.a);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
