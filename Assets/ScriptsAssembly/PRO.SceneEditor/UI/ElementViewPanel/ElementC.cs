using PRO.SceneEditor;
using PRO.Tool;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

internal class ElementC : MonoBehaviour
{
    public RawImage icon;
    private TMP_Text name_Text;
    private Button button;
    public ElementEntity entity;
    
    public void Init()
    {
        icon = transform.Find("icon").GetComponent<RawImage>();
        name_Text = transform.Find("Name").GetComponent<TMP_Text>();
        button = transform.GetComponent<Button>();
        // icon.sprite = Sprite.Create(new Texture2D(0, 0), new Rect(), new Vector2());
        button.onClick.AddListener(() => { SceneEditorCanvasC.Inst.SwitchHold(this); });
    }

    public void Clear()
    {
        name_Text.text = "已回收";
        entity = null;
        texture2DPool.PutIn((Texture2D)icon.texture);
        icon.texture = null;
    }

    public void SetEntity(FileInfo info, ElementEntity entity)
    {
        string infoName = info.Name.Substring(0, info.Name.Length - info.Extension.Length);
        name_Text.text = infoName;
        this.entity = entity;

        string pngPath = info.FullName.Substring(0, info.FullName.Length - info.Extension.Length) + ".png";
        if (File.Exists(pngPath))
        {
            Texture2D texture2D = texture2DPool.TakeOut();
            texture2D.LoadImage(File.ReadAllBytes(pngPath));
            icon.texture = texture2D;
        }
    }

    #region 纹理池
    private static Texture2DPool texture2DPool = new Texture2DPool(50, true);
    private class Texture2DPool : ObjectPoolBase<Texture2D>
    {
        public Texture2DPool(int maxNuber, bool isCanExceed) : base(maxNuber, isCanExceed)
        {
        }

        public override void Remove(Texture2D item)
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
