#define SPRITE_SAMPLE_PARAMS _MainTex, IN.uv_MainTex, _AlphaTex
#define SPRITE_DATA_SET fixed4 _Color; sampler2D _MainTex; float _AttenScale;
//sampler2D _AlphaTex;
#define SPRITE_INPUT_BASE struct Input { float2 uv_MainTex; fixed4 color;};
#define SPRITE_INPUT_DITHER_NOSEM struct Input { float2 uv_MainTex : TEXCOORD0; fixed4 color : COLOR; float4 screenPos;};
#define SPRITE_INPUT_DITHER struct Input { float2 uv_MainTex : TEXCOORD0; fixed4 color : COLOR; float4 screenPos;};
#define SPRITE_INPUT_HEIGHTDITHER struct Input { float2 uv_MainTex : TEXCOORD0; fixed4 color : COLOR; float4 screenPos; float3 worldPos;};



inline fixed4 SampleSprite(sampler2D tex, float2 uv)//, sampler2D alphaTex)
{
	fixed4 c = tex2D(tex, uv);
	//#if ETC1_EXTERNAL_ALPHA
	//c.a = tex2D(alphaTex, uv).r;
	//#endif
	return c;
}

inline float4 ScreenDoorClip(float4 screenPos)
{
	// Screen-door transparency: Discard pixel if below threshold.
	float4x4 thresholdMatrix =
	{    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
		13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
		 4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
		16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
	};
	float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
	float2 pos = screenPos.xy / screenPos.w;
	pos *= _ScreenParams.xy; // pixel position
	return (thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
}