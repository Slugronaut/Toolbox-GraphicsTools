Shader "Custom/Noise/NoiseRandomAdvanced" {
     Properties{
         _Color("Colour (RGBA)", Color) = (0,0,0,1)
         
         _Res("Noise Resolution",Float) = 128
         _MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
 
         _Glossiness("Smoothness", Range(0,1)) = 0.5
         _Metallic("Metallic", Range(0,1)) = 0.0
 
         _Illum("Illumin (A)", 2D) = "white" {}
         _Emission("Emission (Lightmapper)", Float) = 1.0
     }
     SubShader{
             
         Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
         LOD 200
 
         CGPROGRAM
 
         #pragma surface surf Standard fullforwardshadows alpha
         #pragma target 3.0
 
         float4 _Color;
         float _Res;
 
         struct Input {
             float2 uv: TEXCOORD;
             float3 worldPos;
             float2 uv_MainTex;
             float2 uv_Illum;
         };
 
         half _Glossiness;
         half _Metallic;
         sampler2D _Illum;
         fixed _Emission;
 
         float rand(float3 myVector) {
             return frac(sin(_Time[0] * dot(myVector ,float3(12.9898,78.233,45.5432))) * 43758.5453);
         }
     
         sampler2D _MainTex;
 
         void surf(Input IN, inout SurfaceOutputStandard o) {
             float3 vWPos = IN.worldPos;
             vWPos *= _Res;
 
             float Rand1 = rand(round(vWPos));
             vWPos += float3(.5,.5,.5);
 
             Rand1 += rand(round(vWPos));
             vWPos -= float3(1,1,1);
             Rand1 /= 2;
 
             fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * float4(Rand1, Rand1, Rand1, Rand1);
 
             o.Albedo = c;
             o.Metallic = _Metallic - (1- _Color.a);
             o.Smoothness = _Glossiness;
             o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a * _Emission.rrr;
             o.Alpha = _Color.a;
         }
         ENDCG
     }
     FallBack "Diffuse"
 }