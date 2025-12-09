float4x4 World;
float4x4 View;
float4x4 Projection;
float3 LightPosition;
float3 CameraPosition;
float Shininess;
float3 AmbientColor;
float3 DiffuseColor;
float3 SpecularColor;

// --- TEXTURE DEFINITIONS ---
texture NormalMap;
sampler NormalSampler = sampler_state {
    Texture = <NormalMap>;
    AddressU = Wrap;
    AddressV = Wrap;
};

// Define the texture for the maze pattern
texture DiffuseTexture; 
sampler DiffuseSampler = sampler_state {
    Texture = <DiffuseTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexInput
{
    float4 Position : POSITION0;
    float2 UV: TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION0;
    float2 UV: TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
};

VertexOutput TerrainVertexShader(VertexInput input)
{
    VertexOutput output;
    
    output.WorldPosition = mul(input.Position, World);
    float4 viewPosition = mul(output.WorldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.UV = input.UV;
    return output;
}

float4 TerrainPixelShader(VertexOutput input) : COLOR0
{
    // 1. Get the color from the Maze Image (Black or White)
    float4 texColor = tex2D(DiffuseSampler, input.UV);

    // 2. Get the Normal from the NormalMap
    float3 normal = tex2D(NormalSampler, input.UV).xzy; // Swizzle to match common formats
    normal = normalize(normal); 

    // 3. Calculate Lighting
    float3 lightDirection = normalize(LightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 reflectDirection = reflect(-lightDirection, normal); // Note: -lightDirection is standard

    float diffuse = max(dot(lightDirection, normal), 0);
    float specular = pow(max(dot(reflectDirection, viewDirection), 0), Shininess);

    // 4. Combine: Multiply Lighting by the Texture Color
    // This ensures black parts of the image stay black!
    float3 lighting = AmbientColor + diffuse * DiffuseColor + specular * SpecularColor;
    
    return float4(lighting * texColor.rgb, 1);
}

technique TerrainShader
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 TerrainVertexShader();
        PixelShader = compile ps_4_0 TerrainPixelShader();
    }
}