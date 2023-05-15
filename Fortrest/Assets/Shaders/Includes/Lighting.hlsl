#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED
//here
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



//to here
float GetLightIntensity(float3 color) {
    return max(color.r, max(color.g, color.b));
}

#ifndef SHADERGRAPH_PREVIEW
Light GetAdditionalLightForToon(int pixelLightIndex, float3 worldPosition) {
    int perObjectLightIndex = GetPerObjectLightIndex(pixelLightIndex);
    Light light = GetAdditionalPerObjectLight(perObjectLightIndex, worldPosition);
    light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, worldPosition);
    return light;
}
#endif

void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
    float MainDiffuse, float MainSpecular, float3 MainColor,
    out float Diffuse, out float Specular, out float3 Color) {

    float mainIntensity = GetLightIntensity(MainColor);
    Diffuse = MainDiffuse * mainIntensity;
    Specular = MainSpecular * mainIntensity;
    Color = MainColor;

#ifndef SHADERGRAPH_PREVIEW
    float highestDiffuse = Diffuse;

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i) {
        Light light = GetAdditionalLightForToon(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation * GetLightIntensity(light.color);
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        Diffuse += thisDiffuse;
        Specular += thisSpecular;

        if (thisDiffuse > highestDiffuse) {
            highestDiffuse = thisDiffuse;
            Color = light.color;
        }
    }
#endif
}

#endif