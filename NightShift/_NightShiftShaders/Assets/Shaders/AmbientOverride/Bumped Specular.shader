// Uses a custom ambient light parameter and ignores reflections
Shader "NightShift/AmbientOverride/Bumped Specular"
{
    Properties
    {
        _MainTex("Color Map", 2D) = "gray" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Emissive("Emissive Map", 2D) = "white" {}
        _Shininess ("Shininess", Range (0.0, 1.0)) = 0.5
        _Color ("Part Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _EmissiveColor("Emissive Color", Color) = (0.0, 0.0, 0.0, 1.0)
        [PerRendererData]_RimFalloff("Rim Falloff", Range(0.0, 10.0) ) = 0.1
        [PerRendererData]_RimColor("Rim Color", Color) = (0.0, 0.0, 0.0, 0.0)
        [PerRendererData]_TemperatureColor("Temperature Color", Color) = (0.0, 0.0, 0.0, 0.0)
        [PerRendererData]_BurnColor ("Burn Color", Color) = (1.0, 1.0, 1.0, 1.0)
        
        _AmbientLightOverride("AmbientLightOverride", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" }

        Stencil
        {
            Ref 3
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vertexShader noambient finalcolor:finalColor
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF
        #pragma multi_compile_fog
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Emissive;

        float4 _Color;
        float4 _EmissiveColor;
        float4 _AmbientLightOverride;

        struct Input
        {
            float2 uv_MainTex;
            float3 vertexPos;
            
            // UNITY_TRANSFER_FOG macro needs this to be called fogCoord
            float fogCoord;
        };

        void vertexShader(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexPos = v.vertex;

            // Setup fogCoord for UNITY_TRANSFER_FOG macro 
            float4 clipPos = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o, clipPos);
        }

        void surf(Input i, inout SurfaceOutput o)
        {
            float4 color = _Color * tex2D(_MainTex, i.uv_MainTex);

            o.Albedo = color;
            o.Emission = color * _AmbientLightOverride;

//#if UNITY_PASS_DEFERRED
//            // In deferred rendering do not use the flat ambient because Deferred adds its own ambient as a composite of flat ambient and probe
//            // Also do not use #pragma skip_variants LIGHTPROBE_SH because it impacts lighting in forward and some elements can still render in
//            // forward e.g through the VAB scene doors
//            unity_SHAr = 0.0.xxxx;
//            unity_SHAg = 0.0.xxxx;
//            unity_SHAb = 0.0.xxxx;
//#endif
        }

        void finalColor(Input IN, SurfaceOutput o, inout float4 color)
        {
            UNITY_APPLY_FOG(IN.fogCoord, color);
        }


        ENDCG
    }
    Fallback "Diffuse"
}