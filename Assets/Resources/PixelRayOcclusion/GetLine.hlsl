#include "Buffer_LightBuffer.hlsl"

#define Line50 50
int GetLine_50(int2 pos_G0, int2 pos_G1, out int2 points[Line50])
{
    int lineMax = Line50;
    
    int dx = abs(pos_G1.x - pos_G0.x);
    int dy = abs(pos_G1.y - pos_G0.y);

    int sx = pos_G0.x < pos_G1.x ? 1 : -1;
    int sy = pos_G0.y < pos_G1.y ? 1 : -1;
    int err = dx - dy;
    int pointCount = (dx > dy) ? dx + 1 : dy + 1;
    int i = 0;
    int retIndex = -1;
    while (true)
    {
        if (Equalsi2(pos_G0, pos_G1))
            retIndex = i;
        points[i++] = pos_G0;
        if (i >= lineMax)
            break;
        int e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            pos_G0.x += sx;
        }
        if (e2 < dx)
        {
            err += dx;
            pos_G0.y += sy;
        }
    }
    return retIndex;
}



