Shader "Hidden/Custom/ColorCompression"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

        float _Blend;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float steps = 16.0;
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            // float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));

            float intensity = float(int((1.0 - pow(1.0 - color.g, 3.0)) * steps)) / steps;
            color.rgb *= intensity;

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
