using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public abstract class AttackTestSlice2DBase_Disk : Slice_DiskBase
    {
        public Vector2 position;
        public int layerMask;

        public AllowLogicChangeValue_AttackTestSlice2DBase_Disk changeValue;

        private static Queue<RaycastHit2D[]> pool = new Queue<RaycastHit2D[]>();

        protected static RaycastHit2D[] TakeOut()
        {
            if (pool.Count > 0) return pool.Dequeue();
            else return new RaycastHit2D[256];
        }
        protected static void PutIn(RaycastHit2D[] array, int length)
        {
            pool.Enqueue(array);
            for (int i = 0; i < length; i++)
                array[i] = default;
        }

        public abstract class AllowLogicChangeValue_AttackTestSlice2DBase_Disk : AllowLogicChangeValueBase
        {
            public Vector2 position;
            public int layerMask;

            protected void Reset(AttackTestSlice2DBase_Disk slice)
            {
                position = slice.position;
                layerMask = slice.layerMask;
            }
        }
    }
}
