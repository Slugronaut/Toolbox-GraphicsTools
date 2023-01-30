Shader "Crimson/Decal"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_AttenScale("Attenuation", Range(0,10)) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
			"ForceNoShadowCasting" = "True"
			"CanUseSpriteAtlas" = "True"
			"PreviewType" = "Plane"
		}

		Offset -1, -1
		Cull Back

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert keepalpha// decal:blend
		#pragma target 3.0            
		#pragma multi_compile _ PIXELSNAP_ON
		#include "SpriteShader.cginc"

		SPRITE_DATA_SET
		SPRITE_INPUT_BASE

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = SampleSprite(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * c.a * _AttenScale;
			o.Alpha = c.a;
			clip(o.Alpha - 0.01);
		}
		ENDCG
	}

	Fallback "Transparent/Diffuse"
}
