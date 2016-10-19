float3 ShiftTangent(float3 tangent, float3 normal, float shift)
{
	return normalize(tangent + shift*normal);
}

float3 Specular(float3 tangent, float3 viewDir, float3 lightDir, float exponent)
{
	float3 h = normalize(viewDir + lightDir);
	float3 dotTH = dot(tangent, h);
	float3 sinTH = sqrt(1.0 - dotTH*dotTH);
	float dirAtten = smoothstep(-1.0, 0.0, dotTH);
	return dirAtten * pow(sinTH, exponent);
}

float3 Diffuse(float3 normal, float3 lightDir, float softness)
{
	float dotNL = saturate(dot(normal, lightDir));
	return saturate(lerp(softness, 1, dotNL));
}