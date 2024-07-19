//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float random(float2 pos) {
    return frac(sin(dot(pos, float2(12.9898, 78.233))) * 43758.5453);
}

float interpolate(float a, float b, float t) {
    return (1.0-t)*a + (t*b);
}

float noiseIteration(float2 pos) {
    float2 i = floor(pos);
    float2 f = frac(pos);
    f = f * f * (3.0 - 2.0 * f);

    pos = abs(frac(pos) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = random(c0);
    float r1 = random(c1);
    float r2 = random(c2);
    float r3 = random(c3);

    float bottomOfGrid = interpolate(r0, r1, f.x);
    float topOfGrid = interpolate(r2, r3, f.x);
    float t = interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float2 noiseForPosition(float x, float y, float scale, float freq, float amp) {
    return noiseIteration(float2(x * scale / freq, y * scale / freq)) * amp;
}

float noise(float2 pos, float scale, float freqSource, float ampSource) {
    float t = 0.0;

    float freq = pow(freqSource, 0);
    float amp = pow(ampSource, 3);
    t += noiseForPosition(pos.x, pos.y, scale, freq, amp);

    freq = pow(freqSource, 1);
    amp = pow(ampSource, 2);
    t += noiseForPosition(pos.x, pos.y, scale, freq, amp);

    freq = pow(freqSource, 2);
    amp = pow(ampSource, 1);
    t += noiseForPosition(pos.x, pos.y, scale, freq, amp);

    return t;
}

void GroundColor_float(float4 GroundColor, float3 position, float scale, float frequency, float amplitude, float colorBands, float variationStrength, float heightInfluence, out float4 Out)
{
    float noiseValue = noise(position.xz * position.y, scale, frequency, amplitude);
    float bandedValue = 1 / (floor(noiseValue * (colorBands)) + 1) - 0.5;
    Out = GroundColor * (1 + bandedValue * variationStrength) + ((sin(position.y) + 1) / 2 * heightInfluence);
}
#endif //MYHLSLINCLUDE_INCLUDED
