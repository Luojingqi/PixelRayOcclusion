//�����ɫ����
struct PixelColorInfo
{
    //��ɫ
    int4 color;
    //��·��ɫ
    float lightPathColorMixing;
    //��ǿӰ���ϵ��
    float affectsLightIntensity;
    //�Է���ǿ��
    float selfLuminous;
};

struct LightSource
{
    //ȫ������
    int2 gloabPos;
    //�����ɫ
    int3 color;
};

struct BlockPixelInfo
{
    //ʹ�õ���ɫid
    int colorInfoId;
    //�;ö�
    float durability;
};

#define BlockSizeX 64
#define BlockSizeY 64

const float PI = 3.14159265358979323846;