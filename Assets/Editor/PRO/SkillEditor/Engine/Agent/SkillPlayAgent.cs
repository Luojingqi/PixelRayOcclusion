using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static PRO.SkillEditor.Slice_DiskBase;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 技能播放的执行人
    /// </summary>
    public class SkillPlayAgent : MonoScriptBase, IScene, ITime_Update
    {
        public SceneEntity Scene => _scene;
        private SceneEntity _scene;
        public void SetScene(SceneEntity scene) => _scene = scene;
        public void Init()
        {
            if (AgentSprice == null)
                AgentSprice = transform.GetComponent<SpriteRenderer>();
            Skill = idle;
            Play = true;
        }
        [LabelText("动画轨道精灵渲染器")]
        public SpriteRenderer AgentSprice;
        /// <summary>
        /// 特效轨道的精灵图渲染器
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [LabelText("正在播放")]
        [ShowInInspector]
        /// <summary>
        /// 当前播放的技能，设置会立即播放
        /// </summary>
        public Skill_Disk Skill
        {
            get { return skill; }
            set
            {
                skill = value;
                ClearTimeAndBuffer();
                if (value != null)
                {
                    Skill.UpdateFrame(this, 0);
                    play = true;
                }
            }
        }
        private Skill_Disk skill;

        [LabelText("空闲时播放")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// 播放，暂停播放技能,
        /// </summary>
        public bool Play
        {
            get { return play; }
            set
            {
                play = value;
                if (play == true && skill == null)
                    Skill = idle;

            }
        }
        private bool play = false;

        public int NowFrame
        {
            get { return nowFrame; }
            set
            {
                if (value < 0) return;
                if (Skill != null && value >= Skill.MaxFrame) return;
                nowFrame = value;
            }
        }

        [ShowInInspector]
        private int nowFrame;


        private float time;
        public void TimeUpdate()
        {
            if (Play == false || skill == null) return;
            if (UpdateFrameScript(Skill))
            {
                Skill = idle;
            }
        }
        #region 迭代播放
        /// <summary>
        /// 不自动播放，使用手动api播放，需要每帧调用
        /// 返回为真代表播放完毕
        /// </summary>
        /// <param name="playSkill"></param>
        /// <param name="playTrack"></param>
        /// <param name="autoClear">是否播放完毕后自动调用清理时间和缓冲区函数</param>
        /// <returns></returns>
        public bool UpdateFrameScript(Skill_Disk playSkill, int playTrack = ~0, bool autoClear = true)
        {
            time += TimeManager.deltaTime * 1000;
            while (time >= playSkill.FrameTime)
            {
                time -= playSkill.FrameTime;
                playSkill.UpdateFrame(this, nowFrame++, playTrack);
                if (nowFrame >= playSkill.MaxFrame)
                {
                    if (autoClear) ClearTimeAndBuffer();
                    return true;
                }
            }
            return false;
        }

        public void ClearTimeAndBuffer()
        {
            nowFrame = 0;
            time = 0;
            ClearBuffer();
        }
        #endregion

        private Dictionary<string, ISliceBufferData> SliceBufferDataDic = new Dictionary<string, ISliceBufferData>();

        public T GetBufferData<T>(Slice_DiskBase disk) where T : class, ISliceBufferData
        {
            string id = disk.name;
            if (SliceBufferDataDic.TryGetValue(id, out ISliceBufferData value))
                if (value is T ret)
                    return ret;
            return null;
        }
        /// <summary>
        /// 获取某个id的缓冲区数据
        /// </summary>
        public T GetBufferData<T>(string id) where T : class, ISliceBufferData
        {
            if (SliceBufferDataDic.TryGetValue(id, out ISliceBufferData value))
                if (value is T ret)
                    return ret;
            return null;
        }

        public void AddBufferData(Slice_DiskBase disk, ISliceBufferData data)
        {
            string id = disk.name;
            if (SliceBufferDataDic.ContainsKey(id)) SliceBufferDataDic[id] = data;
            else SliceBufferDataDic.Add(id, data);
        }
        public void AddBufferData(string id, ISliceBufferData data)
        {
            if (SliceBufferDataDic.ContainsKey(id)) SliceBufferDataDic[id] = data;
            else SliceBufferDataDic.Add(id, data);
        }

        public void ClearBuffer()
        {
            foreach (var value in SliceBufferDataDic.Values)
                value.PutIn();
        }
    }
}
