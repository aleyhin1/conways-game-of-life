Shader "Unlit/GridPresenter"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<uint> _GridBuffer;
            uint2 _Resolution;
            StructuredBuffer<float4> _Colors;

            uint2 GetPixelCoords(float2 uv)
            {
                uint2 pixelCoords;
                pixelCoords.x = (uint)floor(uv.x * _Resolution.x);
                pixelCoords.y = (uint)floor(uv.y * _Resolution.y);
                return pixelCoords;
            }

            uint GetFlattenedCoords(uint2 pixelCoords)
            {
                pixelCoords.x = min(pixelCoords.x, (uint)(_Resolution.x - 1));
                pixelCoords.y = min(pixelCoords.y, (uint)(_Resolution.y - 1));

                return pixelCoords.x + pixelCoords.y * _Resolution.x;
            }

            float4 GetColor(uint type)
            {
                return _Colors[type];
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                uint2 pixelCoords = GetPixelCoords(i.uv);
                uint flattenedCoords = GetFlattenedCoords(pixelCoords);
                uint type = _GridBuffer[flattenedCoords];
                return GetColor(type);
            }
            ENDCG
        }
    }
}
