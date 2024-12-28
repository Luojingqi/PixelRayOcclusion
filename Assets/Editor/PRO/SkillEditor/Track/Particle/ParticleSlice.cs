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

        [LabelText("Á£×Ó¼ÓÔØÂ·¾¶")]
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

    }
}