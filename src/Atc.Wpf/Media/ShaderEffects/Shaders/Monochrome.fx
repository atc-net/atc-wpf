sampler2D Input : register(s0);
float4 FilterColor : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float2 texuv = uv;
   float4 color = tex2D(Input, texuv);
   float4 luminance = color.r * 0.30 + color.g * 0.59 + color.b * 0.11;
   luminance.a = 1.0;
   return luminance * FilterColor;
}