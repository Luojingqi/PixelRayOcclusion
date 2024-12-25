using PRO.SceneEditor;
using PRO.Tool;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class ElementC : MonoBehaviour
{
    public RawImage icon;
    private TMP_Text name_Text;
    private Button button;
    public Element_Disk entity;

    public void Init()
    {
        icon = transform.Find("icon").GetComponent<RawImage>();
        name_Text = transform.Find("Name").GetComponent<TMP_Text>();
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(() => { SceneEditorCanvasC.Inst.SwitchHold(this); });
    }

    public void Clear()
    {
        name_Text.text = "已回收";
        entity = null;
        texture2DPool.PutIn((Texture2D)icon.texture);
        icon.texture = null;
    }

    public void SetEntity(FileInfo info, Element_Disk entity)
    {
        name_Text.text = entity.name;
        this.entity = entity;
        Texture2D texture2D = texture2DPool.TakeOut();
        texture2D.LoadImage(entity.pngBytes);
        icon.texture = texture2D;
    }

    #region 纹理池
    private static Texture2DPool texture2DPool = new Texture2DPool(50, true);
    private class Texture2DPool : ObjectPoolBase<Texture2D>
    {
        public Texture2DPool(int maxNuber, bool isCanExceed) : base(maxNuber, isCanExceed)
        {
        }

        public override void Destroy(Texture2D item)
        {
            GameObject.Destroy(item);
        }

        protected override Texture2D NewObject()
        {
            var ret = new Texture2D(0, 0);
            ret.filterMode = FilterMode.Point;
            return ret;
        }
    }
    #endregion
}
