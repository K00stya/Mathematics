
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct MeshData
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Interpolators
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0; //0
    float3 normal : TEXCOORD1; //1
    float3 wPos : TEXCOORD2; //2
    LIGHTING_COORDS(3,4) //3,4
};

sampler2D _MainTex;
sampler2D _Albedo;
float4 _MainTex_ST;
float4 _Color;
float _Gloss;

Interpolators vert (MeshData v)
{
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.wPos = mul(unity_ObjectToWorld, v.vertex);
    TRANSFER_VERTEX_TO_FRAGMENT(o)
    return o;
}

fixed4 frag (Interpolators i) : SV_Target
{
    float3 albedo = tex2D(_Albedo, i.uv).rgb;
    float3 surfaceColor = albedo * _Color.rgb;
    
    //deffuse
    float3 n = normalize(i.normal);
    float3 l = normalize(UnityWorldSpaceLightDir(i.wPos));
    float attenuation = LIGHT_ATTENUATION(i);
    float3 lambert = saturate(dot(n,l));
    float3 diffuseLight = (lambert * attenuation) * _LightColor0.xyz;
                
    //specular
    float3 v = normalize(_WorldSpaceCameraPos - i.wPos);
    float3 h = normalize(l + v);
    float3 specularLight = saturate(dot(h, n)) * (lambert > 0);

    float specularExponent = exp2(_Gloss * 11) + 2; 
    specularLight = pow(specularLight, specularExponent) * _Gloss * attenuation;
    specularLight *= _LightColor0.xyz;
                
    return float4( diffuseLight * surfaceColor + specularLight, 1);
                
    fixed4 col = tex2D(_MainTex, i.uv);
    return col;
}