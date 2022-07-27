Shader "Unlit/RipplesMesh"
{
    Properties
    {
        _ColorUp ("Color Up", Color) = (1,1,1,1)
        _ColorDown ("Color Down", Color) = (1,1,1,1)
        _WaveAmp ("Wave Amplitude", Range(0, 0.2)) = 0.1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530717958647692

            float4 _ColorUp;
            float4 _ColorDown;
            float _WaveAmp;
            
            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData m)
            {
                Interpolators o;
                float wave = -cos( (m.uv.y - _Time.y * 0.1) * TAU * 5);
                m.vertex.y = wave * _WaveAmp;
                
                o.vertex = UnityObjectToClipPos(m.vertex);
                o.normal = m.normal;
                o.uv = m.uv;
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                //return float4(i.uv, 0,1);
                
                float wave = cos( (i.uv.y - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                return wave;
            }
            ENDCG
        }
    }
}
