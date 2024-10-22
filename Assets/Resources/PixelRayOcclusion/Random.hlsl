int a = 11035;
int c = 12345;
int randomMax = 100000000;
int seed = 1;
int NextInt(int min, int max)
{
    seed = (a * seed + c) % randomMax;
    int range = max - min + 1;
    return seed % range + min;
}
float NextFloat(float min, float max)
{
    seed = (a * seed + c) % randomMax;
    float range = max - min;
    return range * (float) seed / randomMax + min;
}
