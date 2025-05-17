using Dalamud.Interface.Colors;
using ECommons;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;

namespace QuestAWAY.Gui
{
    internal class ZoneSettings
    {
        internal static uint zoneConfigId = 0;
        static bool OnlyCreated = false;
        static string Filter = "";

        internal static void Draw()
        {
            ImGuiEx.SetNextItemFullWidth();

            if (ImGui.BeginCombo("##zSelector", zoneConfigId == 0 ? "지역 선택..." : TerritoryName.GetTerritoryName(zoneConfigId)))
            {
                ImGui.SetNextItemWidth(150f);
                ImGui.InputTextWithHint("##fltr", "검색...", ref Filter, 50);
                ImGui.SameLine();
                ImGui.Checkbox("사용자 설정이 존재하는 지역만 표시", ref OnlyCreated);
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudViolet);

                if (Svc.ClientState.LocalPlayer != null && ImGui.Selectable($"현재 지역: {TerritoryName.GetTerritoryName(Svc.ClientState.TerritoryType)}"))
                {
                    zoneConfigId = Svc.ClientState.TerritoryType;
                }

                ImGui.PopStyleColor();

                foreach (var x in Svc.Data.GetExcelSheet<TerritoryType>())
                {
                    var col = P.cfg.ZoneSettings.ContainsKey(x.RowId);

                    if (col)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    }

                    if ((Filter == string.Empty || TerritoryName.GetTerritoryName(x.RowId).Contains(Filter, StringComparison.OrdinalIgnoreCase)) && (!OnlyCreated || col) && x.PlaceName.Value.Name.ToString() != "")
                    {
                        if (ImGui.Selectable(TerritoryName.GetTerritoryName(x.RowId)))
                        {
                            zoneConfigId = x.RowId;
                        }

                        if (zoneConfigId == x.RowId && ImGui.IsWindowAppearing())
                        {
                            ImGui.SetScrollHereY();
                        }
                    }

                    if (col)
                    {
                        ImGui.PopStyleColor();
                    }
                }

                ImGui.EndCombo();
            }

            if (P.cfg.ZoneSettings.TryGetValue(zoneConfigId, out var zoneSettings))
            {
                ImGuiEx.Text(ImGuiColors.DalamudYellow, $"{TerritoryName.GetTerritoryName(zoneConfigId)}의 프로필을 수정 중입니다.");
                ImGui.SameLine();

                if (ImGui.SmallButton("설정 제거") && ImGui.GetIO().KeyCtrl)
                {
                    P.cfg.ZoneSettings.Remove(zoneConfigId);
                    P.ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
                }

                ImGuiEx.Tooltip("Ctrl + 클릭");

                if (zoneConfigId != Svc.ClientState.TerritoryType)
                {
                    ImGuiEx.Text(ImGuiColors.DalamudRed, "현재 지역과 다른 지역 입니다.");
                }

                MainSettings.DrawProfile(zoneSettings);
            }
            else
            {
                ImGuiEx.Text($"선택된 지역에 사용자 설정이 존재하지 않으므로 일반 설정이 사용됩니다.");

                if (zoneConfigId != 0 && ImGui.Button("사용자 설정 생성"))
                {
                    P.cfg.ZoneSettings.Add(zoneConfigId, new());
                    P.ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
                }
            }
        }
    }
}
