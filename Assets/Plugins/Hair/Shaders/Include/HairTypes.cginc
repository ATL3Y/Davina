struct VS_OUTPUT
{
	uint id : TEXCOORD0;
};

struct HS_OUTPUT
{
	uint id : TEXCOORD0;
};

struct HS_CONSTANT_OUTPUT
{
	float edges[2] : SV_TessFactor;
};

struct DS_OUTPUT
{
	float4 position : SV_Position;
	float3 tangent: TANGENT;
	float3 normal : NORMAL;
	float4 factor : TEXCOORD0; //x - E(0,1) 0 is root, 1 is tip // y - edge factor E(0,1) 0 is edge //z - shading
	float3 lightDir: TEXCOORD1;
	float3 viewDir : TEXCOORD2;
};

struct GS_OUTPUT
{
	float4 pos   : SV_Position;
	float3 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 factor : TEXCOORD0;
	float3 lightDir: TEXCOORD1;
	float3 viewDir : TEXCOORD2;
};

struct DS_OUTPUT_SHADOW
{
	float4 position : SV_Position;
	float3 tangent: TANGENT;
};

