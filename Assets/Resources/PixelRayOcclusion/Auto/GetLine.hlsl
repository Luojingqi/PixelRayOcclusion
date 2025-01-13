#define Line1 2
int GetLine_1(int2 pos_G0, int2 pos_G1, out int2 points[Line1])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line1; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line1)
Index = -1;
return Index;
}

#define Line2 3
int GetLine_2(int2 pos_G0, int2 pos_G1, out int2 points[Line2])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line2; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line2)
Index = -1;
return Index;
}

#define Line3 4
int GetLine_3(int2 pos_G0, int2 pos_G1, out int2 points[Line3])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line3; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line3)
Index = -1;
return Index;
}

#define Line4 5
int GetLine_4(int2 pos_G0, int2 pos_G1, out int2 points[Line4])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line4; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line4)
Index = -1;
return Index;
}

#define Line5 6
int GetLine_5(int2 pos_G0, int2 pos_G1, out int2 points[Line5])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line5; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line5)
Index = -1;
return Index;
}

#define Line6 7
int GetLine_6(int2 pos_G0, int2 pos_G1, out int2 points[Line6])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line6; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line6)
Index = -1;
return Index;
}

#define Line7 8
int GetLine_7(int2 pos_G0, int2 pos_G1, out int2 points[Line7])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line7; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line7)
Index = -1;
return Index;
}

#define Line8 9
int GetLine_8(int2 pos_G0, int2 pos_G1, out int2 points[Line8])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line8; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line8)
Index = -1;
return Index;
}

#define Line9 10
int GetLine_9(int2 pos_G0, int2 pos_G1, out int2 points[Line9])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line9; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line9)
Index = -1;
return Index;
}

#define Line10 11
int GetLine_10(int2 pos_G0, int2 pos_G1, out int2 points[Line10])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line10; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line10)
Index = -1;
return Index;
}

#define Line11 12
int GetLine_11(int2 pos_G0, int2 pos_G1, out int2 points[Line11])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line11; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line11)
Index = -1;
return Index;
}

#define Line12 13
int GetLine_12(int2 pos_G0, int2 pos_G1, out int2 points[Line12])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line12; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line12)
Index = -1;
return Index;
}

#define Line13 14
int GetLine_13(int2 pos_G0, int2 pos_G1, out int2 points[Line13])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line13; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line13)
Index = -1;
return Index;
}

#define Line14 15
int GetLine_14(int2 pos_G0, int2 pos_G1, out int2 points[Line14])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line14; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line14)
Index = -1;
return Index;
}

#define Line15 16
int GetLine_15(int2 pos_G0, int2 pos_G1, out int2 points[Line15])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line15; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line15)
Index = -1;
return Index;
}

#define Line16 17
int GetLine_16(int2 pos_G0, int2 pos_G1, out int2 points[Line16])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line16; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line16)
Index = -1;
return Index;
}

#define Line17 18
int GetLine_17(int2 pos_G0, int2 pos_G1, out int2 points[Line17])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line17; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line17)
Index = -1;
return Index;
}

#define Line18 19
int GetLine_18(int2 pos_G0, int2 pos_G1, out int2 points[Line18])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line18; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line18)
Index = -1;
return Index;
}

#define Line19 20
int GetLine_19(int2 pos_G0, int2 pos_G1, out int2 points[Line19])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line19; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line19)
Index = -1;
return Index;
}

#define Line20 21
int GetLine_20(int2 pos_G0, int2 pos_G1, out int2 points[Line20])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line20; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line20)
Index = -1;
return Index;
}

#define Line21 22
int GetLine_21(int2 pos_G0, int2 pos_G1, out int2 points[Line21])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line21; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line21)
Index = -1;
return Index;
}

#define Line22 23
int GetLine_22(int2 pos_G0, int2 pos_G1, out int2 points[Line22])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line22; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line22)
Index = -1;
return Index;
}

#define Line23 24
int GetLine_23(int2 pos_G0, int2 pos_G1, out int2 points[Line23])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line23; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line23)
Index = -1;
return Index;
}

#define Line24 25
int GetLine_24(int2 pos_G0, int2 pos_G1, out int2 points[Line24])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line24; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line24)
Index = -1;
return Index;
}

#define Line25 26
int GetLine_25(int2 pos_G0, int2 pos_G1, out int2 points[Line25])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line25; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line25)
Index = -1;
return Index;
}

