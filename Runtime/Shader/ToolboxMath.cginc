/////////////////////////////////////////////////////////////////////
// A bunch of math functions that can be used to remove common logical if/else statements from shader code.
// Extra special thank-you to theorangeduck.com's blog for posting this stuff!!
// http://theorangeduck.com/page/avoiding-shader-conditionals
/////////////////////////////////////////////////////////////////////


/////////////////////////////////////////
//              EQAULITY OPS

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


////////////////////////////////////////////
//              COLOR BLEND EQUALITY OPS
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


///////////////////////////////////////////


float and(float a, float b) 
{
  return a * b;
}

float or(float a, float b) 
{
  return min(a + b, 1.0);
}

float xor(float a, float b) 
{
  return (a + b) % 2.0;
}

float not(float a) 
{
  return 1.0 - a;
}


//////////////////////////////////////////////////////////////////////////////////////


float noise_rand(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

float noise_rand_timed(float3 myVector)
{
    return frac(sin(_Time[0] * dot(myVector ,float3(12.9898,78.233,45.5432))) * 43758.5453);
}

//////////////////////////////////////////////
// From the forum post
// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/
// Seed doesn't work in this version
//////////////////////////////////////////////
/*
float BigNonAlignmentMul = 43758.5453
float4 NonAlignmentMuls = float4(12.9898, 46.4321, 78.233, 138.8765)

inline float Rand1(float3 seed)
{
    float combinedSeed = dot(seed, NonAlignmentMuls.xyz);
    float remainder = fmod(combinedSeed, PI);
    float rand = frac(sin(remainder)*BigNonAlignmentMul);
 
    combinedSeed = dot(seed, rand.xxx);
    remainder = fmod(combinedSeed, PI);
    rand = frac(sin(remainder)*BigNonAlignmentMul);
 
    newSeed.xyz = rand.xxx;
 
    return rand;
}
*/



//////////////////////////////////////////////
// Hash Scale Random From Shadertoy
// https://www.shadertoy.com/view/4djSRW
/////////////////////////////////////////////

// Hash without Sine
// Creative Commons Attribution-ShareAlike 4.0 International Public License
// Created by David Hoskins.

// https://www.shadertoy.com/view/4djSRW
// Trying to find a Hash function that is the same on ALL systens
// and doesn't rely on trigonometry functions that change accuracy 
// depending on GPU. 
// New one on the left, sine function on the right.
// It appears to be the same speed, but I suppose that depends.

// * Note. It still goes wrong eventually!
// * Try full-screen paused to see details.


#define ITERATIONS 4


// *** Change these to suit your range of random numbers..

// *** Use this for integer stepped ranges, ie Value-Noise/Perlin noise functions.
#define HASHSCALE1 .1031
#define HASHSCALE3 float4(.1031, .1030, .0973)
#define HASHSCALE4 float4(.1031, .1030, .0973, .1099)

// For smaller input rangers like audio tick or 0-1 UVs use these...
//#define HASHSCALE1 443.8975
//#define HASHSCALE3 float3(443.897, 441.423, 437.195)
//#define HASHSCALE4 float3(443.897, 441.423, 437.195, 444.129)


//----------------------------------------------------------------------------------------
//  1 out, 1 in...
float hash11(float p)
{
	float3 p3  = frac(float3(p) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

//----------------------------------------------------------------------------------------
//  1 out, 2 in...
float hash12(float2 p)
{
	float3 p3  = frac(float3(p.xyx) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

//----------------------------------------------------------------------------------------
//  1 out, 3 in...
float hash13(float3 p3)
{
	p3  = frac(p3 * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

//----------------------------------------------------------------------------------------
//  2 out, 1 in...
float2 hash21(float p)
{
	float3 p3 = frac(float3(p) * HASHSCALE3);
	p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.xx+p3.yz)*p3.zy);

}

//----------------------------------------------------------------------------------------
///  2 out, 2 in...
float2 hash22(float2 p)
{
	float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
    p3 += dot(p3, p3.yzx+19.19);
    return frac((p3.xx+p3.yz)*p3.zy);

}

//----------------------------------------------------------------------------------------
///  2 out, 3 in...
float2 hash23(float3 p3)
{
	p3 = frac(p3 * HASHSCALE3);
    p3 += dot(p3, p3.yzx+19.19);
    return frac((p3.xx+p3.yz)*p3.zy);
}

//----------------------------------------------------------------------------------------
//  3 out, 1 in...
float3 hash31(float p)
{
   float3 p3 = frac(float3(p) * HASHSCALE3);
   p3 += dot(p3, p3.yzx+19.19);
   return frac((p3.xxy+p3.yzz)*p3.zyx); 
}


//----------------------------------------------------------------------------------------
///  3 out, 2 in...
float3 hash32(float2 p)
{
	float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
    p3 += dot(p3, p3.yxz+19.19);
    return frac((p3.xxy+p3.yzz)*p3.zyx);
}

//----------------------------------------------------------------------------------------
///  3 out, 3 in...
float3 hash33(float3 p3)
{
	p3 = frac(p3 * HASHSCALE3);
    p3 += dot(p3, p3.yxz+19.19);
    return frac((p3.xxy + p3.yxx)*p3.zyx);

}

//----------------------------------------------------------------------------------------
// 4 out, 1 in...
float4 hash41(float p)
{
	float4 p4 = frac(float4(p) * HASHSCALE4);
    p4 += dot(p4, p4.wzxy+19.19);
    return frac((p4.xxyz+p4.yzzw)*p4.zywx);
    
}

//----------------------------------------------------------------------------------------
// 4 out, 2 in...
float4 hash42(float2 p)
{
	float4 p4 = frac(float4(p.xyxy) * HASHSCALE4);
    p4 += dot(p4, p4.wzxy+19.19);
    return frac((p4.xxyz+p4.yzzw)*p4.zywx);

}

//----------------------------------------------------------------------------------------
// 4 out, 3 in...
float4 hash43(float3 p)
{
	float4 p4 = frac(float4(p.xyzx)  * HASHSCALE4);
    p4 += dot(p4, p4.wzxy+19.19);
    return frac((p4.xxyz+p4.yzzw)*p4.zywx);
}

//----------------------------------------------------------------------------------------
// 4 out, 4 in...
float4 hash44(float4 p4)
{
	p4 = frac(p4  * HASHSCALE4);
    p4 += dot(p4, p4.wzxy+19.19);
    return frac((p4.xxyz+p4.yzzw)*p4.zywx);
}




