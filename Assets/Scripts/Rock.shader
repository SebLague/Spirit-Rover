Shader "Custom/Rock" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_GroundTex ("Ground (RGB)", 2D) = "white" {}
		_GroundHeight("Ground height",Float) = 1
		_FadeDst("Fade dst",Float) = 1
		_GroundTexScale("Ground tex Scale",Float) = 1
		_TexScale("Text Scale",Float) = 1
		_Tint ("Tint Col", Color) = (1,1,1,1)
		_GroundTint ("Ground tint Col", Color) = (1,1,1,1)
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
		sampler2D _GroundTex;
		half _FadeDst;
		half _GroundHeight;
		half _GroundTexScale;
		half _TexScale;
		half _GroundPlane;
		half _Glossiness;
		half _Metallic;
		fixed4 _Tint;
		fixed4 _GroundTint;

		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, sampler2D tex) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = tex2D (tex, float2(scaledWorldPos.y, scaledWorldPos.z)) * blendAxes.x;
			float3 yProjection = tex2D (tex, float2(scaledWorldPos.x, scaledWorldPos.z)) * blendAxes.y;
			float3 zProjection = tex2D (tex, float2(scaledWorldPos.x, scaledWorldPos.y)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
			float3 rockTex = triplanar(IN.worldPos,_TexScale,blendAxes,_MainTex);
			float3 groundTex = triplanar(IN.worldPos,_GroundTexScale,blendAxes,_GroundTex);
			half rockStr = saturate((IN.worldPos.y - _GroundHeight)/_FadeDst);

			o.Albedo = rockTex * rockStr * _Tint + groundTex * (1-rockStr)*_GroundTint;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
