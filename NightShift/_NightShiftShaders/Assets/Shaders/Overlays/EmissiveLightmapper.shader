Shader "NightShift/Overlays/Emissive/Lightmapper"
{
    Properties
    {
        _Color ("_Color", Color) = (1.0, 1.0, 1.0, 1.0)

        _MainTex("_MainTex", 2D) = "gray" {}
        _NightOverlayTex("_NightOverlayTex", 2D) = "gray" {}

        _Illum("_Illum", 2D) = "black" {}
        _EmissiveColor("_EmissiveColor", Color) = (0.0, 0.0, 0.0, 1.0)

        _NightCrossFade("_NightCrossFade", Range(0.0, 1.0)) = 0.0

        [Toggle(_UseLightmap)] _UseLightmap ("_UseLightmap", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Stencil
        {
            Ref 3
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        #pragma surface surf StandardSpecular nolightmap vertex:vert
        #pragma shader_feature _ _UseLightmap
        #pragma target 3.0


        float4 _Color;
        float4 _EmissiveColor;

        float _NightCrossFade;

        sampler2D _MainTex;
        sampler2D _NightOverlayTex;
        sampler2D _Illum;
        sampler2D _LightMap;

        struct Input
        {
            float2 uv_MainTex;

            // Must start with lmap
            float2 lmap;
        };

        float3 overlay(float3 base, float4 overlay) {
            return clamp((overlay * overlay.a) + ((1 - overlay.a) * base), 0, 1);
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.uv_MainTex = v.texcoord.xy;
            o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float3 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            float4 nightOverlayColor = _NightCrossFade * tex2D(_NightOverlayTex, IN.uv_MainTex);
            float3 emissive = tex2D(_Illum, IN.uv_MainTex);

            float3 finalBaseColor = overlay(baseColor, nightOverlayColor) * _Color;

            #ifdef _UseLightmap
                float3 lmapColor = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap);
                o.Albedo = finalBaseColor * lmapColor;
            #else
                o.Albedo = finalBaseColor;
            #endif

            o.Emission = emissive * _EmissiveColor * (1 - _NightCrossFade);
        }
        ENDCG
    }
    FallBack "Legacy Shaders/Self-Illumin/VertexLit"
}