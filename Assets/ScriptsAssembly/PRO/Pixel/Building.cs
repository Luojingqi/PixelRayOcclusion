using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Building
    {
        public HashSet<Pixel> Pixels;
        public Building_Disk Disk;

        public Vector2Int global;
        public BoxCollider2D collider;
    }
}