#define Line26 27
int GetLine_26(int2 pos_G0, int2 pos_G1, out int2 points[Line26])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line26; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line26)
Index = -1;
return Index;
}

#define Line27 28
int GetLine_27(int2 pos_G0, int2 pos_G1, out int2 points[Line27])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line27; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line27)
Index = -1;
return Index;
}

#define Line28 29
int GetLine_28(int2 pos_G0, int2 pos_G1, out int2 points[Line28])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line28; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line28)
Index = -1;
return Index;
}

#define Line29 30
int GetLine_29(int2 pos_G0, int2 pos_G1, out int2 points[Line29])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line29; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line29)
Index = -1;
return Index;
}

#define Line30 31
int GetLine_30(int2 pos_G0, int2 pos_G1, out int2 points[Line30])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line30; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line30)
Index = -1;
return Index;
}

#define Line31 32
int GetLine_31(int2 pos_G0, int2 pos_G1, out int2 points[Line31])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line31; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line31)
Index = -1;
return Index;
}

#define Line32 33
int GetLine_32(int2 pos_G0, int2 pos_G1, out int2 points[Line32])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line32; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line32)
Index = -1;
return Index;
}

#define Line33 34
int GetLine_33(int2 pos_G0, int2 pos_G1, out int2 points[Line33])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line33; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line33)
Index = -1;
return Index;
}

#define Line34 35
int GetLine_34(int2 pos_G0, int2 pos_G1, out int2 points[Line34])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line34; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line34)
Index = -1;
return Index;
}

#define Line35 36
int GetLine_35(int2 pos_G0, int2 pos_G1, out int2 points[Line35])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line35; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line35)
Index = -1;
return Index;
}

#define Line36 37
int GetLine_36(int2 pos_G0, int2 pos_G1, out int2 points[Line36])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line36; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line36)
Index = -1;
return Index;
}

#define Line37 38
int GetLine_37(int2 pos_G0, int2 pos_G1, out int2 points[Line37])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line37; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line37)
Index = -1;
return Index;
}

#define Line38 39
int GetLine_38(int2 pos_G0, int2 pos_G1, out int2 points[Line38])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line38; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line38)
Index = -1;
return Index;
}

#define Line39 40
int GetLine_39(int2 pos_G0, int2 pos_G1, out int2 points[Line39])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line39; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line39)
Index = -1;
return Index;
}

#define Line40 41
int GetLine_40(int2 pos_G0, int2 pos_G1, out int2 points[Line40])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line40; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line40)
Index = -1;
return Index;
}

#define Line41 42
int GetLine_41(int2 pos_G0, int2 pos_G1, out int2 points[Line41])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line41; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line41)
Index = -1;
return Index;
}

#define Line42 43
int GetLine_42(int2 pos_G0, int2 pos_G1, out int2 points[Line42])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line42; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line42)
Index = -1;
return Index;
}

#define Line43 44
int GetLine_43(int2 pos_G0, int2 pos_G1, out int2 points[Line43])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line43; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line43)
Index = -1;
return Index;
}

#define Line44 45
int GetLine_44(int2 pos_G0, int2 pos_G1, out int2 points[Line44])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line44; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line44)
Index = -1;
return Index;
}

#define Line45 46
int GetLine_45(int2 pos_G0, int2 pos_G1, out int2 points[Line45])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line45; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line45)
Index = -1;
return Index;
}

#define Line46 47
int GetLine_46(int2 pos_G0, int2 pos_G1, out int2 points[Line46])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line46; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line46)
Index = -1;
return Index;
}

#define Line47 48
int GetLine_47(int2 pos_G0, int2 pos_G1, out int2 points[Line47])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line47; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line47)
Index = -1;
return Index;
}

#define Line48 49
int GetLine_48(int2 pos_G0, int2 pos_G1, out int2 points[Line48])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line48; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line48)
Index = -1;
return Index;
}

#define Line49 50
int GetLine_49(int2 pos_G0, int2 pos_G1, out int2 points[Line49])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line49; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line49)
Index = -1;
return Index;
}

#define Line50 51
int GetLine_50(int2 pos_G0, int2 pos_G1, out int2 points[Line50])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line50; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line50)
Index = -1;
return Index;
}

#define Line51 52
int GetLine_51(int2 pos_G0, int2 pos_G1, out int2 points[Line51])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line51; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line51)
Index = -1;
return Index;
}

