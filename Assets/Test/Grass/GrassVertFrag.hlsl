#ifndef STYLIZED_FOLIAGE_INCLUDED
#define STYLIZED_FOLIAGE_INCLUDED

inline float SineWave(float3 pos, float offset, float speed, float frequency)
{
    const float angle = offset + _WindDirection * PI;
    const float s = sin(angle);
    const float c = cos(angle);
    return sin(offset + _Time.z * speed + (pos.x * s + pos.z * c) * frequency);
}

void GrassVert_float(float3 PositionOS, float3 PositionWS, float3 ObjectScale, float2 UV, out float3 PositionOut,
out float3 ColorOut) {
    PositionOut = PositionOS;
    ColorOut = _ColorTop.rgb;

    float s = SineWave(PositionOS, 0, _WindSpeed, _WindFrequency);

    s = s * UV.y * ObjectScale.y * _WindIntensity;

    PositionOut.xz += s * 0.1;
    PositionOut.y += s * 0.02;
}

void GrassFrag_float(float4 BaseMapColor, float2 UV, float3 ColorTop, out float3 Color, out float Alpha) {
    const float3 albedo = lerp(_ColorBottom.rgb, ColorTop.rgb, UV.y);
    Color = BaseMapColor.rgb * albedo;
    Alpha = BaseMapColor.a;
}

#endif