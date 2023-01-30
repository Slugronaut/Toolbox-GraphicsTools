Shader "Crimson/Shadowcast Only"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }


		//Offset 1,1
		Cull Off

        CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows addshadow
        #pragma target 3.0            

        sampler2D _MainTex;

        struct Input
        {
			float2 uv_MainTex : TEXCOORD0;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = float4(1, 1, 0, 0);
			o.Alpha = c.a;
			clip(c.a-0.01);
        }
                
        ENDCG
    }

    Fallback "Transparent/VertexLit"
}
