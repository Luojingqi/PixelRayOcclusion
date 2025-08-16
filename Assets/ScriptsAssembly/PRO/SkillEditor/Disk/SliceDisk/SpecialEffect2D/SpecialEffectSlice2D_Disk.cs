using UnityEngine;

namespace PRO.SkillEditor
{
    public class SpecialEffectSlice2D_Disk : Slice_DiskBase
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.one;
        public Sprite sprite;

        public AllowLogicChangeValue_SpecialEffectSlice2D_Disk valueChange = new();
        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_SpecialEffectSlice2D(agent, playData, this, frameData);

            #region 生成特效轨道的精灵Render
            if (frameData.trackIndex >= agent.SpecialEffect2DSpriteList.Count)
            {
                for (int i = agent.SpecialEffect2DSpriteList.Count; i <= frameData.trackIndex; i++)
                {
                    SpriteRenderer spriteRenderer = new GameObject($"特效轨道{i}").AddComponent<SpriteRenderer>();
                    spriteRenderer.sortingOrder = 20;
                    spriteRenderer.transform.SetParent(agent.transform);
                    agent.SpecialEffect2DSpriteList.Add(spriteRenderer);
                }
            }
            #endregion
            SpriteRenderer renderer = agent.SpecialEffect2DSpriteList[frameData.trackIndex];
            renderer.sprite = sprite;
            renderer.transform.localPosition = position;
            renderer.transform.localRotation = rotation;
            renderer.transform.localScale = scale;

            if (frameData.sliceFrame == frameLength - 1)
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].After_SpecialEffectSlice2D(agent, playData, this, frameData);
        }


        public class AllowLogicChangeValue_SpecialEffectSlice2D_Disk : AllowLogicChangeValueBase
        {
            public Vector3 position;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 scale = Vector3.one;
            public Sprite sprite;

            protected void Reset(SpecialEffectSlice2D_Disk slice)
            {
                position = slice.position;
                rotation = slice.rotation;
                scale = slice.scale;
                sprite = slice.sprite;
            }
        }
    }
}
