Shader "Custom/Rock" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TexScale("Text Scale",Float) = 1
		_Tint ("Tint Col", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		sampler2D _MainTex;
		half _TexScale;
		half _GroundPlane;
		half _Glossiness;
		half _Metallic;
		fixed4 _Tint;

		float3 triplanar(float3 worldPos, float scale, float3 blendAxes) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = tex2D (_MainTex, float2(scaledWorldPos.y, scaledWorldPos.z)) * blendAxes.x;
			float3 yProjection = tex2D (_MainTex, float2(scaledWorldPos.x, scaledWorldPos.z)) * blendAxes.y;
			float3 zProjection = tex2D (_MainTex, float2(scaledWorldPos.x, scaledWorldPos.y)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
			float3 tex = triplanar(IN.worldPos,_TexScale,blendAxes);

			o.Albedo = tex * _Tint;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
