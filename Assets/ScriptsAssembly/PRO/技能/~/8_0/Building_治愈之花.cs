using PRO;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class Building_治愈之花 : BuildingBase
    {
        private HashSet<Building_Pixel> pixels_茎秆_All = new HashSet<Building_Pixel>();
        private HashSet<Building_Pixel> pixels_花瓣_All = new HashSet<Building_Pixel>();
        private HashSet<Building_Pixel> pixels_花瓣_Death = new HashSet<Building_Pixel>();
        public override void Init()
        {
            foreach (var pixel in AllPixel.Values)
                if (pixel.GetState() == Building_Pixel.State.Survival)
                {
                    if (pixel.typeInfo.typeName == "花朵茎秆")
                    {
                        pixels_茎秆_All.Add(pixel);
                    }
                    else if (pixel.typeInfo.typeName == "花朵花瓣")
                    {
                        pixels_花瓣_All.Add(pixel);
                    }
                }
            if (pixels_茎秆_All.Count <= 5)
            {
                foreach (var building_Pixel in pixels_花瓣_All)
                    if (pixels_花瓣_Death.Contains(building_Pixel) == false)
                        building_Pixel.pixel.blockBase.SetPixel(Pixel.空气.Clone(building_Pixel.pixel.pos));
            }
        }

        protected override void PixelSwitch_Death(Building_Pixel pixelB)
        {
            if (pixelB.typeInfo.typeName == "花朵茎秆")
            {
                pixels_茎秆_All.Remove(pixelB);
                if (pixels_茎秆_All.Count <= 8)
                    foreach (var building_Pixel in pixels_花瓣_All)
                        if (pixels_花瓣_Death.Contains(building_Pixel) == false)
                            building_Pixel.pixel.blockBase.SetPixel(Pixel.空气.Clone(building_Pixel.pixel.pos));
            }
            if (pixelB.typeInfo.typeName == "花朵花瓣")
            {
                var particle = ParticleManager.Inst.GetPool("单像素").TakeOut(_scene);
                particle.SetGlobal(pixelB.pixel.posG);
                particle.Renderer.color = pixelB.colorInfo.color;
                particle.gameObject.layer = (int)GameLayer.Particle_Block_Role;
                particle.SurviveTimeRange = new Vector2Int(5000, 10000);
                particle.CollisionEnterEvent += (p, c) =>
                {
                    var role = GamePlayMain.Inst.GetRole(c.transform);
                    if (role != null)
                    {

                        var context = CombatContext.TakeOut();
                        context.SetAgent(role, null, null);
                        context.Calculate_战斗技能初始化_直接对发起人结算(stackalloc StartCombatEffectData[]
                        {
                            new(属性.治疗 , 1),
                        });
                        LogPanelC.Inst.AddLog(context, true);
                        CombatContext.PutIn(context);
                        ParticleManager.Inst.GetPool(p.loadPath).PutIn(p);
                    }
                    else
                    {
                        p.RemainTime -= Random.Range(500, 2500);
                    }
                };
                pixels_花瓣_Death.Add(pixelB);

                if (pixels_花瓣_All.Count == pixels_花瓣_Death.Count)
                {
                    Delete();
                }
            }
        }

        protected override void PixelSwitch_Survival(Building_Pixel pixelB)
        {
            if (pixelB.typeInfo.typeName == "花朵花瓣")
            {
                pixels_花瓣_Death.Remove(pixelB);
            }
        }

        private float time;
        private void Update()
        {
            if (pixels_花瓣_Death.Count == 0) return;
            time += TimeManager.deltaTime;
            if (time > 30)
            {
                time = 0;
                Building_Pixel pixelB = null;
                foreach (var pixelb in pixels_花瓣_Death) { pixelB = pixelb; break; }

                pixelB.pixel.blockBase.SetPixel(Pixel.TakeOut(pixelB.typeInfo, pixelB.colorInfo, pixelB.pixel.pos));
            }
        }
    }
}