#define Line52 53
int GetLine_52(int2 pos_G0, int2 pos_G1, out int2 points[Line52])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line52; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line52)
Index = -1;
return Index;
}

#define Line53 54
int GetLine_53(int2 pos_G0, int2 pos_G1, out int2 points[Line53])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line53; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line53)
Index = -1;
return Index;
}

#define Line54 55
int GetLine_54(int2 pos_G0, int2 pos_G1, out int2 points[Line54])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line54; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line54)
Index = -1;
return Index;
}

#define Line55 56
int GetLine_55(int2 pos_G0, int2 pos_G1, out int2 points[Line55])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line55; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line55)
Index = -1;
return Index;
}

#define Line56 57
int GetLine_56(int2 pos_G0, int2 pos_G1, out int2 points[Line56])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line56; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line56)
Index = -1;
return Index;
}

#define Line57 58
int GetLine_57(int2 pos_G0, int2 pos_G1, out int2 points[Line57])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line57; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line57)
Index = -1;
return Index;
}

#define Line58 59
int GetLine_58(int2 pos_G0, int2 pos_G1, out int2 points[Line58])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line58; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line58)
Index = -1;
return Index;
}

#define Line59 60
int GetLine_59(int2 pos_G0, int2 pos_G1, out int2 points[Line59])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line59; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line59)
Index = -1;
return Index;
}

#define Line60 61
int GetLine_60(int2 pos_G0, int2 pos_G1, out int2 points[Line60])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line60; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line60)
Index = -1;
return Index;
}

#define Line61 62
int GetLine_61(int2 pos_G0, int2 pos_G1, out int2 points[Line61])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line61; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line61)
Index = -1;
return Index;
}

#define Line62 63
int GetLine_62(int2 pos_G0, int2 pos_G1, out int2 points[Line62])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line62; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line62)
Index = -1;
return Index;
}

#define Line63 64
int GetLine_63(int2 pos_G0, int2 pos_G1, out int2 points[Line63])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line63; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line63)
Index = -1;
return Index;
}

#define Line64 65
int GetLine_64(int2 pos_G0, int2 pos_G1, out int2 points[Line64])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line64; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line64)
Index = -1;
return Index;
}

#define Line65 66
int GetLine_65(int2 pos_G0, int2 pos_G1, out int2 points[Line65])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line65; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line65)
Index = -1;
return Index;
}

#define Line66 67
int GetLine_66(int2 pos_G0, int2 pos_G1, out int2 points[Line66])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line66; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line66)
Index = -1;
return Index;
}

#define Line67 68
int GetLine_67(int2 pos_G0, int2 pos_G1, out int2 points[Line67])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line67; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line67)
Index = -1;
return Index;
}

#define Line68 69
int GetLine_68(int2 pos_G0, int2 pos_G1, out int2 points[Line68])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line68; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line68)
Index = -1;
return Index;
}

#define Line69 70
int GetLine_69(int2 pos_G0, int2 pos_G1, out int2 points[Line69])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line69; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line69)
Index = -1;
return Index;
}

#define Line70 71
int GetLine_70(int2 pos_G0, int2 pos_G1, out int2 points[Line70])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line70; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line70)
Index = -1;
return Index;
}

#define Line71 72
int GetLine_71(int2 pos_G0, int2 pos_G1, out int2 points[Line71])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line71; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line71)
Index = -1;
return Index;
}

#define Line72 73
int GetLine_72(int2 pos_G0, int2 pos_G1, out int2 points[Line72])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line72; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line72)
Index = -1;
return Index;
}

#define Line73 74
int GetLine_73(int2 pos_G0, int2 pos_G1, out int2 points[Line73])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line73; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line73)
Index = -1;
return Index;
}

#define Line74 75
int GetLine_74(int2 pos_G0, int2 pos_G1, out int2 points[Line74])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line74; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line74)
Index = -1;
return Index;
}

#define Line75 76
int GetLine_75(int2 pos_G0, int2 pos_G1, out int2 points[Line75])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line75; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line75)
Index = -1;
return Index;
}

#define Line76 77
int GetLine_76(int2 pos_G0, int2 pos_G1, out int2 points[Line76])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line76; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line76)
Index = -1;
return Index;
}

#define Line77 78
int GetLine_77(int2 pos_G0, int2 pos_G1, out int2 points[Line77])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line77; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line77)
Index = -1;
return Index;
}

