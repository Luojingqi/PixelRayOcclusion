Shader "PixelRayOcclusion/Block_Renderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off // �������д��
        ZTest LEqual // ������Ȳ���
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "Queue" = "Geometry+100" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert  // ָ��������ɫ��
            #pragma fragment frag  // ָ��Ƭ����ɫ��

            #pragma target 5.0
            #include "Block_Buffer.hlsl"

            sampler2D _MainTex;
            // �����������ݽṹ
            struct appdata
            {
                float4 vertex : POSITION;  // ��������
                float2 uv : TEXCOORD0;     // UV ����
            };

            // ���ݸ�Ƭ����ɫ�������ݽṹ
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;  // ת������Ļ����
            };

            // ������ɫ��
            v2f vert (appdata data)
            {
                v2f i;
                i.pos = UnityObjectToClipPos(data.vertex);  // ������ռ�����ת��Ϊ���ÿռ�
                i.uv = data.uv;      
                return i;
            }

            // Ƭ����ɫ��
            fixed4 frag (v2f i) : SV_Target
            {
                int2 pixelPos = UVToPixel(i.uv);
                int index = PixelToIndex(pixelPos);
                BlockPixelInfo pixelInfo = GetBlockPixelInfo(index);
                PixelColorInfo colorInfo = GetPixelColorInfo(pixelInfo.colorInfoId);
                
                float3 pixelColor = pow(colorInfo.color.xyz / 255.0, 2.2) * pow(pixelInfo.durability , 0.75);
                float3 lightColor = pow(LightResultBuffer[PixelToIndex(pixelPos)].xyz / 255.0, 2.2);
                return float4(lightColor.xyz *  pixelColor.xyz + pixelColor.xyz * (colorInfo.selfLuminous + 0.0125)  , colorInfo.color.w / 255.0 * pixelInfo.affectsTransparency);
            }

            ENDCG
        }
    }
}
