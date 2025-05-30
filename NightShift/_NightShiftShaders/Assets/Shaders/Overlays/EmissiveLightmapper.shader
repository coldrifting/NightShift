Shader "NightShift/Overlays/EmissiveLightmapper"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)

        _MainTex ("_MainTex", 2D) = "white" { }
        _OverlayTex ("_OverlayTex", 2D) = "black" { }

        _Illum ("_Illum", 2D) = "black" { }
        _EmissiveColor ("_EmissiveColor", Color) = (1.0, 1.0, 1.0, 1.0)

        _SkyMaskTex ("_SkyMaskTex", 2D) = "white" { }
        _SkyColor ("SkyColor", Color) = (1.0, 1.0, 1.0, 1.0)

        [Toggle(_LIGHTMAP_ON)] _LightmapOn ("Lightmap Enabled", Float) = 1.0
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
        #pragma surface surf StandardSpecular noambient nolightmap noforwardadd
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF
        #pragma multi_compile __ _LIGHTMAP_ON
        #pragma target 3.0

        #include "UnityCG.cginc"

        float4 _Color;
        float4 _EmissiveColor;

        float4 _SkyColor;
        sampler2D _SkyMaskTex;

        sampler2D _MainTex;
        sampler2D _OverlayTex;
        sampler2D _Illum;

        struct Input
        {
            float4 pos : SV_POSITION;
            float2 uv_MainTex : TEXCOORD0;
            float2 lmap : TEXCOORD1;
        };

        float3 overlay(float3 base, float4 overlay)
        {
            return clamp((overlay * overlay.a) + ((1 - overlay.a) * base), 0, 1);
        }
        
        void vert(inout appdata_full v, out Input o)
        {
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv_MainTex = v.texcoord;
            o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        }

        void surf(Input i, inout SurfaceOutputStandardSpecular o)
        {
            float4 baseColor = tex2D(_MainTex, i.uv_MainTex);
            float4 overlayColor = tex2D(_OverlayTex, i.uv_MainTex);
            float3 finalColor = overlay(baseColor.rgb, overlayColor) * _Color;

            #ifdef _LIGHTMAP_ON
                float hdr = unity_Lightmap_HDR.x;

                float4 lmap_raw = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap);
                float3 lightmap = lmap_raw.rgb * hdr * lmap_raw.a;
                finalColor *= lightmap;
            #else
                finalColor *= 0.5;
            #endif

            float emissive = tex2D(_Illum, i.uv_MainTex).r;
            float skyMask = tex2D(_SkyMaskTex, i.uv_MainTex).r;

            float3 skyEmissive = emissive * skyMask * _SkyColor;
            float3 lightsEmissive = emissive * (1 - skyMask) * _EmissiveColor;

            float3 emissiveTotal = skyEmissive + lightsEmissive;

            o.Albedo = finalColor;
            o.Emission = o.Albedo + emissiveTotal;
        }

        ENDCG
    }

    FallBack "Diffuse"
}