using Dalamud.Interface.Windowing;
using ECommons.Funding;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;

namespace QuestAWAY.Gui
{
    internal class ConfigGui : Window
    {
        public ConfigGui() : base("QuestAWAY 설정")
        { }

        public override void OnClose()
        {
            base.OnClose();
            P.cfg.Save();
        }

        public override void PreDraw()
        {
            base.PreDraw();
            ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.FirstUseEver);
        }

        public override void Draw()
        {
            P.reprocessAreaMap = true;
            P.reprocessNaviMap = true;
            PatreonBanner.DrawRight();
            ImGuiEx.EzTabBar("questaway", PatreonBanner.Text,
                ("일반 설정", MainSettings.Draw, null, true),
                ("지역별 설정", ZoneSettings.Draw, null, true),
                ("개발자 기능", DevSettings.Draw, null, true)
                );
        }
    }
}
