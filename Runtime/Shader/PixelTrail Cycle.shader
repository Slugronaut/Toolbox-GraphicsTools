Shader "Custom/Pixel Trail - Cycle"
{
    Properties
    {
        // Color property for material inspector, default to white
        [HDR]
        _InsideFarColor("Inside Far Color", Color) = (0.2,0.2,1,1)
        [HDR]
        _InsideNearColor("Inside Near Color", Color) = (0.5,1,1,1)
        [HDR]
        _OutsideColor("Outside Color", Color) = (1,1,1,1)
        _MainTex("pixel", 2D) = "white" {}
        _Scale("Scale", float) = 5
        _LengthMax("Length Max", float) = 0.7
        _LengthMin("Length Max", float) = 0.5
        _WidthMax("Width Max", float) = 0.8
        _WidthMin("Width Min", float) = 0.6
        _Speed("Speed", float) = 0.1

    }
    SubShader
    {
        Tags
        {
        "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _OutsideColor;
            fixed4 _InsideFarColor;
            fixed4 _InsideNearColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            
            #define ITERATIONS 4
            // *** Use this for integer stepped ranges, ie Value-Noise/Perlin noise functions.
            #define HASHSCALE1 .1031
            #define HASHSCALE3 vec3(.1031, .1030, .0973)


            half _Scale;
            fixed _LengthMax;
            fixed _LengthMin;
            fixed _WidthMax;
            fixed _WidthMin;
            half _Speed;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };



            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float4(v.uv.xy, 0, 0);
                o.color = float4(0, 0, v.uv.y, v.color.a);
                return o;
            }

            //----------------------------------------------------------------------------------------
            //  1 out, 2 in...
            float hash12(float2 p)
            {
	            float3 p3  = frac(float3(p.xyx) * HASHSCALE1);
                p3 += dot(p3, p3.yzx + 19.19) * _SinTime[0];
                return frac((p3.x + p3.y) * p3.z);
            }
            

            // pixel shader
            fixed4 frag(v2f i) : SV_Target
            {
                float distY = abs(i.uv.y - _LengthMin) * 2;
                fixed4 c;
                if (distY > _LengthMax)
                {
                    c = _OutsideColor;
                    if (hash12(i.vertex) > 1 - (i.uv.x * i.uv.x * i.uv.x))
                        c = float4(0, 0, 0, 0);
                }
                else
                {
                    if (i.uv.x > _WidthMax)
                        c = _InsideFarColor;
                    else if (_WidthMax >= i.uv.x && i.uv.x > _WidthMin)
                    {
                        c = _InsideFarColor*((i.uv.x - _WidthMin) * _Scale) + _InsideNearColor*(1 - (i.uv.x - _WidthMin) * _Scale);
                    }
                    else
                    {
                        c = _InsideNearColor;
                    }
                    if (distY <= _LengthMax && distY > _LengthMin)
                    {
                        c = _OutsideColor*((distY - _LengthMin) * _Scale) + c*(1 - (distY - _LengthMin) * _Scale);
                    }
                    if (hash12(i.vertex) > 1 - (i.uv.x * i.uv.x))
                        c = float4(0, 0, 0, 0);
                }
                c.a *= i.color.a;
                return c;
            }

            ENDCG
        }
    }
}