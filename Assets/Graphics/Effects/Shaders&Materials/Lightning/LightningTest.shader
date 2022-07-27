Shader "Unlit/Lightning"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Albedo ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }

        //Base pass
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "FGLightning.cginc"
            ENDCG
        }
        
        //Add pass
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }
            Blend one one
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "FGLightning.cginc"
            ENDCG
        }
    }
}
