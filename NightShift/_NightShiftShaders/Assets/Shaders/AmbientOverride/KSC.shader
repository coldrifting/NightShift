Shader "NightShift/AmbientOverride/KSC"
{
    // Uses a custom ambient light parameter and ignores reflections
    // Credit goes to LGHassen's Deferred mod for a lot of this
    Properties
    {
        _AmbientLightOverride ("AmbientLightOverride", Color) = (1, 1, 1, 1)

        _NearGrassTexture ("Near Grass Color Texture", 2D) = "gray" { }
        _TarmacTexture ("Ground Color Texture", 2D) = "gray" { }
        _NearGrassNormal ("Near Grass Normal Texture", 2D) = "gray" { }
        _TarmacNormal ("Ground Normal Texture", 2D) = "gray" { }
        _BlendMaskTexture ("Blend Mask", 2D) = "gray" { }

        _GrassColor ("Grass Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _TarmacColor ("Tarmac Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        //Pass
        //{
        //    Blend One One
        //}

        Stencil 
        { 
            Ref 3 
            Comp Always 
            Pass Replace 
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert noambient nolightmap
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF
        #pragma multi_compile_fog
        #pragma target 3.0

        float4 _AmbientLightOverride;

        sampler2D _NearGrassTexture;
        sampler2D _TarmacTexture;
        sampler2D _NearGrassNormal;
        sampler2D _TarmacNormal;
        sampler2D _BlendMaskTexture;

        float4 _GrassColor;
        float4 _TarmacColor;

        struct Input
        {
            float2 uv_TarmacTexture;
            float2 uv2_BlendMaskTexture;
            float3 viewDir;
            float3 vertexPos;
            float3 worldPos;
            float4 screenPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexPos = v.vertex;
        }

        void surf(Input i, inout SurfaceOutput o)
        {
            // Hack to equalize mismatching color settings in the KSC
            _TarmacColor = min(_TarmacColor, float4(0.89.xxx, 1.0));

            float4 groundColor = tex2D(_TarmacTexture, i.uv_TarmacTexture);
            float3 groundNormal = UnpackNormalDXT5nm(tex2D(_TarmacNormal, i.uv_TarmacTexture));
            float blendMask = tex2D(_BlendMaskTexture, (i.uv2_BlendMaskTexture));

            float cameraDistance = length(_WorldSpaceCameraPos - i.worldPos);
            // Blend between different scales of the grass texture depending on distance

            // This is different from how the stock shader works (there are parameters for preset scales and blend distances) but this looks fine
            float textureScale = 10.0;

            // Based on the distance figure out the two scales to blend between, log10 looked good
            float currentPower = max(log10(cameraDistance / textureScale), 0.0);
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
            o.Emission = o.Albedo * _AmbientLightOverride;
        }

        ENDCG
    }

    FallBack "Diffuse"
}