using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace QuestAWAY.Gui
{
    internal class DevSettings
    {
        internal static void Draw()
        {
            ImGui.Checkbox("[개발자] 텍스쳐 수집 활성화", ref P.collect);

            if (P.collect)
            {
                if (ImGui.Button("초기화"))
                {
                    P.texSet.Clear();
                }

                ImGui.SameLine();
                ImGui.Checkbox("텍스쳐 표시", ref P.collectDisplay);

                if (P.collectDisplay)
                {
                    foreach (var e in P.texSet)
                    {
                        ImGuiDrawImage(e);
                        ImGui.SameLine();

                        if (!Static.MapIcons.Contains(e))
                        {
                            ImGui.PushStyleColor(ImGuiCol.Button, 0xff0000ff);
                        }

                        if (ImGui.Button("복사: " + e))
                        {
                            ImGui.SetClipboardText(e);
                        }

                        if (!Static.MapIcons.Contains(e))
                        {
                            ImGui.PopStyleColor();
                        }
                    }
                }

                var s = string.Join("\n", P.texSet);

                ImGui.InputTextMultiline("##QADATA", ref s, 1000000, new Vector2(300f, 100f));
            }

            ImGui.Separator();

            if (ImGui.Button("숨겨진 텍스쳐 리스트 지우기" + (ImGui.GetIO().KeyCtrl ? "" : " (Ctrl + 클릭)")) && ImGui.GetIO().KeyCtrl)
            {
                P.cfg.HiddenTextures.Clear();
                P.BuildHiddenByteSet();
            }

            ImGui.Checkbox("프로파일링", ref P.profiling);

            if (P.profiling)
            {
                ImGui.Text("Total time: " + P.totalTime);
                ImGui.Text("Total ticks: " + P.totalTicks);
                ImGui.Text("Tick avg: " + P.totalTime / (float)P.totalTicks);
                ImGui.Text("MS avg: " + P.totalTime / (float)P.totalTicks / Stopwatch.Frequency * 1000 + " ms");

                if (ImGui.Button("Reset##SW"))
                {
                    P.totalTicks = 0;
                    P.totalTime = 0;
                }
            }
        }
    }
}
