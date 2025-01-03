//�����ɫ����
struct PixelColorInfo
{
    //��ɫ
    int4 color;
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

#define BlockSizeX 64
#define BlockSizeY 64

const float PI = 3.14159265358979323846;