sampler2D Input : register(s0);
float Brightness : register(c0);
float Contrast : register(c1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Input, uv);
    float4 result = color;
    result = color + Brightness;
    result = ((result - 0.5) * pow((Contrast + 1.0) / 1.0, 2)) + 0.5;
    result.a = color.a;
    return result;
}