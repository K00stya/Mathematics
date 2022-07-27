Shader "Unlit/TextureTest"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", Range(0,1)) = 1
        _BorderSize ("Border Size", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque"
                "Queue" = "Transparent" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Health;
            float _BorderSize;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return(v-a)/(b-a);
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                float2 coords = i .uv;
                coords.x *= 8;
                float2 pointSegment = float2(clamp(coords.x, 0.5, 7.5), 0.5);
                float sdf = distance (coords, pointSegment) * 2 - 1;
                clip(-sdf);

                float bordersSdf = sdf + _BorderSize;
                float pd = fwidth(bordersSdf);
                float borderMask = 1 - saturate(bordersSdf / pd);
                    
                float3 col = tex2D(_MainTex, float2(_Health, i.uv.y));
                float healthbarMask = _Health > floor(i.uv.x * 8) / 8;
                float4 color = float4(col * healthbarMask * borderMask, 1); 
                
                return color;
            }
            ENDCG
        }
    }
}
