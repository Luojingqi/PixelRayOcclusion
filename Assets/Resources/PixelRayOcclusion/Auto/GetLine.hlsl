#define Line5 6
int GetLine_5(int2 pos_G0, int2 pos_G1, out int2 points[Line5])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
for (int i = 0; i < Line5; i++)
{
points[i] = nowPoint;
int e2 = 2 * err;
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
if (index >= Line5)
index = -1;
return index;
}

#define Line15 16
int GetLine_15(int2 pos_G0, int2 pos_G1, out int2 points[Line15])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
for (int i = 0; i < Line15; i++)
{
points[i] = nowPoint;
int e2 = 2 * err;
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
if (index >= Line15)
index = -1;
return index;
}

#define Line35 36
int GetLine_35(int2 pos_G0, int2 pos_G1, out int2 points[Line35])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
for (int i = 0; i < Line35; i++)
{
points[i] = nowPoint;
int e2 = 2 * err;
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
if (index >= Line35)
index = -1;
return index;
}

#define Line50 51
int GetLine_50(int2 pos_G0, int2 pos_G1, out int2 points[Line50])
{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;
for (int i = 0; i < Line50; i++)
{
points[i] = nowPoint;
int e2 = 2 * err;
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
if (index >= Line50)
index = -1;
return index;
}

