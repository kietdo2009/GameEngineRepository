float4x4 World; // World Matrix
float4x4 View; // View Matrix
float4x4 Projection; // Projection Matrix
float3 LightPosition; // in world space
float3 CameraPosition; // in world space
float Shininess; // scalar value
float3 AmbientColor;
float3 DiffuseColor;
float3 SpecularColor;
texture DiffuseTexture;

sampler DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexInput
{
    float4 Position : POSITION0; // Here, POSITION0 and NORMAL0
	float4 Normal : NORMAL0; // are called mnemonics
	float2 UV: TEXCOORD0;
};
struct GouraudVertexOutput
{
    float4 Position : POSITION0;
	float3 Color : COLOR0;
};

// ********************************
struct PhongVertexOutput
{
	float4 Position : POSITION0;
	float2 UV: TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};
PhongVertexOutput PhongVertex(VertexInput input)
{
	PhongVertexOutput output;
	
	output.WorldPosition = mul(input.Position, World);
	float4 viewPosition = mul(output.WorldPosition, View);
	output.Position = mul(viewPosition, Projection);
	
	output.WorldNormal = mul(input.Normal, World).xyz;
	output.UV = input.UV * 10;
	return output;
}
float4 PhongPixel(PhongVertexOutput input) : COLOR0
{
	float3 lightDirection = normalize(LightPosition - input.WorldPosition.xyz);
	float3 viewDirection = normalize(CameraPosition - input.WorldPosition.xyz);
	input.WorldNormal = normalize(input.WorldNormal);
	float3 reflectDirection = -reflect(lightDirection, input.WorldNormal);
	
	float3 diffuse = max(dot(lightDirection, input.WorldNormal), 0) * tex2D(DiffuseSampler, input.UV).rgb;
	float specular = pow(max(dot(reflectDirection, viewDirection), 0), Shininess);
	return float4(AmbientColor + diffuse * DiffuseColor  + specular * SpecularColor, 1);
}

float4 BlinnPixel(PhongVertexOutput input) : COLOR0
{
	float3 lightDirection = normalize(LightPosition - input.WorldPosition.xyz);
	float3 viewDirection = normalize(CameraPosition - input.WorldPosition.xyz);
	input.WorldNormal = normalize(input.WorldNormal);
	float3 reflectDirection = -reflect(lightDirection, input.WorldNormal);
	
	float3 diffuse = max(dot(lightDirection, input.WorldNormal), 0) * tex2D(DiffuseSampler, input.UV).rgb;
	float specular = pow(max(dot( normalize(lightDirection+viewDirection), input.WorldNormal), 0), Shininess);
	return float4(AmbientColor + diffuse * DiffuseColor + specular * SpecularColor, 1);
}
//********************************

GouraudVertexOutput GouraudVertex(VertexInput input)
{
	GouraudVertexOutput output; // create the output struct
	
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	
	float3 lightDirection = normalize(LightPosition - worldPosition.xyz);
	float3 viewDirection = normalize(CameraPosition - worldPosition.xyz);
	float3 worldNormal = mul(input.Normal, World).xyz;
	float3 reflectDirection = -reflect(lightDirection, worldNormal);
	
	float diffuse = max(dot(lightDirection, worldNormal), 0);
	float specular = pow(max(dot(reflectDirection, viewDirection), 0), Shininess);
	
	output.Color = AmbientColor + diffuse * DiffuseColor + specular * SpecularColor;
    return output;
}

float4 GouraudPixel(GouraudVertexOutput input) : COLOR0
{
    return float4(input.Color, 1);
}
technique Gouraud
{
    pass Pass1
    {
		VertexShader = compile vs_4_0 GouraudVertex();
		PixelShader = compile ps_4_0 GouraudPixel();
    }
}
technique Phong
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertex();
		PixelShader = compile ps_4_0 PhongPixel();
	}
}
technique Blinn
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertex();
		PixelShader = compile ps_4_0 BlinnPixel();
	}
}