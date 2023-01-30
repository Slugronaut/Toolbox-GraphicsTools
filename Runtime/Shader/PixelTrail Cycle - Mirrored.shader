Shader "Custom/Pixel Trail - Cycle Mirrored"
{
    Properties
    {
        // Color property for material inspector, default to white
        [HDR]
        [HDR]
        _InsideColor("Inside Color", Color) = (0.5,1,1,1)
        [HDR]
        _OutsideColor("Outside Color", Color) = (1,1,1,1)
        _MainTex("pixel", 2D) = "white" {}
        _Width("Width", float) = .5
        _Start("Start", float) = 3
        _FxWidth("Fx Width", Range(0,1)) = .5

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
            fixed4 _InsideColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            
            #define ITERATIONS 4
            // *** Use this for integer stepped ranges, ie Value-Noise/Perlin noise functions.
            #define HASHSCALE1 .1031
            #define HASHSCALE3 float4(.1031, .1030, .0973)
            #define HASHSCALE4 float4(.1031, .1030, .0973, .1099)

            fixed _Width;
            half _Start;
            fixed _FxWidth;

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
            float hash12_Time(float2 p)
            {
	            float3 p3  = frac(float3(p.xyx + unity_DeltaTime) * HASHSCALE1);
                p3 += dot(p3, p3.yzx + 19.19);
                return frac((p3.x + p3.y) * p3.z);
            }

            //----------------------------------------------------------------------------------------
            //  1 out, 1 in...
            float hash11_Time(float p)
            {
                p += unity_DeltaTime;
	            float3 p3  = frac(float3(p, p, p) * HASHSCALE1);
                p3 += dot(p3, p3.yzx + 19.19);
                return frac((p3.x + p3.y) * p3.z);
            }

            inline float when_eq(float x, float y) 
            {
              return 1.0 - abs(sign(x - y));
            }

            inline float when_neq(float x, float y) 
            {
              return abs(sign(x - y));
            }

            inline float when_gt(float x, float y) 
            {
                return max(sign(x - y), 0.0);
            }

            inline float when_lt(float x, float y) 
            {
              return max(sign(y - x), 0.0);
            }

            inline float when_ge(float x, float y) 
            {
              return 1.0 - when_lt(x, y);
            }

            inline float when_le(float x, float y) 
            {
              return 1.0 - when_gt(x, y);
            }


            inline float4 when_eq(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_eq(x, y));
            }

            inline float4 when_neq(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_neq(x, y));
            }

            inline float4 when_gt(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_gt(x, y));
            }
            
            inline float4 when_lt(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_lt(x, y));
            }

            inline float4 when_ge(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_ge(x, y));
            }

            inline float4 when_le(float x, float y, float4 a, float4 b)
            {
                return lerp(b, a, when_le(x, y));
            }



            // pixel shader
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c;
                float distY = abs(i.uv.y - 0.5) * 2 * _Width;
                c = lerp(_InsideColor, _OutsideColor, distY);
                half comp = pow(i.uv.y, _Start);

                //Simple vertical-only
                //c = when_le(hash12_Time(i.vertex), 1-comp, float4(0,0,0,0), c);

                //both directions
                float outside = when_le(i.uv.y, _FxWidth);
                c = when_eq(outside, 1, when_le(hash12_Time(i.vertex), 1-comp, float4(0,0,0,0), c), c);
                float inside = when_ge(i.uv.y, 1-_FxWidth);
                c = when_eq(inside, 1, when_ge(hash12_Time(i.vertex), 1-comp, float4(0,0,0,0), c), c);

                return c;
            }

            ENDCG
        }
    }
}