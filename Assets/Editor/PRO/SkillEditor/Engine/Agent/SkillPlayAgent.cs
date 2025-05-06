using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static PRO.SkillEditor.Slice_DiskBase;
namespace PRO.SkillEditor
{
    /// <summary>
    /// ���ܲ��ŵ�ִ����
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
        [LabelText("�������������Ⱦ��")]
        public SpriteRenderer AgentSprice;
        /// <summary>
        /// ��Ч����ľ���ͼ��Ⱦ��
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [LabelText("���ڲ���")]
        [ShowInInspector]
        /// <summary>
        /// ��ǰ���ŵļ��ܣ����û���������
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

        [LabelText("����ʱ����")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// ���ţ���ͣ���ż���,
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
        #region ��������
        /// <summary>
        /// ���Զ����ţ�ʹ���ֶ�api���ţ���Ҫÿ֡����
        /// ����Ϊ����������
        /// </summary>
        /// <param name="playSkill"></param>
        /// <param name="playTrack"></param>
        /// <param name="autoClear">�Ƿ񲥷���Ϻ��Զ���������ʱ��ͻ���������</param>
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
        /// ��ȡĳ��id�Ļ���������
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
