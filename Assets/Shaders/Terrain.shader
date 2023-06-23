Shader "Custom/Terrain"
{
    Properties
    {
        _GroundColor ("Ground", Color) = (0, 0, 0, 1)
        _HillThreshold ("Hill Threshold", float) = 0.3
        _HillColor ("Hill", Color) = (0, 0, 0, 1)
        _CliffThreshold ("Cliff Threshold", float) = 0.8
        _CliffColor ("Cliff", Color) = (0, 0, 0, 1)
        _Scale ("Scale", float) = 1
        _Freq ("Frequency", float) = 2.0
        _Amp ("Amplitude", float) = 0.5
        _ColorBands ("Color Bands", int) = 1
        _VariationStrength ("Variation Strength", float) = 0.1
        _HeightInfluence ("Height Influence", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        struct Input
        {
            float4 screenPos;
            float3 worldPos;
            float3 worldNormal;
            float eyeDepth;
        };

        uniform fixed4 _GroundColor;
        uniform fixed4 _HillColor;
        uniform fixed4 _CliffColor;

        uniform float _HillThreshold;
        uniform float _CliffThreshold;

        uniform float _Scale;
        uniform float _Freq;
        uniform float _Amp;
        uniform float _VariationStrength;
        uniform int _ColorBands;
        uniform float _HeightInfluence;

        // Yoinked (and adapted) from Shaderlabs Simple Noise Node
        inline float random (float2 pos) {
            return frac(sin(dot(pos, float2(12.9898, 78.233))) * 43758.5453);
        }

        inline float interpolate (float a, float b, float t) {
            return (1.0-t)*a + (t*b);
        }

        inline float noiseIteration (float2 pos) {
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

        inline float2 noiseForPosition(float x, float y, float scale, float freq, float amp) {
            return noiseIteration(float2(x * scale / freq, y * scale / freq)) * amp;
        }

        inline float noise(float2 pos, float scale, float freqSource, float ampSource) {
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
        // End of Yoinked code

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float steepness = 1 - dot(float3(0, 1, 0), IN.worldNormal);
            if (steepness >= _CliffThreshold) {
                o.Albedo = _CliffColor;
            } else if (steepness >= _HillThreshold) {
                o.Albedo = _HillColor;
            } else {
                float noiseValue = noise(IN.worldPos.xz * IN.worldPos.y, _Scale, _Freq, _Amp);
                float bandedValue = 1 / (floor(noiseValue * (_ColorBands)) + 1) - 0.5;
                o.Albedo = _GroundColor * (1 + bandedValue * _VariationStrength) + (sin(IN.worldPos.y) * _HeightInfluence);
            }

            o.Smoothness = 0.1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
