// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient" {
     Properties {
         _ColorTop ("Top Color", Color) = (1,1,1,1)
         _ColorMid ("Mid Color", Color) = (1,1,1,1)
         _ColorBot ("Bot Color", Color) = (1,1,1,1)
         _Middle ("Middle", Range(0.001, 0.999)) = 1
     }
 
     SubShader {
		Tags { "RenderType"="Opaque" }
         LOD 100
 
         ZWrite On
 
         Pass {
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag
         #include "UnityCG.cginc"
 
         fixed4 _ColorTop;
         fixed4 _ColorMid;
         fixed4 _ColorBot;
         float  _Middle;
 
         struct v2f {
             float4 pos : SV_POSITION;
             float4 uv : TEXCOORD0;
         };
 
         v2f vert (appdata_full v) {
             v2f o;
             o.pos = UnityObjectToClipPos (v.vertex);
             o.uv = v.texcoord;
             return o;
         }
 
         fixed4 frag (v2f i) : COLOR {
             fixed4 c = lerp(_ColorBot, _ColorMid, i.uv.y / _Middle) * step(i.uv.y, _Middle);
             c += lerp(_ColorMid, _ColorTop, (i.uv.y - _Middle) / (1 - _Middle)) * step(_Middle, i.uv.y);
             c.a = 1;
             return c;
         }
         ENDCG
         }
     }
 }