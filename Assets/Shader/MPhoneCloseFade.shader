Shader "Custom/MPhoneCloseFade"
{
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_SpecColor ("Specular Color", Color) = (.5,.5,.5,1)
		_Shininess ("Shininess" , float ) = 1
		_DisappearRange ("Disappear Range(Max,Min)" , float ) = 0.1
     	_InColor ("Inner side Color" , Color ) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_InTex("In Tex" , 2d ) = "white" {}
	}
 
	CGINCLUDE
	#include "UnityCG.cginc"
	 
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	 
	struct v2f_out {
		float4 pos : POSITION;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
		float3 normal : TEXCOORD1;		// the normal
		float4 posWorld : TEXCOORD2;	// the position of the vertex in world
		UNITY_FOG_COORDS(3)
	};

	struct v2f_in {
		float4 pos : POSITION;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
		float4 posWorld : TEXCOORD1;	// the position of the vertex in world
	};
	 
	uniform float4 _Color;
	uniform float4 _SpecColor;
	uniform float _Shininess;
    uniform float4 _InColor;
    uniform float _DisappearRange;

	float4 _LightColor0;
	float4 _LightPosition;

	sampler2D _CameraDepthTexture;
	sampler2D _CameraDepthNormalsTexture;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _InTex;
	float4 _InTex_ST;
	 
	v2f_out vert_out(appdata_base v ) {
		v2f_out o;
		o.pos = UnityObjectToClipPos(v.vertex); 
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

        o.normal = normalize(
               mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
		o.posWorld = mul(unity_ObjectToWorld,v.vertex);

		UNITY_TRANSFER_FOG(o,o.vertex);

		return o;
	}

	v2f_in vert_in(appdata_base v ) {
		v2f_in o;
		o.pos = UnityObjectToClipPos(v.vertex); 
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.posWorld = mul(unity_ObjectToWorld,v.vertex);
		return o;
	}

	float depth2Alpha( float d )
	{
		float a = 1;
		if ( d < _DisappearRange.x )
			a = d / _DisappearRange.x;
//		{
//			if ( d > _DisappearRange.y )
//			{
//				a = ( d - _DisappearRange.y ) / ( _DisappearRange.x - _DisappearRange.y);
//			}else
//			{
//				a = 0;
//			}
//		}
		return a;
	}

	

	fixed4 frag_base(v2f_out i ) : COLOR {

		float4 col = tex2D(_MainTex, i.uv) * _Color;
        float3 normalDirection = normalize(i.normal);

        float3 viewDirection = normalize( _WorldSpaceCameraPos - i.posWorld.xyz);
        float3 lightDirection;
        float attenuation;

        if (0.0 == _WorldSpaceLightPos0.w) // directional light?
        {
           attenuation = 1.0; // no attenuation
           lightDirection = normalize(_WorldSpaceLightPos0.xyz);
        } 
        else // point or spot light
        {
           float3 vertexToLightSource = 
              _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
           float distance = length(vertexToLightSource);
           attenuation = 1.0 / distance; // linear attenuation 
           lightDirection = normalize(vertexToLightSource);
        }

        float3 ambientLighting = 
           UNITY_LIGHTMODEL_AMBIENT.rgb * col.rgb;

        float3 diffuseReflection = 
           attenuation * _LightColor0.rgb * col.rgb
           * max(0.0, dot(normalDirection, lightDirection));

        float3 specularReflection;
        if (dot(normalDirection, lightDirection) < 0.0) 
           // light source on the wrong side?
        {
           specularReflection = float3(0.0, 0.0, 0.0); 
              // no specular reflection
        }
        else // light source on the right side
        {
           specularReflection = attenuation * _LightColor0.rgb 
              * _SpecColor.rgb * pow(max(0.0, dot(
              reflect(-lightDirection, normalDirection), 
              viewDirection)), _Shininess);
        }

        float4 phongColor = float4(ambientLighting + diffuseReflection 
           + specularReflection, 1.0);

		UNITY_APPLY_FOG(i.fogCoord, phongColor);

		float depth = length(_WorldSpaceCameraPos - i.posWorld.xyz);
		float alpha = depth2Alpha(depth);
		phongColor.a = alpha;


         return phongColor;
	}



	// no ambition light
	fixed4 frag_add(v2f_out i ) : COLOR {

		float4 col = tex2D(_MainTex, i.uv) * _Color;

        float3 normalDirection = normalize(i.normal);

        float3 viewDirection = normalize(
           _WorldSpaceCameraPos - i.posWorld.xyz);
        float3 lightDirection;
        float attenuation;

        if (0.0 == _WorldSpaceLightPos0.w) // directional light?
        {
           attenuation = 1.0; // no attenuation
           lightDirection = normalize(_WorldSpaceLightPos0.xyz);
        } 
        else // point or spot light
        {
           float3 vertexToLightSource = 
              _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
           float distance = length(vertexToLightSource);
           attenuation = 1.0 / distance; // linear attenuation 
           lightDirection = normalize(vertexToLightSource);
        }

        float3 diffuseReflection = 
           attenuation * _LightColor0.rgb * _Color.rgb
           * max(0.0, dot(normalDirection, lightDirection));

        float3 specularReflection;
        if (dot(normalDirection, lightDirection) < 0.0) 
           // light source on the wrong side?
        {
           specularReflection = float3(0.0, 0.0, 0.0); 
              // no specular reflection
        }
        else // light source on the right side
        {
           specularReflection = attenuation * _LightColor0.rgb 
              * _SpecColor.rgb * pow(max(0.0, dot(
              reflect(-lightDirection, normalDirection), 
              viewDirection)), _Shininess);
        }

        half4 phongColor = float4(diffuseReflection 
               + specularReflection, 1.0);

       	UNITY_APPLY_FOG(i.fogCoord, phongColor);

       	float depth = length(_WorldSpaceCameraPos - i.posWorld.xyz);
		float alpha = depth2Alpha(depth);
		phongColor.a = alpha;

        return phongColor ;
	}

	ENDCG
	 
		SubShader {
//			Tags { "Queue" = "Geometry" , "RenderType" = "Opaque" }
			Tags { "Queue" = "Geometry" }

			Pass {
				Name "BASE_OUT"
				 Tags { "LightMode" = "ForwardBase" } 

				ZWrite On
				ZTest LEqual
				Cull BACK
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert_out
				#pragma fragment frag_base
				 
				ENDCG
			}
	 
			Pass {
				Name "ADD_OUT"
				 Tags { "LightMode" = "ForwardAdd" } 

				ZWrite On
				ZTest LEqual
				Cull back
				Blend One One // additive blending 

				CGPROGRAM
				#pragma vertex vert_out
				#pragma fragment frag_add
				 
				ENDCG
			}

		}
 
	Fallback "Diffuse"
}