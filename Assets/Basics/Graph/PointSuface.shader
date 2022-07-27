Shader "Custom/PointSuface"
{
    Properties
    {
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        float Smoothness;

        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            surface.Albedo = input.worldPos;
            surface.Smoothness = 0.5f;
        }
        
		ENDCG
    }
    FallBack "Diffuse"
}
