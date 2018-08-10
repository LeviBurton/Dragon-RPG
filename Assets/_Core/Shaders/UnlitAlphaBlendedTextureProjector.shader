// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Unlit Alpha Blended Texture" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {} // texture must be white for this to work!
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
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			sampler2D _MainTex;
			fixed4 _Color;

			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uv = mul (unity_Projector, vertex);
				o.uvFalloff = mul(unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_MainTex, UNITY_PROJ_COORD(i.uv));
				texS.rgba *= _Color.rgba;
	
				UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(0,0,0,0));
				return texS;
			}
			ENDCG
		}
	}
}