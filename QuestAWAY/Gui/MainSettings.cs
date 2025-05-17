using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Numerics;

namespace QuestAWAY.Gui
{
    internal class MainSettings
    {
        internal static void Draw()
        {
            ImGuiEx.Text(ImGuiColors.DalamudYellow, "일반 프로필을 수정 중입니다.");

            if (P.cfg.ZoneSettings.ContainsKey(Svc.ClientState.TerritoryType))
            {
                ImGuiEx.Text(ImGuiColors.DalamudRed, "현재 지역에 사용자 설정이 존재하므로 일반 설정은 사용되지 않습니다.");
            }

            DrawProfile(P.cfg);
        }

        internal static void DrawProfile(Configuration config)
        {
            if (ImGui.Button("설정 복사"))
            {
                Safe(delegate
                {
                    var copy = JsonConvert.DeserializeObject<Configuration>(JsonConvert.SerializeObject(config));
                    copy.ZoneSettings = null;
                    ImGui.SetClipboardText(JsonConvert.SerializeObject(copy));
                });
            }

            ImGui.SameLine();

            if (ImGui.Button("설정 붙혀넣기") && ImGui.GetIO().KeyCtrl)
            {
                Safe(delegate
                {
                    var imp = JsonConvert.DeserializeObject<Configuration>(ImGui.GetClipboardText());
                    config.QuickEnable = imp.QuickEnable;
                    config.Enabled = imp.Enabled;
                    config.AetheryteInFront = imp.AetheryteInFront;
                    config.Bigmap = imp.Bigmap;
                    config.Minimap = imp.Minimap;
                    config.CustomPathes = imp.CustomPathes;
                    config.HideAreaMarkers = imp.HideAreaMarkers;
                    config.HideFateCircles = imp.HideFateCircles;
                    config.HiddenTextures = imp.HiddenTextures;
                    QuestAWAY.ApplyMemoryReplacer();
                });
            }

            ImGuiEx.Tooltip("Ctrl + 클릭");

            if (ImGui.Checkbox("플러그인 활성화", ref config.Enabled))
            {
                QuestAWAY.ApplyMemoryReplacer();
            }

            ImGui.Checkbox("지도에서 아이콘 숨기기", ref config.Bigmap);
            ImGui.Checkbox("미니맵에서 아이콘 숨기기", ref config.Minimap);
            ImGui.Checkbox("지도 상단에 빠른 활성화/비활성화, 설정 버튼 표시", ref config.QuickEnable);

            if (ImGui.Checkbox("지도에서 에테라이트를 항상 위에 표시 (Ctrl키를 사용해 원래대로 표시)", ref config.AetheryteInFront))
            {
                QuestAWAY.ApplyMemoryReplacer();
            }

            ImGui.Text("추가로 숨길 아이콘 경로 (한 줄에 하나씩, _hr1, .tex 없이 작성)");
            ImGui.InputTextMultiline("##QAUSERADD", ref config.CustomPathes, 1000000, new Vector2(300f, 100f));
            ImGui.Text("특별 숨김 옵션:");
            ImGui.Checkbox("돌발 구역 숨기기", ref config.HideFateCircles);
            ImGui.Checkbox("특정 장소에 꽂혀있는 핀 숨기기", ref config.HideAreaMarkers);
            ImGui.Text("종류:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(200f);

            if (ImGui.BeginCombo("##QASELCAT", Static.CategoryNames[P.selectedCategory]))
            {
                foreach (var cat in Static.CategoryNames)
                {
                    if (ImGui.Selectable(cat.Value, P.selectedCategory == cat.Key))
                    {
                        P.selectedCategory = cat.Key;
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.Text("빠른 선택:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100f);

            if (ImGui.BeginCombo("##QASELOPT", "선택..."))
            {
                if (ImGui.Selectable("모두"))
                {
                    if (P.selectedCategory == Category.All)
                    {
                        config.HiddenTextures.UnionWith(Static.MapIcons);
                    }
                    else
                    {
                        config.HiddenTextures.UnionWith(Static.MapIcons.Where(e => Static.MapIconData[e].Category == P.selectedCategory));
                    }

                    P.BuildHiddenByteSet();
                }

                if (ImGui.Selectable("선택 안 함"))
                {
                    if (P.selectedCategory == Category.All)
                    {
                        config.HiddenTextures.Clear();
                    }
                    else
                    {
                        config.HiddenTextures.ExceptWith(Static.MapIcons.Where(e => Static.MapIconData[e].Category == P.selectedCategory));
                    }

                    P.BuildHiddenByteSet();
                }

                ImGui.EndCombo();
            }

            ImGui.SameLine();
            ImGui.Checkbox("선택한 항목만 표시", ref P.onlySelected);

            //ImGui.BeginChild("##QAWAYCHILD");
            ImGuiHelpers.ScaledDummy(10f);

            var width = ImGui.GetColumnWidth();
            var numColumns = Math.Max((int)(width / 100), 2);

            ImGui.Columns(numColumns);

            for (var i = 0; i < numColumns; i++)
            {
                ImGui.SetColumnWidth(i, width / numColumns);
            }

            foreach (var e in Static.MapIcons)
            {
                var b = config.HiddenTextures.Contains(e);

                if ((P.selectedCategory == Category.All || P.selectedCategory == Static.MapIconData[e].Category) && (!P.onlySelected || config.HiddenTextures.Contains(e)))
                {
                    ImGui.Checkbox("##" + e, ref b);
                    ImGui.SameLine();
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 11);
                    ImGuiDrawImage(e + "_hr1");

                    if (ImGui.IsItemHovered() && ImGui.GetMouseDragDelta() == Vector2.Zero)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

                        if (Static.MapIconData[e].Name.Length > 0 || ImGui.GetIO().KeyCtrl)
                        {
                            ImGui.SetTooltip(Static.MapIconData[e].Name.Length > 0 ? Static.MapIconData[e].Name : e);
                        }

                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.SetClipboardText(e);
                        }

                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                        {
                            b = !b;
                        }
                    }

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 11);
                    ImGui.NextColumn();

                    if (config.HiddenTextures.Contains(e) && !b)
                    {
                        config.HiddenTextures.Remove(e);

                        P.BuildHiddenByteSet();
                    }

                    if (!config.HiddenTextures.Contains(e) && b)
                    {
                        config.HiddenTextures.Add(e);

                        P.BuildHiddenByteSet();
                    }
                }
            }

            ImGui.Columns(1);
            //ImGui.EndChild();
        }
    }
}
