//点的颜色数据
struct PixelColorInfo
{
    //颜色
    float4 color;
    //反光度
    float shininess;
};

//太阳
struct Sun
{   
    //全局坐标
    int2 gloabPos;
    //半径
    int r;
    //光的颜色
    float4 color;
};