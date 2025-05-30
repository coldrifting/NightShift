Shader "NightShift/AmbientOverride/Flag"
{
    // Uses a custom ambient light parameter and ignores reflections
    Properties
    {
        _AmbientLightOverride ("AmbientLightOverride", Color) = (1, 1, 1, 1)

        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("MainTex", 2D) = "gray" {}
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
        }

        Stencil
        {
            Ref 3
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf StandardSpecular noambient nolightmap
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

        float3 overlay(float3 base, float4 overlay) {
            return clamp((overlay * overlay.a) + ((1 - overlay.a) * base), 0, 1);
        }

        void surf(Input i, inout SurfaceOutputStandardSpecular o)
        {
            float4 detail = tex2D(_MainTex, i.uv_MainTex);
            float3 finalColor = overlay(_Color, detail);

            o.Albedo = finalColor;
            o.Emission = o.Albedo * _AmbientLightOverride;
        }

        ENDCG
    }
    Fallback "Diffuse"
}