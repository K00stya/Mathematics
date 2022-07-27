Shader "Custom/Ring"
{
    Properties
    {
        _ColorUp ("ColorUp", Color) = (1,1,1,1)
        _ColorDown ("ColorDown", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass {
            
            Zwrite off
            Cull off
            Blend SrcAlpha OneMinusSrcAlpha 
             
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #define TAU 6.28318530717958647692

        float4 _ColorUp;
        float4 _ColorDown;

        struct MeshData
        {
            float3 vertex : POSITION;
            float3 normal : NORMAL;
            float4 uv0 : TEXCOORD0;
        };

        struct Interpolators
        {
            float4 vertex : SV_POSITION;
            float3 normal : TEXCOORD0;
            float2 uv : TEXCOORD1;
        };

        Interpolators vert (MeshData m)
        {
            Interpolators i;
            i.vertex = UnityObjectToClipPos(m.vertex);
            i.normal = m.normal;
            i.uv = m.uv0;
            return i;
        }

        float4 frag( Interpolators i) : SV_Target {
            float xOffset = cos(i.uv.x * TAU * 8) * 0.1 * 0.5 + 0.5;
            float t = cos( (i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
            t *= 1-i.uv.y;
            
            float topBottomRemover = abs(i.normal.y) < 0.999;
            float waves = t * topBottomRemover;
            float4 color = lerp(_ColorDown, _ColorUp, i.uv.y);
            return color * waves;
        }
        
        ENDCG
        }
    }
    FallBack "Unlit"
}
