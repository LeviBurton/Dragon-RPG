Shader "Custom/Dissolve Shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_TextureScale ("Texture Scale",float) = 1
		_TriplanarBlendSharpness ("Blend Sharpness",float) = 1
		_DissolvePercentage("DissolvePercentage", Range(0,1)) = 0.0
		_ShowTexture("ShowTexture", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 worldPos : TEXCOORD0;
			float3 worldNormal;
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _DissolvePercentage;
		half _ShowTexture;
		float _TextureScale;
		float _TriplanarBlendSharpness;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
		/*
			// Albedo comes from a texture tinted by color
			half gradient = tex2D (_MainTex, IN.worldPos.rg).r;
			clip(gradient - _DissolvePercentage);

			fixed4 c = lerp(1, gradient, _ShowTexture) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			*/

			// Find our UVs for each axis based on world position of the fragment.
			half2 yUV = IN.worldPos.xz / _TextureScale;
			half2 xUV = IN.worldPos.zy / _TextureScale;
			half2 zUV = IN.worldPos.xy / _TextureScale;

			// Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
			half4 yDiff = tex2D (_MainTex, yUV.rg).r;
			half4 xDiff = tex2D (_MainTex, xUV.rg).r;
			half4 zDiff = tex2D (_MainTex, zUV.rg).r;

			half3 gy = tex2D(_MainTex, yUV.rg).r;
			//clip(yDiff - _DissolvePercentage);
			fixed4 cy = lerp(1, yDiff, _ShowTexture) * _Color;

			half3 gx = tex2D(_MainTex, xUV.rg).r;
			//clip(xDiff - _DissolvePercentage);
			fixed4 cx = lerp(1, xDiff, _ShowTexture) * _Color;

			half3 gz = tex2D(_MainTex, zUV.rg).r;
			//clip(zDiff - _DissolvePercentage);

			fixed4 cz = lerp(1, zDiff, _ShowTexture) * _Color;

			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);

			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

			half3 c1 = gx + gy + gz;
			//clip(c1 - _DissolvePercentage);

			// Finally, blend together all three samples based on the blend mask.
			half3 c = gx * blendWeights.x + gy * blendWeights.y + gz * blendWeights.z;

			clip(c - _DissolvePercentage);

			o.Albedo = c;


		}
		ENDCG
	}
	FallBack "Diffuse"
}
