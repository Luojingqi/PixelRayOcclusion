using Sirenix.OdinInspector;
using UnityEngine;
namespace PRO.Skill
{
    public abstract class SkillPointerBase : SerializedMonoBehaviour
    {
        /// <summary>
        /// ·¶Î§±ß¿ò
        /// </summary>
        public SpriteRenderer RangeBox { get; private set; }
        /// <summary>
        /// ÖÐÐÄµã
        /// </summary>
        public Transform Center { get; private set; }

        public virtual void Init()
        {
            RangeBox = transform.Find("RangeBox").GetComponent<SpriteRenderer>();
            Center = transform.Find("Center");
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void SetPosition(Vector2Int global)
        {
            transform.position = Block.GlobalToWorld(global) + Vector3.one * Pixel.Size / 2;
        }
    }
}