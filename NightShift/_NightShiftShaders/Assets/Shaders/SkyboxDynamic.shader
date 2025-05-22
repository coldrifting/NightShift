// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NightShift/SkyboxDynamic"
{
    Properties
    {
        [NoScaleOffset] _FrontTex ("Front [+Z]",2D)="grey" { }
        [NoScaleOffset] _BackTex ("Back [-Z]",2D)="grey" { }
        [NoScaleOffset] _LeftTex ("Left [+X]",2D)="grey" { }
        [NoScaleOffset] _RightTex ("Right [-X]",2D)="grey" { }
        [NoScaleOffset] _UpTex ("Up [+Y]",2D)="grey" { }
        [NoScaleOffset] _DownTex ("Down [-Y]",2D)="grey" { }

        _Rotation ("_Rotation", Range(0.0, 360.0)) = 0.0
        _TimeOffset ("_TimeOffset", Range(0.0, 720.0)) = 0.0
        _SunSize ("_SunSize", Range(0.1, 5.0)) = 2.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }
        Cull Off
        ZWrite Off

        CGINCLUDE
        #include "UnityCG.cginc"

        float _Rotation;
        float _TimeOffset;
        float _SunSize;

        float3 RotateAroundZXYInDegrees(float3 vertex, float3 degrees)
        {
            // Calculate sine and cosine for each of the three components ...
            float3 alpha = radians(degrees);
            float3 sina, cosa;
            sincos(alpha, sina, cosa);
            // Create a rotation matrix around each axis ...
            float3x3 rx = float3x3(1.0, 0.0, 0.0, 0.0, cosa.x, -sina.x, 0.0, sina.x, cosa.x);
            float3x3 ry = float3x3(cosa.y, 0.0, sina.y, 0.0, 1.0, 0.0, -sina.y, 0.0, cosa.y);
            float3x3 rz = float3x3(cosa.z, -sina.z, 0.0, sina.z, cosa.z, 0.0, 0.0, 0.0, 1.0);
            // Composite rotation in order of Z axis, X axis, Y axis ...
            float3x3 m = mul(rz, mul(rx, ry));
            // apply rotation and return
            return mul(m, vertex);
        }

        float2 rsi(float3 r0, float3 rd, float sr) {
            // ray-sphere intersection that assumes
            // the sphere is centered at the origin.
            // No intersection when result.x > result.y
            float a = dot(rd, rd);
            float b = 2.0 * dot(rd, r0);
            float c = dot(r0, r0) - (sr * sr);
            float d = (b*b) - 4.0*a*c;
            if (d < 0.0) return float2(1e5,-1e5);
            return float2(
                (-b - sqrt(d))/(2.0*a),
                (-b + sqrt(d))/(2.0*a)
            );
        }

        float3 atmosphere(float3 r, float3 pSun) {
            // Constants
            float pi = 3.14159265359;
            float sunIntensity = 26;
            float earthRadius = 6370000;
            float atmoRadius  = 6470000;
            float3 betaR = float3(4e-6, 13e-6, 33e-6); // Rayleigh Scattering Coefficient
            float betaM = 21e-6;                             // Mie Scattering Coefficient
            float Hr = 8000; // Rayleigh Scale Height
            float Hm = 1200; // Mie Scale Height
            float g = 0.75; // Mie preferred scattering direction
            
            float3 rayOrigin = float3(0, earthRadius + 50, 0);

            int iSteps = 16;
            int jSteps = 8; 

            // Normalize the sun and view directions.
            pSun = normalize(pSun);
            r = normalize(r);

            // Calculate the step size of the primary ray.
            float2 p = rsi(rayOrigin, r, atmoRadius);
            if (p.x > p.y) return float3(0,0,0);
            p.y = min(p.y, rsi(rayOrigin, r, earthRadius).x);
            float iStepSize = (p.y - p.x) / iSteps;

            // Initialize the primary ray time.
            float iTime = 0.0;

            // Initialize accumulators for Rayleigh and Mie scattering.
            float3 totalRlh = float3(0,0,0);
            float3 totalMie = float3(0,0,0);

            // Initialize optical depth accumulators for the primary ray.
            float iOdRlh = 0.0;
            float iOdMie = 0.0;

            // Calculate the Rayleigh and Mie phases.
            float mu = dot(r, pSun);
            float mumu = mu * mu;
            float gg = g * g;
            float pRlh = 3.0 / (16.0 * pi) * (1.0 + mumu);
            float pMie = 3.0 / (8.0 * pi) * ((1.0 - gg) * (mumu + 1.0)) / (pow(1.0 + gg - 2.0 * mu * g, 1.5) * (2.0 + gg));

            // Sample the primary ray.
            for (int i = 0; i < iSteps; i++) {

                // Calculate the primary ray sample position.
                float3 iPos = rayOrigin + r * (iTime + iStepSize * 0.5);

                // Calculate the height of the sample.
                float iHeight = length(iPos) - earthRadius;

                // Calculate the optical depth of the Rayleigh and Mie scattering for this step.
                float odStepRlh = exp(-iHeight / Hr) * iStepSize;
                float odStepMie = exp(-iHeight / Hm) * iStepSize;

                // Accumulate optical depth.
                iOdRlh += odStepRlh;
                iOdMie += odStepMie;

                // Calculate the step size of the secondary ray.
                float jStepSize = rsi(iPos, pSun, atmoRadius).y / float(jSteps);

                // Initialize the secondary ray time.
                float jTime = 0.0;

                // Initialize optical depth accumulators for the secondary ray.
                float jOdRlh = 0.0;
                float jOdMie = 0.0;

                // Sample the secondary ray.
                for (int j = 0; j < jSteps; j++) {

                    // Calculate the secondary ray sample position.
                    float3 jPos = iPos + pSun * (jTime + jStepSize * 0.5);

                    // Calculate the height of the sample.
                    float jHeight = length(jPos) - earthRadius;

                    // Accumulate the optical depth.
                    jOdRlh += exp(-jHeight / Hr) * jStepSize;
                    jOdMie += exp(-jHeight / Hm) * jStepSize;

                    // Increment the secondary ray time.
                    jTime += jStepSize;
                }

                // Calculate attenuation.
                float3 attn = exp(-(betaM * (iOdMie + jOdMie) + betaR * (iOdRlh + jOdRlh)));

                // Accumulate scattering.
                totalRlh += odStepRlh * attn;
                totalMie += odStepMie * attn;

                // Increment the primary ray time.
                iTime += iStepSize;
            }

            // Calculate and return the final color.
            return sunIntensity * (pRlh * betaR * totalRlh + pMie * betaM * totalMie);
        }

        float3 mapRange(
            float input,
            float key1, float value1,
            float key2, float value2) {

            if (key2 - key1 < 0.0000001) {
                return value1;
            }

            float mapped = clamp((input - key1) / (key2 - key1), 0, 1);
            return lerp(value1, value2, mapped * mapped);
        }

        float3 mapRange(float input, 
                        float key1, float3 value1, 
                        float key2, float3 value2, 
                        float key3, float3 value3) {
            bool useFirst = input < key2;

            if (key2 - key1 < 0.001) {
                return value2;
            }
            if (key3 - key2 < 0.001) {
                return value3;
            }

            float3 val1 = useFirst ? value1 : value2;
            float3 val2 = useFirst ? value2 : value3;
            float transition = useFirst
                ? clamp((input - key1) / (key2 - key1), 0, 1)
                : clamp((input - key2) / (key3 - key2), 0, 1);

            return lerp(val1, val2, transition);
        }

        float3 mapRange(float input, 
                        float key1, float value1, 
                        float key2, float value2, 
                        float key3, float value3) {
            bool useFirst = input < key2;

            if (key2 - key1 < 0.001) {
                return value2;
            }
            if (key3 - key2 < 0.001) {
                return value3;
            }

            float val1 = useFirst ? value1 : value2;
            float val2 = useFirst ? value2 : value3;
            float transition = useFirst
                ? clamp((input - key1) / (key2 - key1), 0, 1)
                : clamp((input - key2) / (key3 - key2), 0, 1);

            return lerp(val1, val2, transition);
        }

        float3 getSunDir(float sunHeightInDegrees) {
            float alpha = radians(_Rotation + 90);
            float beta = radians(sunHeightInDegrees);

            return normalize(float3(cos(beta)*sin(alpha), sin(beta), cos(beta)*cos(alpha)));
        }

        float4 sunDisc(float3 sunDir, float3 normal, float sunHeight) {

            float3 color = mapRange(sunHeight + 3, 
                0, float3(1, 0.25, 0),
                3, float3(1, 1, 0.5),
                10, float3(1, 1, 1));

            float size = mapRange(sunHeight, 
                2, _SunSize * 1.25,
                4, _SunSize * 1.15,
                6, _SunSize);

            float falloff = mapRange(sunHeight, 
                2, _SunSize * 0.15,
                4, _SunSize * 0.25,
                6, _SunSize * 0.5);

            // Fixes a small bug with a tiny black dot in the middle of the sun
            float dotFixer = 0.000001;
            float diffAngle = degrees(acos(dot(sunDir, normal) / (length(sunDir) * length(normal)) - dotFixer ));
            if (diffAngle <= size) {
                return float4(color, 1);
            }
            else if (diffAngle <= size + falloff) {
                float finalFalloff = clamp((diffAngle - size) / falloff, 0, 1);
                float alpha = smoothstep(1, 0, finalFalloff);
                return float4(color, alpha);
            }
            return float4(0,0,0,0);
        }

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
            //UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        struct v2f
        {
            float4 vertex : SV_POSITION0;
            float3 worldPos : TEXCOORD0;
            float2 texcoord : TEXCOORD1;
            float timeOffset: PSIZE0;
            float sunHeight: PSIZE1;
            //UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            //UNITY_SETUP_INSTANCE_ID(v);
            //UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.timeOffset = _TimeOffset % 360;
            o.sunHeight = ((o.timeOffset + 90) % 360) - 90;
            if (o.timeOffset > 90 && o.timeOffset < 270) {
                o.sunHeight = 180 - o.timeOffset;
            }

            // Rotate skybox background
            float3 rotated = RotateAroundZXYInDegrees(v.vertex, float3(_Rotation + 90, o.timeOffset, 90));
            o.vertex = UnityObjectToClipPos(rotated);

            o.worldPos = mul (unity_ObjectToWorld, rotated);
            o.worldPos = RotateAroundZXYInDegrees(o.worldPos, float3(0,0,0));
            o.texcoord = v.texcoord;

            return o;
        }

        float3 tonemap(float3 layer) {
            return 1.0 - exp(-1.0 * layer);
        }

        float3 overlay(float3 base, float4 overlay) {
            return clamp((overlay * overlay.a) + ((1 - overlay.a) * base), 0, 1);
        }

        float3 skybox_frag(v2f i, sampler2D smp, bool flip)
        {
            float2 texcoord = i.texcoord;
            if (flip)
            {
                texcoord = float2(i.texcoord.x * -1, i.texcoord.y * -1);
            }
            float3 stars = tex2D(smp, texcoord);
            stars = stars * stars;

            float starsDaylightMask = 1 - clamp((i.sunHeight + 5) / 10, 0, 1);
            float starsFadeMask = 1 - clamp((i.sunHeight + 20) / 20, 0, 1);
            float3 starsWithMask = stars * lerp(starsFadeMask * starsFadeMask, 1 * stars, stars) * starsDaylightMask;

            float3 sunDir = getSunDir(i.timeOffset);
            float3 atmo = tonemap(atmosphere(normalize(i.worldPos), sunDir));
            float4 sun = sunDisc(sunDir, normalize(i.worldPos), i.sunHeight);

            float3 atmoWithStars = starsWithMask + atmo - starsWithMask * atmo;

            float horizonMask = mapRange(normalize(i.worldPos).y, -0.03, 1, -0.01, 0);

            float4 horizon = horizonMask * unity_FogColor.rgba;

            return overlay(overlay(atmoWithStars, sun) * (1 - horizonMask), horizon);
        }
        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _FrontTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _FrontTex, false);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _BackTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _BackTex, false);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _LeftTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _LeftTex, false);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _RightTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _RightTex, false);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _UpTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _UpTex, true);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            sampler2D _DownTex;

            float3 frag(v2f i) : SV_Target
            {
                return skybox_frag(i, _DownTex, false);
            }
            ENDCG
        }
    }
}