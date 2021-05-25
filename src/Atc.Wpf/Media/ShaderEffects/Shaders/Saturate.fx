sampler2D Input : register(s0);
sampler2D SecondInput : register(s1);
float Progress : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR0
{
    float4 color1 = tex2D(SecondInput, uv);
    color1 = saturate(color1 * (2 * Progress + 1));
    float4 color2 = tex2D(Input, uv);

    if (Progress > 0.8)
    {
        float newProgress = (Progress - 0.8) * 5.0;
        return lerp(color1, color2, newProgress);
    }
    else
    {
        return color1;
    }
}