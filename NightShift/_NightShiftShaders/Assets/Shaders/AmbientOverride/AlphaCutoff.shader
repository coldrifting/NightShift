Shader "NightShift/AmbientOverride/AlphaCutoff"
{
    // Uses a custom ambient light parameter and ignores reflections
    Properties
    {
        _AmbientLightOverride ("AmbientLightOverride", Color) = (1, 1, 1, 1)

        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Color Map", 2D) = "gray" { }
        _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "IgnoreProjector" = "True"
            "Queue" = "AlphaTest"
        }

        Stencil 
        { 
            Ref 3 
            Comp Always 
            Pass Replace 
        }

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf StandardSpecular noambient nolightmap alphatest:_Cutoff
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF
        #pragma target 3.0

        float4 _AmbientLightOverride;

        float4 _Color;
        sampler2D _MainTex;

        struct Input
        {
            float4 pos : SV_POSITION;
            float2 uv_MainTex : TEXCOORD0;
        };

        void vert(inout appdata_full v, out Input o)
        {
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv_MainTex = v.texcoord;
        }

        void surf(Input i, inout SurfaceOutputStandardSpecular o)
        {
            float4 finalColor = tex2D(_MainTex, i.uv_MainTex) * _Color;

            o.Albedo = finalColor;
            o.Alpha = finalColor.a;
            o.Emission = o.Albedo * _AmbientLightOverride;
        }

        ENDCG
    }

    FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}