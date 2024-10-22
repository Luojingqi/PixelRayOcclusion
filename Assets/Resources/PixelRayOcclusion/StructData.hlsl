//�����ɫ����
struct PixelColorInfo
{
    //��ɫ
    int4 color;
    //�Ƿ�Ӱ�����
    bool affectsLight;
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

struct SelfLuminous
{
    //��������
    int2 pos;
    float strength;
};

#define BlockSizeX 64
#define BlockSizeY 64

const float PI = 3.14159265358979323846;