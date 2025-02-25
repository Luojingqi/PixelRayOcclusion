Shader "PixelRayOcclusion/Background_Renderer"
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
        Tags { "Queue" = "Geometry" } 
        Pass
        {
            CGPROGRAM

            #pragma vertex vert  // ָ��������ɫ��
            #pragma fragment frag  // ָ��Ƭ����ɫ��

            #pragma target 5.0
            #include "Background_Buffer.hlsl"


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
                PixelColorInfo pixel = GetPixel(pixelPos);
                float3 pixelColor = pow(pixel.color.xyz / 255.0, 2.2);
                float3 lightColor = pow(LightResultBuffer[PixelToIndex(pixelPos)].xyz / 255.0 , 1.6);
                return float4(lightColor * pixelColor.xyz + pixelColor.xyz * pixel.selfLuminous , pixel.color.w / 255.0);
                //return float4(lightColor,1);
            }

            ENDCG
        }
    }
}
