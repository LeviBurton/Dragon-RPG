// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Grid" {
	 Properties {
      _GridThickness ("Grid Thickness", Float) = 0.01
	  _GridInnerThickness ("Grid Inner Thickness", Float) = 0.01
      _GridSpacing ("Grid Spacing", Float) = 10.0
	  _GridInnerSpacing("Grid Inner Spacing", Float) = 1.0
      _GridColour ("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
	  _GridInnerColour ("Grid Inner Colour", Color) = (0.5, 1.0, 1.0, 1.0)
      _BaseColour ("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
    }
     
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			  // Access Shaderlab properties
			uniform float _GridThickness;
			uniform float _GridInnerThickness;
			uniform float _GridSpacing;
			uniform float _GridInnerSpacing;
			uniform float4 _GridColour;
			uniform float4 _GridInnerColour;
			uniform float4 _BaseColour;

			struct v2f {
				float4 worldPos : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			fixed4 _Color;
			fixed4 _OutlineColor;
			float _InnerRadius;
			float _OuterRadius;			

			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);

				// this would be projector space, so local to the object its attached to
				//o.uv = mul (unity_Projector, vertex);

				// this is the world space of the vertex.
				// this requies the projector to cover the entire world space.
				o.worldPos = mul(unity_ObjectToWorld, vertex);
			
				return o;
			}

			fixed4 frag (v2f input) : COLOR
			{
				if (frac(input.worldPos.x / _GridSpacing) < _GridThickness || 
					frac(input.worldPos.z / _GridSpacing) < _GridThickness) 
				{
	
					return _GridColour;
				}
				else 
				{
					if (frac(input.worldPos.x / _GridInnerSpacing) <_GridInnerThickness ||
					frac(input.worldPos.z / _GridInnerSpacing) < _GridInnerThickness)
					{
						return _GridInnerColour;
					}
					return _BaseColour;
				}
			}
			ENDCG
		}
	}
}