#define Line78 79
int GetLine_78(int2 pos_G0, int2 pos_G1, out int2 points[Line78])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line78; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line78)
Index = -1;
return Index;
}

#define Line79 80
int GetLine_79(int2 pos_G0, int2 pos_G1, out int2 points[Line79])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line79; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line79)
Index = -1;
return Index;
}

#define Line80 81
int GetLine_80(int2 pos_G0, int2 pos_G1, out int2 points[Line80])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line80; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line80)
Index = -1;
return Index;
}

#define Line81 82
int GetLine_81(int2 pos_G0, int2 pos_G1, out int2 points[Line81])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line81; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line81)
Index = -1;
return Index;
}

#define Line82 83
int GetLine_82(int2 pos_G0, int2 pos_G1, out int2 points[Line82])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line82; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line82)
Index = -1;
return Index;
}

#define Line83 84
int GetLine_83(int2 pos_G0, int2 pos_G1, out int2 points[Line83])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line83; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line83)
Index = -1;
return Index;
}

#define Line84 85
int GetLine_84(int2 pos_G0, int2 pos_G1, out int2 points[Line84])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line84; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line84)
Index = -1;
return Index;
}

#define Line85 86
int GetLine_85(int2 pos_G0, int2 pos_G1, out int2 points[Line85])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line85; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line85)
Index = -1;
return Index;
}

#define Line86 87
int GetLine_86(int2 pos_G0, int2 pos_G1, out int2 points[Line86])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line86; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line86)
Index = -1;
return Index;
}

#define Line87 88
int GetLine_87(int2 pos_G0, int2 pos_G1, out int2 points[Line87])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line87; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line87)
Index = -1;
return Index;
}

#define Line88 89
int GetLine_88(int2 pos_G0, int2 pos_G1, out int2 points[Line88])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line88; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line88)
Index = -1;
return Index;
}

#define Line89 90
int GetLine_89(int2 pos_G0, int2 pos_G1, out int2 points[Line89])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line89; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line89)
Index = -1;
return Index;
}

#define Line90 91
int GetLine_90(int2 pos_G0, int2 pos_G1, out int2 points[Line90])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line90; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line90)
Index = -1;
return Index;
}

#define Line91 92
int GetLine_91(int2 pos_G0, int2 pos_G1, out int2 points[Line91])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line91; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line91)
Index = -1;
return Index;
}

#define Line92 93
int GetLine_92(int2 pos_G0, int2 pos_G1, out int2 points[Line92])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line92; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line92)
Index = -1;
return Index;
}

#define Line93 94
int GetLine_93(int2 pos_G0, int2 pos_G1, out int2 points[Line93])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line93; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line93)
Index = -1;
return Index;
}

#define Line94 95
int GetLine_94(int2 pos_G0, int2 pos_G1, out int2 points[Line94])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line94; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line94)
Index = -1;
return Index;
}

#define Line95 96
int GetLine_95(int2 pos_G0, int2 pos_G1, out int2 points[Line95])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line95; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line95)
Index = -1;
return Index;
}

#define Line96 97
int GetLine_96(int2 pos_G0, int2 pos_G1, out int2 points[Line96])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line96; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line96)
Index = -1;
return Index;
}

#define Line97 98
int GetLine_97(int2 pos_G0, int2 pos_G1, out int2 points[Line97])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line97; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line97)
Index = -1;
return Index;
}

#define Line98 99
int GetLine_98(int2 pos_G0, int2 pos_G1, out int2 points[Line98])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line98; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line98)
Index = -1;
return Index;
}

#define Line99 100
int GetLine_99(int2 pos_G0, int2 pos_G1, out int2 points[Line99])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line99; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line99)
Index = -1;
return Index;
}

#define Line100 101
int GetLine_100(int2 pos_G0, int2 pos_G1, out int2 points[Line100])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line100; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line100)
Index = -1;
return Index;
}

#define Line101 102
int GetLine_101(int2 pos_G0, int2 pos_G1, out int2 points[Line101])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line101; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line101)
Index = -1;
return Index;
}

#define Line102 103
int GetLine_102(int2 pos_G0, int2 pos_G1, out int2 points[Line102])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line102; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line102)
Index = -1;
return Index;
}

#define Line103 104
int GetLine_103(int2 pos_G0, int2 pos_G1, out int2 points[Line103])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line103; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line103)
Index = -1;
return Index;
}

