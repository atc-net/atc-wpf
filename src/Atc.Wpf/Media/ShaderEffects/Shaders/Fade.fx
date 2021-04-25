sampler2D Input : register(s0);
float Strength : register(c0);
float4 ToColor : register(c2);

float4 lerp_rgba(float4 x, float4 y, float s)
{
    float a = lerp(x.a, y.a, s);
    float3 rgb = lerp(x.rgb, y.rgb, s) * a;
    return float4(rgb.r, rgb.g, rgb.b, a);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Input, uv);
    return lerp_rgba(color, ToColor, Strength);
}