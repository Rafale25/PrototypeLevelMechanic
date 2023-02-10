Shader "Hidden/Custom/ColorCompression"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

        float _Blend;

        static const float steps = 12.0;

        // static const float palette_size = 9;

        // static const float3 palette[palette_size] = {
        //     float3(0.055, 0.047, 0.001),
        //     float3(0.314, 0.251, 0.027),
        //     float3(0.380, 0.345, 0.051),
        //     float3(0.475, 0.475, 0.264),
        //     float3(0.513, 0.576, 0.302),
        //     float3(0.541, 0.749, 0.420),
        //     float3(0.584, 0.878, 0.592),
        //     float3(0.635, 0.984, 0.823),
        //     float3(0.800, 1.000, 0.900),
        // };

        // static const float palette_size = 11;

        // static const float3 palette[palette_size] = {
        //     float3(35.0/255.0, 9.0/255.0, 53.0/255.0),
        //     float3(71.0/255.0, 18.0/255.0, 107.0/255.0),
        //     float3(87.0/255.0, 16.0/255.0, 137.0/255.0),
        //     float3(100.0/255.0, 17.0/255.0, 173.0/255.0),
        //     float3(109.0/255.0, 35.0/255.0, 182.0/255.0),
        //     float3(130.0/255.0, 47.0/255.0, 175.0/255.0),
        //     float3(151.0/255.0, 58.0/255.0, 168.0/255.0),
        //     float3(172.0/255.0, 70.0/255.0, 161.0/255.0),
        //     float3(192.0/255.0, 82.0/255.0, 153.0/255.0),
        //     float3(213.0/255.0, 93.0/255.0, 146.0/255.0),
        //     float3(234.0/255.0, 105.0/255.0, 139.0/255.0),
        // };

        // float map(float value, float min1, float max1, float min2, float max2)
        // {
        //     float perc = (value - min1) / (max1 - min1);

        //     value = perc * (max2 - min2) + min2;
        //     return value;
        // }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            // color.rgb = luminance.xxx;
            color.rgb = luminance * float3(234.0/255.0, 140.0/255.0, 85.0/255.0);

            // float intensity = float(int((1.0 - pow(1.0 - color.g, 3.0)) * steps)) / steps;
            // color.rgb *= intensity;

            // color.rgb = round(color.rgb * (palette_size - 1.0)) / (palette_size - 1.0);
            // color.rgb = palette[int(color.r * (palette_size - 1.0))];

            // color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);

            return color;
        }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
