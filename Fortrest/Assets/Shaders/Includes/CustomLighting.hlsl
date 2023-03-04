
#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#if (SHADERPASS != SHADERPASS_FORWARD)
#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#endif
#endif

struct CustomLightingData {

    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;


    float3 albedo;
    float smoothness;
    float ambientOcclusion;


    float3 bakedGI;
    float4 shadowMask;
    float fogFactor;
};


float GetSmoothnessPower(float rawSmoothness) {
    return exp2(10 * rawSmoothness + 1);
}

void CalculateMainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out half DistanceAtten, out half ShadowAtten) {
#ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}


#ifndef SHADERGRAPH_PREVIEW
float3 CustomGlobalIllumination(CustomLightingData d) {
    float3 indirectDiffuse = d.albedo * d.bakedGI * d.ambientOcclusion;

    float3 reflectVector = reflect(-d.viewDirectionWS, d.normalWS);

    float fresnel = Pow4(1 - saturate(dot(d.viewDirectionWS, d.normalWS)));

    float3 indirectSpecular = GlossyEnvironmentReflection(reflectVector,
        RoughnessToPerceptualRoughness(1 - d.smoothness),
        d.ambientOcclusion) * fresnel;

    return indirectDiffuse + indirectSpecular;
}

float3 CustomLightHandling(CustomLightingData d, Light light) {

    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);

    float diffuse = saturate(dot(d.normalWS, light.direction));
    float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
    float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * diffuse;

    float3 color = d.albedo * radiance * (diffuse + specular);

    return color;
}
#endif

float3 CalculateCustomLighting(CustomLightingData d) {
#ifdef SHADERGRAPH_PREVIEW

    float3 lightDir = float3(0.5, 0.5, 0);
    float intensity = saturate(dot(d.normalWS, lightDir)) +
        pow(saturate(dot(d.normalWS, normalize(d.viewDirectionWS + lightDir))), GetSmoothnessPower(d.smoothness));
    return d.albedo * intensity;
#else

    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, d.shadowMask);

    MixRealtimeAndBakedGI(mainLight, d.normalWS, d.bakedGI);
    float3 color = CustomGlobalIllumination(d);
    color += CustomLightHandling(d, mainLight);

#ifdef _ADDITIONAL_LIGHTS

    uint numAdditionalLights = GetAdditionalLightsCount();
    for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
        Light light = GetAdditionalLight(lightI, d.positionWS, d.shadowMask);
        color += CustomLightHandling(d, light);
    }
#endif

    color = MixFog(color, d.fogFactor);

    return color;
#endif
}

void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection,
    float3 Albedo, float Smoothness, float AmbientOcclusion,
    float2 LightmapUV,
    out float3 Color) {

    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.albedo = Albedo;
    d.smoothness = Smoothness;
    d.ambientOcclusion = AmbientOcclusion;

#ifdef SHADERGRAPH_PREVIEW

    d.shadowCoord = 0;
    d.bakedGI = 0;
    d.shadowMask = 0;
    d.fogFactor = 0;
#else

    float4 positionCS = TransformWorldToHClip(Position);
#if SHADOWS_SCREEN
    d.shadowCoord = ComputeScreenPos(positionCS);
#else
    d.shadowCoord = TransformWorldToShadowCoord(Position);
#endif


    float2 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);

    float3 vertexSH;
    OUTPUT_SH(Normal, vertexSH);

    d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);

    d.shadowMask = SAMPLE_SHADOWMASK(lightmapUV);

    d.fogFactor = ComputeFogFactor(positionCS.z);
#endif

    Color = CalculateCustomLighting(d);
}

#endif