#define Line104 105
int GetLine_104(int2 pos_G0, int2 pos_G1, out int2 points[Line104])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line104; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line104)
Index = -1;
return Index;
}

#define Line105 106
int GetLine_105(int2 pos_G0, int2 pos_G1, out int2 points[Line105])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line105; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line105)
Index = -1;
return Index;
}

#define Line106 107
int GetLine_106(int2 pos_G0, int2 pos_G1, out int2 points[Line106])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line106; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line106)
Index = -1;
return Index;
}

#define Line107 108
int GetLine_107(int2 pos_G0, int2 pos_G1, out int2 points[Line107])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line107; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line107)
Index = -1;
return Index;
}

#define Line108 109
int GetLine_108(int2 pos_G0, int2 pos_G1, out int2 points[Line108])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line108; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line108)
Index = -1;
return Index;
}

#define Line109 110
int GetLine_109(int2 pos_G0, int2 pos_G1, out int2 points[Line109])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line109; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line109)
Index = -1;
return Index;
}

#define Line110 111
int GetLine_110(int2 pos_G0, int2 pos_G1, out int2 points[Line110])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line110; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line110)
Index = -1;
return Index;
}

#define Line111 112
int GetLine_111(int2 pos_G0, int2 pos_G1, out int2 points[Line111])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line111; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line111)
Index = -1;
return Index;
}

#define Line112 113
int GetLine_112(int2 pos_G0, int2 pos_G1, out int2 points[Line112])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line112; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line112)
Index = -1;
return Index;
}

#define Line113 114
int GetLine_113(int2 pos_G0, int2 pos_G1, out int2 points[Line113])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line113; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line113)
Index = -1;
return Index;
}

#define Line114 115
int GetLine_114(int2 pos_G0, int2 pos_G1, out int2 points[Line114])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line114; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line114)
Index = -1;
return Index;
}

#define Line115 116
int GetLine_115(int2 pos_G0, int2 pos_G1, out int2 points[Line115])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line115; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line115)
Index = -1;
return Index;
}

#define Line116 117
int GetLine_116(int2 pos_G0, int2 pos_G1, out int2 points[Line116])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line116; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line116)
Index = -1;
return Index;
}

#define Line117 118
int GetLine_117(int2 pos_G0, int2 pos_G1, out int2 points[Line117])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line117; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line117)
Index = -1;
return Index;
}

#define Line118 119
int GetLine_118(int2 pos_G0, int2 pos_G1, out int2 points[Line118])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line118; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line118)
Index = -1;
return Index;
}

#define Line119 120
int GetLine_119(int2 pos_G0, int2 pos_G1, out int2 points[Line119])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line119; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line119)
Index = -1;
return Index;
}

#define Line120 121
int GetLine_120(int2 pos_G0, int2 pos_G1, out int2 points[Line120])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line120; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line120)
Index = -1;
return Index;
}

#define Line121 122
int GetLine_121(int2 pos_G0, int2 pos_G1, out int2 points[Line121])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line121; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line121)
Index = -1;
return Index;
}

#define Line122 123
int GetLine_122(int2 pos_G0, int2 pos_G1, out int2 points[Line122])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line122; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line122)
Index = -1;
return Index;
}

#define Line123 124
int GetLine_123(int2 pos_G0, int2 pos_G1, out int2 points[Line123])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line123; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line123)
Index = -1;
return Index;
}

#define Line124 125
int GetLine_124(int2 pos_G0, int2 pos_G1, out int2 points[Line124])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line124; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line124)
Index = -1;
return Index;
}

#define Line125 126
int GetLine_125(int2 pos_G0, int2 pos_G1, out int2 points[Line125])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line125; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line125)
Index = -1;
return Index;
}

#define Line126 127
int GetLine_126(int2 pos_G0, int2 pos_G1, out int2 points[Line126])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line126; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line126)
Index = -1;
return Index;
}

#define Line127 128
int GetLine_127(int2 pos_G0, int2 pos_G1, out int2 points[Line127])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line127; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line127)
Index = -1;
return Index;
}

#define Line128 129
int GetLine_128(int2 pos_G0, int2 pos_G1, out int2 points[Line128])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
[unroll]
for (int i = 0; i < Line128; i++)
{
points[i] = nowPoint;
e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}
if (Index >= Line128)
Index = -1;
return Index;
}

