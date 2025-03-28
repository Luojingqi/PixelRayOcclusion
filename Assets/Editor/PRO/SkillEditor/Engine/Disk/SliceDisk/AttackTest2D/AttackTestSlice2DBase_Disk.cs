using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public abstract class AttackTestSlice2DBase_Disk : SliceBase_Disk
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.one;

        public class BufferData : ISliceBufferData
        {
            public List<RaycastHit2D> value = new List<RaycastHit2D>();
            public void PutIn()
            {
                value.Clear();
                pool.PutIn(this);
            }
            public static BufferData TakeOut()
            {
                return pool.TakeOut();
            }

            private static ObjectPool<BufferData> pool = new ObjectPool<BufferData>();
        }
    }
}
