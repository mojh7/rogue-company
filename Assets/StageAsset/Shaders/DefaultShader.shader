Shader "Custom/DefaultShader" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Albedo (RGB)", 2D) = "white" {}
		_DitherPattern("Dither pattern", 2D) = "gray"{}
		_MainIntensity("Main intensity", Range(0, 2)) = 1
		_DitherIntensity("Dither intensity", Range(0, 2)) = 1
		_TintMapIntensity("Tint map intensity", Range(0, 2)) = 1
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

		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			float ditherPattern = s.Smoothness;
			int boundary = 2;

			gi.light.color.rgb *= 1.5;
			gi.light.color.rgb = clamp(gi.light.color.rgb,0, 2) * 0;
			float total = gi.light.color.r + gi.light.color.g + gi.light.color.b;
			total /= 3;

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

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
			fixed4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _DitherPattern;
		sampler2D _TintMap;
		half _MainIntensity;
		half _DitherIntensity;
		half _TintMapIntensity;
		int _UnevenResolution;
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {

			if (_UnevenResolution == 1) IN.uv_MainTex.xy += 1.0 / 1024.0;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;

			o.Alpha = c.a;
			o.Metallic = 0;

			o.Smoothness = _DitherIntensity * tex2D(_DitherPattern, IN.screenPos.xy * _ScreenParams.xy / 8 + _WorldSpaceCameraPos.xy).r;


			fixed4 tintmap = tex2D(_TintMap, (IN.worldPos.xy / 256) + .5);
			float albedoValue = (c.r + c.g + c.b) / 3;

			float3 tinted;

			tinted = 2 * o.Albedo * tintmap.rgb;
			o.Albedo = _MainIntensity * saturate(tinted) * _TintMapIntensity + o.Albedo * (1 - _TintMapIntensity);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
