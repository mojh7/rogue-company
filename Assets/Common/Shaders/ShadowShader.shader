// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/ShadowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _MainTex("Main texture", 2D) = "white" {}
		_Plane("Plane", Vector) = (0,-1,-1,0)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off

		Pass
		{
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed4 _Color;
			half4 _Plane;

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			VertexOutput vert(appdata_full v) 
			{
				float4 lightDirection;
				VertexOutput o;
				o.uv = v.texcoord;
				lightDirection = mul(unity_WorldToObject, _WorldSpaceLightPos0);
				if (0.0 != _WorldSpaceLightPos0.w)
				{
					// point or spot light
					lightDirection = normalize(v.vertex - lightDirection);
				}
				else
				{
					// directional light
					lightDirection = -normalize(lightDirection);
				}
				v.vertex += lightDirection;
				o.pos = UnityObjectToClipPos(v.vertex);

#ifdef PIXELSNAP_ON
				o.pos = UnityPixelSnap(o.pos);
#endif
				return o;
			}

			fixed4 frag(VertexOutput IN) : SV_Target{
				fixed4 c = tex2D(_MainTex, IN.uv);
				c.rgb = saturate(c.rgb) *IN.color;
				return c;
			}
			ENDCG
		}
	}
}
