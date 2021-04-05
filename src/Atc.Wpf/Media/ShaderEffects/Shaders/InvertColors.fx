sampler2D Input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = tex2D(Input, uv);
   float4 result = 1 - color;
   result.a = color.a;
   result.rgb *= result.a;
   return result;
}