Shader "Sprites/StoneEffect"
{
    // Public properties
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _SecondTex("Second Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _StoneProgress ("Stone Progress", Range(0, 1)) = 0 
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float4 _Color;
            float _StoneProgress;

            // Added: To generate per-pixel randomness in a 32x32 grid
            float pixelRandom(float2 uv)
            {
                float2 pixelPos = floor(uv * 16);
                return frac(sin(dot(pixelPos, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseTexture = tex2D(_MainTex, i.uv);
                fixed4 secondTexture = tex2D(_SecondTex, i.uv);

                // Generate a noise mask
                float noise = pixelRandom(i.uv);
                float blendFactor = step(noise, _StoneProgress);

                return lerp(baseTexture, secondTexture, blendFactor);
                //return baseTexture;
            }
            ENDCG
        }
    }
}