using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class SceneRuinSlice : SliceBase
    {
        public SceneRuinSlice(SceneRuin_Disk sliceDisk) : base(sliceDisk)
        {
            if (diskData.texture != null)
                Background = new StyleBackground(diskData.texture);
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            if (Tools.current != UnityEditor.Tool.Move) return;
            EditorGUI.BeginChangeCheck();
            Vector3 ret = Handles.PositionHandle(Block.GlobalToWorld(diskData.offset) + agent.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                diskData.offset = Block.WorldToGlobal(ret - agent.transform.position);
                diskData.EditorShow(SkillEditorWindow.Inst.Config.Agent, Track.trackIndex);
            }
        }

        public override void Select()
        {
            base.Select();
            if (SkillEditorWindow.Inst.Config.Agent == null) return;
            diskData.EditorShow(SkillEditorWindow.Inst.Config.Agent, Track.trackIndex);
            SkillEditorWindow.Inst.Config.Agent.transform.Find($"场景破坏轨道{Track.trackIndex}").gameObject.SetActive(true);
        }
        public override void UnSelect()
        {
            base.UnSelect();
            SkillEditorWindow.Inst.Config.Agent.transform.Find($"场景破坏轨道{Track.trackIndex}").gameObject.SetActive(false);
        }

        private SceneRuin_Disk diskData => DiskData as SceneRuin_Disk;
        public Texture2D Texture
        {
            get { return diskData.texture; }
            set
            {
                diskData.RuinPixelDic.Clear();
                diskData.texture = value;
                Background = new StyleBackground(diskData.texture);
                for (int y = 0; y < diskData.texture.height; y++)
                    for (int x = 0; x < diskData.texture.width; x++)
                    {
                        Color color = diskData.texture.GetPixel(x, y);
                        if (color.a != 1) continue;
                        Color32 color32 = new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), 255);
                        if (diskData.RuinPixelDic.TryGetValue(color32.ToString(), out List<Vector2Int> pixelList) == false)
                        {
                            pixelList = new List<Vector2Int>();
                            diskData.RuinPixelDic.Add(color32.ToString(), pixelList);
                        }
                        pixelList.Add(new Vector2Int(x, y));
                    }

            }
        }
    }
}
