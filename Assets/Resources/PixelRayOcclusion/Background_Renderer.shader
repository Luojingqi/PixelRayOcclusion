Shader "PixelRayOcclusion/Background_Renderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
            
        ZWrite Off // 禁用深度写入
        ZTest LEqual // 保持深度测试
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "Queue" = "Geometry" } 
        Pass
        {
            CGPROGRAM

            #pragma vertex vert  // 指定顶点着色器
            #pragma fragment frag  // 指定片段着色器

            #pragma target 5.0
            #include "Background_Buffer.hlsl"


            sampler2D _MainTex;
            // 顶点输入数据结构
            struct appdata
            {
                float4 vertex : POSITION;  // 顶点坐标
                float2 uv : TEXCOORD0;     // UV 坐标
            };

            // 传递给片段着色器的数据结构
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;  // 转换到屏幕坐标
            };

            // 顶点着色器
            v2f vert (appdata data)
            {
                v2f i;
                i.pos = UnityObjectToClipPos(data.vertex);  // 将对象空间坐标转换为剪裁空间
                i.uv = data.uv;
                return i;
            }

            // 片段着色器
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
