//点的颜色数据
struct PixelColorInfo
{
    //颜色
    int4 color;
    //是否影响光照
    bool affectsLight;
    //自发光强度
    float selfLuminous;
};

struct LightSource
{   
    //全局坐标
    int2 gloabPos;
    //光的颜色
    int3 color;
};

struct SelfLuminous
{
    //块内坐标
    int2 pos;
    float strength;
};

#define BlockSizeX 64
#define BlockSizeY 64

const float PI = 3.14159265358979323846;