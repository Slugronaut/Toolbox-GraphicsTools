Shader "Custom/Color Lerp Cycle" 
{
	Properties 
    {
		_Color ("Color", Color) = (1,1,1,1)
        [HDR]
		_CycleColor1 ("Cycle Color 1", Color) = (1,1,1,1)
        [HDR]
		_CycleColor2 ("Cycle Color 2", Color) = (1,1,1,1)
        _Speed ("Speed", float) = 1
        _PingPong ("Ping Pong", int) = 1
	}

	SubShader 
    {
		Tags { "RenderType"="Opaque" }
		LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _CycleColor1;
            fixed4 _CycleColor2;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed _Blend = (1+sin(_Time.w*_Speed))/2;
                fixed4 c = lerp(_CycleColor1, _CycleColor2, _Blend);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }

            ENDCG
        }
	}
	FallBack "Diffuse"
}
