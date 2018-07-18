Shader "Custom/DefaultShader" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Albedo (RGB)", 2D) = "white" {}
		_DitherPattern("Dither pattern", 2D) = "gray"{}
		_NormalMap("Normal Map", 2D) = "white"{}
		_MainIntensity("Main intensity", Range(0, 2)) = 1
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		_DitherIntensity("Dither intensity", Range(0, 2)) = 1
	}
	SubShader {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#pragma surface surf Custom alpha:fade vertex:vert nofog noinstancing nodynlightmap
		#pragma target 3.0
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _DitherPattern;
		sampler2D _NormalMap;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
			fixed4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _MainIntensity;
		half _DitherIntensity;
		int _UnevenResolution;
		half4 _Flip;


		void vert(inout appdata_full v, out Input o)
		{
			v.vertex.xy *= _Flip.xy;
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {

			if (_UnevenResolution == 1) IN.uv_MainTex.xy += 1.0 / 1024.0;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;

			o.Alpha = c.a;
			o.Metallic = 0;

			half3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			o.Normal = normalize(normal);

			o.Smoothness = _DitherIntensity * tex2D(_DitherPattern, IN.screenPos.xy * _ScreenParams.xy / 8 + _WorldSpaceCameraPos.xy).r;


			float albedoValue = (c.r + c.g + c.b) / 3;

			o.Albedo = o.Albedo * _MainIntensity;
		}

		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			float ditherPattern = s.Smoothness;
			int boundary = 4;

			gi.light.color.rgb *= 1.5;
			gi.light.color.rgb = clamp(gi.light.color.rgb, 0, 2);
			float total = gi.light.color.r + gi.light.color.g + gi.light.color.b;
			total /= 2;

			float clampedLight = floor(total * boundary) / boundary;
			float nextLight = ceil(total * boundary) / boundary;
			float lerp = frac(total * boundary);
			float stepper = step(ditherPattern, lerp);
			gi.light.color *= clampedLight * (1 - stepper) + nextLight * stepper;
			s.Smoothness = 0;
			s.Metallic = 0;
			half4 standard = LightingStandard(s, lightDir, gi);
			return standard;
		}

		inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);
		}


		ENDCG
	}
	FallBack "Diffuse"
}
