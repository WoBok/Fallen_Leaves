#ifndef FALLEN_LEAVES_INPUT
#define FALLEN_LEAVES_INPUT

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
    float4 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 positionCS : SV_POSITION;
    float4 TtoW0 : TEXCOORD3;
    float4 TtoW1 : TEXCOORD4;
    float4 TtoW2 : TEXCOORD5;
    float fogFactor : TEXCOORD6;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

sampler2D _BaseMap;
sampler2D _NormalMap;

float4 _BaseMap_ST;
half4 _LightDirection;

#if defined(HSISWITCH_ON)
    half _Brightness;
    half _Saturation;
    half _Contrast;
#endif

#if defined(DIFFUSESWITCH_ON)
    half4 _FrontLightColor;
    half4 _BackLightColor;
    half _DiffuseFrontIntensity;
    half _DiffuseBackIntensity;
#endif

#if defined(SPECULARSWITCH_ON)
    half4 _SpecularColor;
    half _SpecularIntensity;
    half _Smoothness;
#endif

#if defined(NORMALSWITCH_ON)
    float4 _NormalMap_ST;
    half _NormalScale;
#endif

#if defined(FRESNELSWITCH_ON)
    half4 _FresnelColor;
    half _FresnelPower;
#endif

half _Alpah;

#if defined(ALPHACLIPPING_ON)
    half _AlphaClipThreshold;
#endif

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    float4x4 _InstanceObjectToWorld;
    float4x4 _InstanceWorldToObject;
#endif

//---------------------------------------------------------------------------------------------------------------//

struct FallenLeavesData {
    float3 position;
    float3 windForce;
    float fallingSpeed;
    float2 rotationSpeed;
    float scale;
};

RWStructuredBuffer<FallenLeavesData> fallenLeavesDataBuffer;
RWStructuredBuffer<float4x4> objectToWorldMatrixBuffer;

float time;

#endif