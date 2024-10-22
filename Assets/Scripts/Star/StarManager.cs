using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public void Awake()
    {
        Inst = this;
    }
    public static StarManager Inst;
    public List<Star> allStar = new List<Star>();

    public void CreateStar(Vector2Int GloabPos, int r, int zeroGR, string name, float g = 10)
    {
        Star star = new Star(GloabPos, r, zeroGR, name, g);
        if (star == null) return;
        allStar.Add(star);
    }

    public Vector3 GetAcceleration(Vector2Int gloabPos)
    {
        Vector3 ret = new Vector3();
        for (int i = 0; i < allStar.Count; i++)
        {
            ret += allStar[i].GetAcceleration(gloabPos);
        }
        return ret;
    }
}
