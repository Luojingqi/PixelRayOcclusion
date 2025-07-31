using PRO.Buff;
using PRO.DataStructure;
using PRO.Disk;
using PRO.SceneEditor;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace PRO.Console
{
    internal static class Console
    {
        public static Dictionary<string, Func<string[], string>> MethodInfoDic = new Dictionary<string, Func<string[], string>>();
        private static ConsolePanelC panel;
        public static void Init(ConsolePanelC panel)
        {
            Console.panel = panel;
            Type actionType = typeof(Func<string[], string>);
            foreach (var method in typeof(Console).GetMethods())
                try
                {
                    var action = Delegate.CreateDelegate(actionType, method) as Func<string[], string>;
                    foreach (var attr in method.GetCustomAttributes<ConsoleMethodAttribute>())
                        if (MethodInfoDic.TryAdd($"/{attr.MethodHeader}", action) == false)
                            panel.AddLog($"指令方法重复定义：{attr.MethodHeader}");
                }
                catch { }
            pixelTypeInfo_放置 = Pixel.空气.typeInfo;
            pixelColorInfo_放置 = Pixel.空气.colorInfo;
        }


        [ConsoleMethod("0_0")]
        [ConsoleMethod("设置鼠标放置像素类型")]
        #region
        public static string Method_0_0(string[] values)
        {
            if (values.Length < 2) return Method_0_0_ErrorLog;
            var info = Pixel.GetPixelTypeInfo(values[1]);
            if (info == null) return "未找到此像素类型";
            else
            {
                pixelTypeInfo_放置 = info;
                pixelColorInfo_放置 = Pixel.GetPixelColorInfo(info.availableColors[0]);
                Method_0_4_State = true;
                return $"/设置鼠标放置像素类型  {values[1]}";
            }
        }
        private static PixelTypeInfo pixelTypeInfo_放置;
        private static string Method_0_0_ErrorLog = "指令不合法：/设置鼠标放置像素类型  {像素类型名称}";
        #endregion

        [ConsoleMethod("0_1")]
        [ConsoleMethod("设置鼠标放置像素颜色")]
        #region
        public static string Method_0_1(string[] values)
        {
            if (values.Length < 2) return Method_0_1_ErrorLog;
            var info = Pixel.GetPixelColorInfo(values[1]);
            if (info == null) return "未找到此像素颜色";
            else
            {
                pixelColorInfo_放置 = info;
                Method_0_4_State = true;
                return $"/设置鼠标放置像素颜色 {values[1]}";
            }
        }
        private static PixelColorInfo pixelColorInfo_放置;
        private static string Method_0_1_ErrorLog = "指令不合法：/设置鼠标放置像素颜色  {像素颜色名称}";
        #endregion

        [ConsoleMethod("0_2_0")]
        [ConsoleMethod("设置鼠标放置像素0")]
        #region
        public static string Method_0_2_0(string[] values)
        {
            if (values.Length < 3) return Method_0_2_0_ErrorLog;
            var typeInfo = Pixel.GetPixelTypeInfo(values[1]);
            if (typeInfo == null) return "未找到此像素类型";
            else
            {
                var colorInfo = Pixel.GetPixelColorInfo(values[2]);
                if (colorInfo == null) return "未找到此像素名称";

                pixelTypeInfo_放置 = typeInfo;
                pixelColorInfo_放置 = colorInfo;

                Method_0_4_State = true;
                return $"/设置鼠标放置像素  {values[1]}  {colorInfo.colorName}";
            }
        }
        private static string Method_0_2_0_ErrorLog = "指令不合法：/设置鼠标放置像素  {像素类型名称}  {像素颜色名称}";
        #endregion

        [ConsoleMethod("0_2_1")]
        [ConsoleMethod("设置鼠标放置像素1")]
        #region
        public static string Method_0_2_1(string[] values)
        {
            try
            {
                if (values.Length < 3) return Method_0_2_1_ErrorLog;
                var typeInfo = Pixel.GetPixelTypeInfo(values[1]);
                if (typeInfo == null) return "未找到此像素类型";
                else
                {
                    int index = Convert.ToInt32(values[2]);
                    var colorInfo = Pixel.GetPixelColorInfo(typeInfo.availableColors[Mathf.Clamp(index, 0, typeInfo.availableColors.Length - 1)]);
                    pixelTypeInfo_放置 = typeInfo;
                    pixelColorInfo_放置 = colorInfo;

                    Method_0_4_State = true;
                    return $"/设置鼠标放置像素  {values[1]}  {colorInfo.colorName}";
                }
            }
            catch { return Method_0_2_1_ErrorLog; }
        }
        private static string Method_0_2_1_ErrorLog = "指令不合法：/设置鼠标放置像素  {像素类型名称}  {此像素的第n个颜色}";
        #endregion

        [ConsoleMethod("0_3")]
        [ConsoleMethod("设置鼠标放置像素形状")]
        #region
        public static string Method_0_3(string[] values)
        {
            if (values.Length < 2) return Method_0_3_ErrorLog;
            bool useShape = false;
            foreach (var shape in Method_0_3_Shape)
                if (shape == values[1]) useShape = true;
            if (useShape == false) return Method_0_3_ErrorLog;
            pixel_放置_形状 = values[1];
            try
            {
                int r = Math.Abs(Convert.ToInt32(values[2]));
                pixel_放置_半径 = r;
                Method_0_4_State = true;
                return $"/设置鼠标放置像素形状  {values[1]}  {r}";
            }
            catch { return Method_0_3_ErrorLog; }
        }
        private static string pixel_放置_形状 = "正方形";
        private static int pixel_放置_半径 = 1;
        private static string Method_0_3_ErrorLog = "指令不合法：/设置鼠标放置像素形状  {正方形||圆环||实心圆||八边形}  {边长||半径}";
        private static string[] Method_0_3_Shape = { "正方形", "圆环", "实心圆", "八边形" };
        #endregion

        [ConsoleMethod("0_4")]
        [ConsoleMethod("设置鼠标放置像素状态")]
        #region
        public static string Method_0_4(string[] values)
        {
            if (values.Length < 2) return Method_0_4_ErrorLog;
            int b = JudgeBool(values[1]);
            if (b == 0) return Method_0_4_ErrorLog;
            try
            {
                if (b == 1) Method_0_4_State = true;
                else Method_0_4_State = false;
                return $"/设置鼠标放置像素状态  {values[1]}\n按住 左Alt + 鼠标左键 放置在前景快\n按住 左Alt + 鼠标右键 放置在背景快";
            }
            catch { return Method_0_4_ErrorLog; }
        }
        private static string Method_0_4_ErrorLog = "指令不合法：/设置鼠标放置像素状态  {开启/关闭}";
        private static bool method_0_4_State = false;
        private static bool Method_0_4_State
        {
            get { return method_0_4_State; }
            set
            {
                if (method_0_4_State == value) return;
                method_0_4_State = value;
                if (method_0_4_State) TimeManager.Inst.AddToQueue_MainThreadUpdate_UnClear(Method_0_4_主函数更新);
                else TimeManager.Inst.RemoveToQueue_MainThreadUpdate_UnClear(Method_0_4_主函数更新);
            }
        }
        private static void Method_0_4_主函数更新()
        {
            if (pixelTypeInfo_放置 == null || pixelColorInfo_放置 == null) return;
            bool setBlock = false;
            bool setBack = false;
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Mouse0)) setBlock = true;
            else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Mouse1)) setBack = true;
            if (setBlock == false && setBack == false) return;

            if (pixel_放置_形状 == "正方形")
            {
                int ahalfRMax = pixel_放置_半径 / 2;
                int ahalfRMin = ahalfRMax - (pixel_放置_半径 + 1) % 2;
                for (int y = -ahalfRMin; y <= ahalfRMax; y++)
                    for (int x = -ahalfRMin; x <= ahalfRMax; x++)
                        Method_0_4_设置像素(MousePoint.globalPos + new Vector2Int(x, y), setBlock, setBack);
            }
            else if (pixel_放置_形状 == "圆环")
            {
                foreach (var globalPos in DrawTool.GetRing(MousePoint.globalPos, pixel_放置_半径))
                    Method_0_4_设置像素(globalPos, setBlock, setBack);
            }
            else if (pixel_放置_形状 == "实心圆")
            {
                var list = new List<Vector2Int>();
                foreach (var globalPos in DrawTool.GetCircle(MousePoint.globalPos, pixel_放置_半径, ref list))
                    Method_0_4_设置像素(globalPos, setBlock, setBack);
            }
            else if (pixel_放置_形状 == "八边形")
            {
                foreach (var globalPos in DrawTool.GetOctagon(MousePoint.globalPos, pixel_放置_半径))
                    Method_0_4_设置像素(globalPos, setBlock, setBack);
            }
        }
        private static void Method_0_4_设置像素(Vector2Int globalPos, bool setBlock, bool setBack)
        {
            Vector2Int blockPos = Block.GlobalToBlock(globalPos);
            Vector2Byte pixelPos = Block.GlobalToPixel(globalPos);
            if (setBlock)
                SceneManager.Inst.NowScene.GetBlock(blockPos)?.GetPixel(Block.GlobalToPixel(globalPos)).Replace(pixelTypeInfo_放置, pixelColorInfo_放置);
            if (setBack)
                SceneManager.Inst.NowScene.GetBackground(blockPos)?.GetPixel(Block.GlobalToPixel(globalPos)).Replace(pixelTypeInfo_放置, pixelColorInfo_放置);
        }
        #endregion

        [ConsoleMethod("1_0")]
        [ConsoleMethod("保存")]
        #region
        public static string Method_1_0(string[] values)
        {
            //var countdown = SceneManager.Inst.NowScene.SaveAll();
            //TimeManager.enableUpdate = false;
            //ThreadPool.QueueUserWorkItem((obj) =>
            //{
            //    if (countdown.Wait(1000 * 15))
            //        Log.Print("保存完成");
            //    else
            //        Log.Print("保存超时");
            //    TimeManager.enableUpdate = true;
            //});
            return "/保存";
        }
        #endregion

        [ConsoleMethod("1_2")]
        [ConsoleMethod("加载")]
        #region
        public static string Method_1_2(string[] values)
        {
            //    for (int x = -3; x <= 3; x++)
            //        for (int y = -3; y <= 3; y++)
            //        {
            //            SceneManager.Inst.NowScene.ThreadLoadOrCreateBlock(new(x, y));
            //        }
            //    BlockMaterial.UpdateBind();
            //SceneManager.Inst.NowScene.LoadAll();
            return "加载";
        }
        #endregion

        [ConsoleMethod("1_3")]
        [ConsoleMethod("相机坐标")]
        #region
        public static string Method_1_3(string[] values)
        {
            try
            {
                if (values.Length < 2) return Method_1_3_ErrorLog;
                string[] xy = values[1].Split(',', '，');
                int x = Convert.ToInt32(xy[0]);
                int y = Convert.ToInt32(xy[1]);
                Camera.main.transform.position = Block.BlockToWorld(new Vector2Int(x, y)) + new Vector3(0, 0, -10);
                return $"/相机坐标  {x}  {y}";
            }
            catch
            {
                return Method_1_3_ErrorLog;
            }
        }
        private static string Method_1_3_ErrorLog = "指令不合法：/相机坐标  {x,y}";
        #endregion

        [ConsoleMethod("2_0")]
        [ConsoleMethod("场景编辑窗口状态")]
        #region
        public static string Method_2_0(string[] values)
        {
            if (values.Length < 2) return Method_2_0_ErrorLog;
            int b = JudgeBool(values[1]);
            if (b == 0) return Method_2_0_ErrorLog;
            if (b == 1 && SceneEditorCanvasC.Inst.gameObject.activeSelf == false) SceneEditorCanvasC.Inst.Open();
            else SceneEditorCanvasC.Inst.Close();
            return $"/场景编辑窗口状态  {(b == 1 ? "开启" : "关闭")}";
        }
        private static string Method_2_0_ErrorLog = "指令不合法：/场景编辑窗口状态  {开启/关闭}";
        #endregion

        private static int JudgeBool(string value)
        {
            if (value[0] == '开' || value[0] == 't' || value[0] == 'T') return 1;
            if (value[0] == '关' || value[0] == 'f' || value[0] == 'F') return -1;
            return 0;
        }


        [ConsoleMethod("5_0")]
        [ConsoleMethod("导电")]
        #region
        public static string Method_5_0(string[] values)
        {
            try
            {
                if (values.Length < 3) return Method_5_0_ErrorLog;
                string[] xy0 = values[1].Split(',', '，');
                string[] xy1 = values[2].Split(',', '，');
                Vector2Int start = new Vector2Int(Convert.ToInt32(xy0[0]), Convert.ToInt32(xy0[1]));
                Vector2Int end = new Vector2Int(Convert.ToInt32(xy1[0]), Convert.ToInt32(xy1[1]));
                //BuffEx.Set导电Path(SceneManager.Inst.NowScene, start, end, true, false);
                return $"/导电  {xy0[0]},{xy0[1]} {xy1[0]},{xy1[1]}";
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return Method_5_0_ErrorLog;
            }
        }
        private static string Method_5_0_ErrorLog = "指令不合法：/导电  {x1,y1} {x2,y2}";
        #endregion
    }
}
