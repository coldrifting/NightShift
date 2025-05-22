// Uses a custom ambient light parameter and ignores reflections

// Credit goes to LGHassen's Deferred mod for a lot of this
Shader "NightShift/AmbientOverride/Diffuse Ground KSC Specular"
{
    Properties
    {
        _NearGrassTexture("Near Grass Color Texture", 2D) = "gray" {}
        _TarmacTexture("Ground Color Texture", 2D) = "gray" {}
        _NearGrassNormal("Near Grass Normal Texture", 2D) = "gray" {}
        _TarmacNormal("Ground Normal Texture", 2D) = "gray" {}
        _BlendMaskTexture("Blend Mask", 2D) = "gray" {}

        _GrassColor("Grass Color", Color) = (1.0, 1.0 ,1.0 ,1.0)
        _TarmacColor("Tarmac Color", Color) = (1.0, 1.0 ,1.0 ,1.0)

        _SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)

        [PerRendererData]_RimFalloff("Rim Falloff", Range(0.0, 10.0)) = 0.1
        [PerRendererData]_RimColor("Rim Color", Color) = (0.0, 0.0, 0.0, 0.0)
        
        _AmbientLightOverride("AmbientLightOverride", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" }

        Pass {
            Blend One One
        }

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

        sampler2D _NearGrassTexture;
        sampler2D _TarmacTexture;
        sampler2D _NearGrassNormal;
        sampler2D _TarmacNormal;
        sampler2D _BlendMaskTexture;

        float4 _GrassColor;
        float4 _TarmacColor;
        float4 _AmbientLightOverride;

        struct Input
        {
            float2 uv_TarmacTexture;
            float2 uv2_BlendMaskTexture;
            float3 viewDir;
            float3 vertexPos;
            float3 worldPos;
            float4 screenPos;

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
            // hack to equalize mismatching color settings in the KSC
            _TarmacColor = min(_TarmacColor, float4(0.89.xxx, 1.0));

            float4 groundColor = tex2D(_TarmacTexture, i.uv_TarmacTexture);
            float3 groundNormal = UnpackNormalDXT5nm(tex2D(_TarmacNormal, i.uv_TarmacTexture));
            float blendMask = tex2D(_BlendMaskTexture,(i.uv2_BlendMaskTexture));

            float cameraDistance = length(_WorldSpaceCameraPos - i.worldPos);

            // Blend between different scales of the grass texture depending on distance
            // This is different from how the stock shader works (there are parameters for preset scales and blend distances) but this looks fine
            float textureScale = 10.0;

            // Based on the distance figure out the two scales to blend between, log10 looked good
            float currentPower = max(log10(cameraDistance/textureScale), 0.0);
            float fractionalPart = frac(currentPower);
            currentPower -= fractionalPart;
            float nextPower = currentPower + 1;

            float currentScale = pow(10.0, currentPower) * textureScale;
            float nextScale = pow(10.0, nextPower) * textureScale;

            // Sample grass textures
            float4 grassColor10 = tex2D(_NearGrassTexture, i.vertexPos.xz / currentScale * 3.0);
            float4 grassColor11 = tex2D(_NearGrassTexture, i.vertexPos.xz / nextScale * 3.0);

            float3 grassNormal10 = UnpackNormalDXT5nm(tex2D(_NearGrassNormal, i.vertexPos.xz / currentScale * 3.0));
            float3 grassNormal11 = UnpackNormalDXT5nm(tex2D(_NearGrassNormal, i.vertexPos.xz / nextScale * 3.0));

            float4 grass = _GrassColor * lerp(grassColor10, grassColor11, fractionalPart);
            float3 grassNormal = lerp(grassNormal10, grassNormal11, fractionalPart);

            // Blend between groundColor and grass based on the blend mask, this appears to be working correctly
            float4 color;
            color.rgb = lerp(grass.rgb, _TarmacColor.rgb * groundColor, blendMask);

            float3 normal = lerp(grassNormal, groundNormal, blendMask);

            o.Albedo = color;
            o.Normal = normal;
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