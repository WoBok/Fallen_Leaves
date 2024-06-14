Shader "FallenLeaves/FallenLeaves" {
    Properties {
        _BaseMap ("Albedo", 2D) = "white" { }

        [Space(15)]
        _LightDirection ("Light Direction", vector) = (0.1, 0.2, 0.1, 0)

        [Header(Diffuse)]
        [Toggle]DiffuseSwitch ("Diffuse Switch", int) = 1
        _FrontLightColor ("Front Light Color", Color) = (1, 1, 1, 1)
        _BackLightColor ("Back Light Color", Color) = (1, 1, 1, 1)
        _DiffuseFrontIntensity ("Diffuse Front Intensity", float) = 1
        _DiffuseBackIntensity ("Diffuse Back Intensity", float) = 0.3

        [Header(Specular)]
        [Toggle]SpecularSwitch ("Specular Switch", int) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularIntensity ("Specular Intensity", Range(0, 10)) = 1
        _Smoothness ("Smoothness", Range(0.03, 2)) = 0.5

        [Header(Normal)]
        [Toggle]NormalSwitch ("Normal Switch", int) = 0
        _NormalMap ("Normap Map", 2D) = "white" { }
        _NormalScale ("Normal Scale", float) = 1

        [Header(Fresnel)]
        [Toggle]FresnelSwitch ("Fresnel Switch", int) = 1
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 0)
        _FresnelPower ("Fresnel Power", Range(0, 8)) = 3

        [Header(HSI)]
        [Toggle]HSISwitch ("HSI Switch", int) = 1
        _Brightness ("Brightness", Range(0, 10)) = 1
        _Saturation ("Saturation", Range(0, 10)) = 1
        _Contrast ("Contrast", Range(0, 10)) = 1

        [Header(Alpha)]
        _Alpah ("Alpha", Range(0, 1)) = 1
        [Toggle]AlphaClipping ("Alpah Clipping", int) = 0
        _AlphaClipThreshold ("Threshold", Range(0, 1)) = 0.5

        [Header(Fog)]
        [Toggle]FogSwitch ("Fog Switch", int) = 0

        [Header(Other Settings)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend ("SrcBlend", float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend ("DstBlend", float) = 0
        [Enum(On, 1, Off, 0)]_ZWrite ("ZWrite", float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest ("ZTest", float) = 4
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Cull", float) = 2
    }

    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass {
            Tags { "LightMode" = "UniversalForward" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            ZTest [_ZTest]
            Cull[_Cull]

            HLSLPROGRAM

            #pragma multi_compile_instancing
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile_fog

            #pragma shader_feature HSISWITCH_ON
            #pragma shader_feature DIFFUSESWITCH_ON
            #pragma shader_feature SPECULARSWITCH_ON
            #pragma shader_feature FRESNELSWITCH_ON
            #pragma shader_feature ALPHACLIPPING_ON
            #pragma shader_feature NORMALSWITCH_ON
            #pragma shader_feature FOGSWITCH_ON

            #pragma instancing_options procedural:SetupMatrix

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
                StructuredBuffer<float4x4> objectToWorldMatrixBuffer;
                float4x4 _InstanceObjectToWorld;
                float4x4 _InstanceWorldToObject;
            #endif

            void SetupMatrix() {
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    _InstanceObjectToWorld = objectToWorldMatrixBuffer[unity_InstanceID];
                    _InstanceWorldToObject = Inverse(_InstanceObjectToWorld);
                #endif
            }

            Varyings Vertex(Attributes input) {

                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float4x4 objectToWorld = 0;
                float4x4 worldToObject = 0;

                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    objectToWorld = _InstanceObjectToWorld;
                    worldToObject = _InstanceWorldToObject;
                #else
                    objectToWorld = unity_ObjectToWorld;
                    worldToObject = unity_WorldToObject;
                #endif

                float4 positionWS = mul(objectToWorld, input.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS.xyz);
                
                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON)
                    output.normalWS = mul(input.normalOS, (float3x3)worldToObject);

                    #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON)
                        output.positionWS = mul(objectToWorld, input.positionOS).xyz;
                    #endif

                    #if defined(NORMALSWITCH_ON)
                        half3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
                        half3 worldBinormal = cross(output.normalWS, worldTangent) * input.tangentOS.w;

                        output.TtoW0 = float4(worldTangent.x, worldBinormal.x, output.normalWS.x, output.positionWS.x);
                        output.TtoW1 = float4(worldTangent.y, worldBinormal.y, output.normalWS.y, output.positionWS.y);
                        output.TtoW2 = float4(worldTangent.z, worldBinormal.z, output.normalWS.z, output.positionWS.z);

                        output.uv.zw = input.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
                    #endif

                #endif

                output.uv.xy = input.texcoord.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

                #if defined(FOGSWITCH_ON)
                    output.fogFactor = ComputeFogFactor(output.positionCS.z);
                #endif

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 albedo = tex2D(_BaseMap, input.uv.xy);
                half3 color = albedo.rgb;

                #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON)
                    #if defined(NORMALSWITCH_ON)
                        half3 positionWS = float3(input.TtoW0.w, input.TtoW1.w, input.TtoW2.w);
                    #else
                        half3 positionWS = input.positionWS;
                    #endif
                #endif

                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON)
                    #if defined(NORMALSWITCH_ON)
                        half3 normalWS = UnpackNormal(tex2D(_NormalMap, input.uv.zw));
                        normalWS.xy * _NormalScale;
                        normalWS.z = sqrt(1 - saturate(dot(normalWS.xy, normalWS.xy)));
                        normalWS = normalize(half3(dot(input.TtoW0.xyz, normalWS), dot(input.TtoW1.xyz, normalWS), dot(input.TtoW2.xyz, normalWS)));
                    #else
                        half3 normalWS = normalize(input.normalWS);
                    #endif
                #endif

                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON)
                    half3 lightDirWS = normalize(_LightDirection.xyz);
                #endif

                #if defined(DIFFUSESWITCH_ON)
                    half halfLambert = dot(normalWS, lightDirWS) * 0.5 + 0.5;
                    half3 diffuse = _FrontLightColor.rgb * albedo.rgb * halfLambert * _DiffuseFrontIntensity;
                    half oneMinusHalfLambert = 1 - halfLambert;
                    diffuse += _BackLightColor.rgb * albedo.rgb * oneMinusHalfLambert * _DiffuseBackIntensity;
                    color = diffuse;
                #endif

                #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON)
                    half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - positionWS);
                #endif

                #if defined(SPECULARSWITCH_ON)
                    half3 halfDir = normalize(lightDirWS + viewDir);
                    half3 specular = _SpecularColor.rgb * pow(max(0, dot(normalWS, halfDir)), _Smoothness * 256) * _SpecularIntensity;
                    color += specular;
                #endif

                #if defined(FRESNELSWITCH_ON)
                    half3 fresnel = pow((1 - saturate(dot(normalWS, viewDir))), _FresnelPower) * _FresnelColor.rgb;
                    color += fresnel;
                #endif

                #if defined(ALPHACLIPPING_ON)
                    half alphaTest = albedo.a;
                    clip(alphaTest - _AlphaClipThreshold);
                    _Alpah *= alphaTest;
                #endif

                #if defined(FOGSWITCH_ON)
                    color.rgb = MixFog(color.rgb, input.fogFactor);
                #endif

                #if defined(HSISWITCH_ON)
                    color *= _Brightness;
                    half gray = dot(color, half3(0.2125, 0.7154, 0.0721));
                    color = lerp(half3(gray, gray, gray), color, _Saturation);
                    color = lerp(half3(0.5, 0.5, 0.5), color, _Contrast);
                #endif

                return half4(color, _Alpah);
            }
            ENDHLSL
        }
    }
}