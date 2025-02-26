//点的颜色数据
struct PixelColorInfo
{
    //颜色
    int4 color;
    //光路混色
    float lightPathColorMixing;
    //光强影响的系数
    float affectsLightIntensity;
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

struct BlockPixelInfo
{
    //使用的颜色id
    int colorInfoId;
    //耐久度
    float durability;
};

#define BlockSizeX 64
#define BlockSizeY 64

const float PI = 3.14159265358979323846;