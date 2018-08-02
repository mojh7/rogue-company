Shader "Custom/DefaultShader" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_Layer("Layer", 2D) = "white" {}
		[PerRendererData]_MainTex("Albedo (RGB)", 2D) = "white" {}
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		_Boundary("Boundary Number", Int) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_LightIntensity("Light intensity", Range(0, 1)) = 1
		_EffectAmount("Effect Amount", Range(0, 1)) = 0
		_TintMapIntensity("Tint map intensity", Range(0, 2)) = 1
		_Contrast("Contrast Amount", Range(0, 3)) = 1.0 // 희지 권장 1.4
	}
	SubShader {
		Tags{ 
			"RenderType" = "Transparent" 
			"Queue" = "Transparent"
			"CanUseSpriteAtlas" = "True"
		}
		LOD 200
		Cull Off
		ZWrite Off
		Fog{ Mode Off }

		CGPROGRAM
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA PIXELSNAP_ON
		#pragma surface surf Custom alpha:fade vertex:vert nofog noinstancing nodynlightmap
		#pragma target 3.0
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _Layer;

		struct Input {
			float3 worldPos;
			float4 screenPos;
			float2 uv_MainTex;
			fixed4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _MainIntensity;
		half _LightIntensity;
		int _UnevenResolution;
		int _Boundary;
		uniform float _EffectAmount;
		uniform float _Contrast;

		half _TintMapIntensity;
	
		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			int bound = _Boundary;

			gi.light.color.rgb *= _LightIntensity;
			gi.light.color.rgb = clamp(gi.light.color.rgb, 0, 1);
			float vall = gi.light.color.r + gi.light.color.g + gi.light.color.b;
			vall /= 3;
			float clampedLight = floor(vall * bound) / bound;
			float nextLight = ceil(vall * bound) / bound;

			s.Smoothness = 0;
			s.Metallic = 0;
			gi.light.color *= clampedLight * 0.5f + nextLight * 0.5f;
			half4 standard = LightingStandard(s, lightDir, gi);
			return standard;
		}

		inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);
		}

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;

			c.rgb = ((c.rgb - 0.5f) * _Contrast) + 0.5f;
			c.rgb = lerp(c.rgb, dot(c.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount);

			o.Albedo = c.rgb;
			o.Alpha = c.a;

			fixed4 tintmap = tex2D(_Layer, (IN.worldPos.xy / 256) + .5);
			float albedoValue = (c.r + c.g + c.b) / 3;

			float3 tinted;

			if (albedoValue < .5) tinted = 2 * o.Albedo * tintmap.rgb;
			else tinted = 1 - 2 * (1 - o.Albedo) * (1 - tintmap.rgb);
			o.Albedo = saturate(tinted) * _TintMapIntensity + o.Albedo * (1 - _TintMapIntensity);


			//o.Albedo = saturate(o.Albedo);
		}


		ENDCG
	}
	FallBack "Diffuse"
}
