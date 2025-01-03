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

#define Line150 151
int GetLine_150(int2 pos_G0, int2 pos_G1, out int2 points[Line150])
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
for (int i = 0; i < Line150; i++)
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
if (Index >= Line150)
Index = -1;
return Index;
}

#define Line160 161
int GetLine_160(int2 pos_G0, int2 pos_G1, out int2 points[Line160])
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
for (int i = 0; i < Line160; i++)
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
if (Index >= Line160)
Index = -1;
return Index;
}

#define Line170 171
int GetLine_170(int2 pos_G0, int2 pos_G1, out int2 points[Line170])
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
for (int i = 0; i < Line170; i++)
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
if (Index >= Line170)
Index = -1;
return Index;
}

#define Line180 181
int GetLine_180(int2 pos_G0, int2 pos_G1, out int2 points[Line180])
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
for (int i = 0; i < Line180; i++)
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
if (Index >= Line180)
Index = -1;
return Index;
}

#define Line190 191
int GetLine_190(int2 pos_G0, int2 pos_G1, out int2 points[Line190])
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
for (int i = 0; i < Line190; i++)
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
if (Index >= Line190)
Index = -1;
return Index;
}

