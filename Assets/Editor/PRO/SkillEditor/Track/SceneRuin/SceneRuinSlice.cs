using Sirenix.OdinInspector;
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
            if (diskData.sprite != null)
                Background = new StyleBackground(diskData.sprite);
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
            if (SkillEditorWindow.Inst.Config.Agent == null) return;
            SkillEditorWindow.Inst.Config.Agent.transform.Find($"场景破坏轨道{Track.trackIndex}").gameObject.SetActive(false);
        }

        private SceneRuin_Disk diskData => DiskData as SceneRuin_Disk;





        [LabelText("偏移")]
        [ShowInInspector]
        public Vector2Int offset { get => diskData.offset; set => diskData.offset = value; }
        [LabelText("破坏的层")]
        [ShowInInspector]
        public BlockBase.BlockType BlockType { get => diskData.BlockType; set => diskData.BlockType = value; }
        [LabelText("破坏的坚硬度")]
        [ShowInInspector]
        public int hardness { get => diskData.hardness; set => diskData.hardness = value; }
        [LabelText("破坏的耐久度")]
        [ShowInInspector]
        public int durability { get => diskData.durability; set => diskData.durability = value; }
        [LabelText("破坏的点集")]
        [ShowInInspector]
        public List<Vector2Int> pixelList { get => diskData.pixelList; set => diskData.pixelList = value; }




        [LabelText("颜色精灵图")]
        [ShowInInspector]
        public Sprite sprite
        {
            get { return diskData.sprite; }
            set
            {
                diskData.pixelList.Clear();
                diskData.sprite = value;
                Background = new StyleBackground(diskData.sprite);
                for (int y = 0; y < diskData.sprite.rect.height; y++)
                    for (int x = 0; x < diskData.sprite.rect.width; x++)
                    {
                        Color32 color = (Color32)diskData.sprite.texture.GetPixel(x + (int)diskData.sprite.rect.x, y + (int)diskData.sprite.rect.y);
                        if (color.a != 255) continue;
                        diskData.pixelList.Add(new Vector2Int(x, y));
                    }
            }
        }
    }
}
