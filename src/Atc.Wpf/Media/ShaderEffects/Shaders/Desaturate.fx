sampler2D Input : register(s0);
float Strength : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Input, uv);
    float3 rgb = color.rgb;
    float3 luminance = lerp(rgb, dot(rgb, float3(0.30, 0.59, 0.11)), Strength);
    return float4(luminance, color.a);
}