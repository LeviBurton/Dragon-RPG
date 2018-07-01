// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Circle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_InnerRadius("Inner Radius", Float) = 0.1
		_OuterRadius("Outer Radius", Float) = 1.0
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
			
			struct v2f {
				float4 uv : TEXCOORD0;
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
				o.uv = mul (unity_Projector, vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = fixed4 (0,0,0,0);
				float4 projUV = UNITY_PROJ_COORD(i.uv);
                float dis = sqrt(pow((0.5 - projUV.x), 2) + pow((0.5 - projUV.y), 2));

				if (dis > _InnerRadius && dis < _OuterRadius)
				{
					c = _Color;
				}
			
				else
				{
					discard;
				}

                return c;
			
			}
			ENDCG
		}
	}
}