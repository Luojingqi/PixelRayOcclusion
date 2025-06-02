using System;
using UnityEngine;

namespace PRO.SkillEditor
{
    public abstract class AttackTestSlice2DBase_Disk : Slice_DiskBase
    {
        public Vector2 position;
        public Quaternion rotation = Quaternion.identity;
        public Vector2 scale = Vector2.one;

        public event Action<RaycastHit2D[]> testEvent;

        protected void InvokeEvent(RaycastHit2D[] value) => testEvent?.Invoke(value);
    }
}
