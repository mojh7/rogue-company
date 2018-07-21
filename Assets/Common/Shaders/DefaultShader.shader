Shader "Custom/DefaultShader" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Albedo (RGB)", 2D) = "white" {}
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		_Boundary("Boundary Number", Int) = 1
		_LightIntensity("Light intensity", Range(0, 1)) = 1
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
		half _LightIntensity;
		int _UnevenResolution;
		int _Boundary;
		half4 _Flip;

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
			v.vertex.xy *= _Flip.xy;
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			if (_UnevenResolution == 1) IN.uv_MainTex.xy += 1.0 / 1024.0;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Albedo = saturate(o.Albedo);
		}


		ENDCG
	}
	FallBack "Diffuse"
}
