using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class ParticleSlice : SliceBase
    {
        private ParticleSlice_Disk slice_Disk => DiskData as ParticleSlice_Disk;
        public ParticleSlice(ParticleSlice_Disk sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.loadPath != null)
            {
                var strs = sliceDisk.loadPath.Split('\\', '/');
                LabelView.text = strs[strs.Length - 1];
            }
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one);
            Gizmos.DrawRay(slice_Disk.position, slice_Disk.directionMin);
            Gizmos.DrawRay(slice_Disk.position, slice_Disk.directionMax);
            for (int i = 1; i < 10; i++)
            {
                Gizmos.DrawRay(slice_Disk.position, Vector3.Lerp(slice_Disk.directionMin, slice_Disk.directionMax, i * 0.1f));
            }
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            HandlePosition(agent, Quaternion.identity, ref slice_Disk.position);
            HandlePosition(agent);
        }

        protected void HandlePosition(SkillPlayAgent agent)
        {
            if (Tools.current != UnityEditor.Tool.Move) return;
            Handles.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one);
            EditorGUI.BeginChangeCheck();
            Vector3 ret0 = Handles.PositionHandle(slice_Disk.position + slice_Disk.directionMin, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                slice_Disk.directionMin = ret0 - slice_Disk.position;
                SkillEditorWindow.Inst.UpdateFrame();
            }
            EditorGUI.BeginChangeCheck();
            Vector3 ret1 = Handles.PositionHandle(slice_Disk.position + slice_Disk.directionMax, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                slice_Disk.directionMax = ret1 - slice_Disk.position;
                SkillEditorWindow.Inst.UpdateFrame();
            }
        }

        [LabelText("粒子加载路径")]
        [ShowInInspector]
        public string LoadPath
        {
            get { return slice_Disk.loadPath; }
            set
            {
                slice_Disk.loadPath = value;
                var strs = value.Split('\\', '/');
                LabelView.text = strs[strs.Length - 1];
            }
        }
        [LabelText("粒子创建位置（相对）")]
        [ShowInInspector]
        public Vector3 Position
        {
            get { return slice_Disk.position; }
            set { slice_Disk.position = value; }
        }
        [LabelText("发射方向min")]
        [ShowInInspector]
        public Vector3 DirectionMin
        {
            get { return slice_Disk.directionMin; }
            set { slice_Disk.directionMin = value; }
        }
        [LabelText("发射方向max")]
        [ShowInInspector]
        public Vector3 DirectionMax
        {
            get { return slice_Disk.directionMax; }
            set { slice_Disk.directionMax = value; }
        }
        [LabelText("发射的力")]
        [ShowInInspector]
        public Vector2 forceRange
        {
            get { return slice_Disk.forceRange; }
            set { slice_Disk.forceRange = value; }
        }
        [LabelText("发射的数量")]
        [ShowInInspector]
        public Vector2Int numRange
        {
            get { return slice_Disk.numRange; }
            set { slice_Disk.numRange = value; }
        }
        [LabelText("存活时间范围（单位ms）")]
        [ShowInInspector]
        public Vector2Int surviveTimeRange
        {
            get { return slice_Disk.surviveTimeRange; }
            set { slice_Disk.surviveTimeRange = value; }
        }
        [LabelText("颜色")]
        [ShowInInspector]
        public Color color
        {
            get { return slice_Disk.color; }
            set { slice_Disk.color = value; }
        }
    }
}