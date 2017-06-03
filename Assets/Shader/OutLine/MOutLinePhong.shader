// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Outlined/MOutLinePhong"
{
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_SpecColor ("Specular Color", Color) = (.5,.5,.5,1)
		_Shininess ("Shininess" , float ) = 1
		_DisappearRange ("Disappear Range" , float ) = 0.1
     	_InColor ("Inner side Color" , Color ) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_InTex("In Tex" , 2d ) = "white" {}
	}
 
	CGINCLUDE
	#include "UnityCG.cginc"
	 
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	 
	struct v2f_outline {
		float4 pos : POSITION;
		float4 color : COLOR;
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
	 
	uniform float _Outline;
	uniform float4 _OutlineColor;
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
	 
	v2f_outline vert_outline(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f_outline o;
		o.pos = UnityObjectToClipPos(v.vertex);
	 
		float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
	 
		o.pos.xy += offset * o.pos.z * _Outline;
		o.color = _OutlineColor;
		o.color.a = ( _Outline == 0 )? 0 : 1;
		return o;
	}
	 
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
		float alpha = ( depth < _DisappearRange)? depth / _DisappearRange : 1;
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
		float alpha = ( depth < _DisappearRange)? depth / _DisappearRange : 1;
		phongColor.a = alpha;

        return phongColor ;
	}

	fixed4 frag_base_in(v2f_in i ) : COLOR {

		float4 col = tex2D(_InTex, i.uv) * _InColor;

		float depth = length(_WorldSpaceCameraPos - i.posWorld.xyz);
		float alpha = ( depth < _DisappearRange)? depth / _DisappearRange : 1;
		col.a = alpha;
         return col;
	}

	fixed4 frag_add_in(v2f_in i ) : COLOR {

		float4 col = tex2D(_InTex, i.uv) * _InColor;
		float depth = length(_WorldSpaceCameraPos - i.posWorld.xyz);
		float alpha = ( depth < _DisappearRange)? depth / _DisappearRange : 1;
		col.a = alpha;
         return col;
	}

	ENDCG
	 
		SubShader {
//			Tags { "Queue" = "Geometry" , "RenderType" = "Opaque" }
			Tags { "Queue" = "Geometry" }
	 
			// note that a vertex shader is specified here but its using the one above
			Pass {
				Name "OUTLINE"
				Tags { "LightMode" = "Always" }
				Cull Front
				ZWrite On
				ZTest LEqual
//				ColorMask RGB // alpha not used
	 
				// you can choose what kind of blending mode you want for the outline
//				Blend SrcAlpha OneMinusSrcAlpha // Normal
				Blend One One // Additive
//				Blend One OneMinusDstColor // Soft Additive
				//Blend DstColor Zero // Multiplicative
				//Blend DstColor SrcColor // 2x Multiplicative
	 
			CGPROGRAM
			#pragma vertex vert_outline
			#pragma fragment frag
			 
			half4 frag(v2f_outline i) :COLOR {
				return i.color;
			}
			ENDCG
			}

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

//			Pass {
//				Name "BASE_IN"
//				 Tags { "LightMode" = "ForwardBase" } 
//
//				ZWrite On
//				ZTest LEqual
//				Cull front
//
//				CGPROGRAM
//				#pragma vertex vert_in
//				#pragma fragment frag_base_in
//				 
//				ENDCG
//			}
//	 
//			Pass {
//				Name "ADD_IN"
//				 Tags { "LightMode" = "ForwardAdd" } 
//
//				ZWrite On
//				ZTest LEqual
//				Cull front
//				Blend One One // additive blending 
//
//				CGPROGRAM
//				#pragma vertex vert_in
//				#pragma fragment frag_add_in
//				 
//				ENDCG
//			}
		}
 
	Fallback "Diffuse"
}