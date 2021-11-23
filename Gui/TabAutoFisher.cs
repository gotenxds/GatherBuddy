using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using GatherBuddy.Enums;
using ImGuiNET;

namespace GatherBuddy.Gui
{
    public partial class Interface
    {
        private static void DrawContinueToFishBox()
            => DrawCheckbox("Auto fish",
                "Toggle whether to just continue auto fish indefinitely",
                GatherBuddy.Config.AutoFish, b =>
                {
                    GatherBuddy.Config.AutoFish = b;
                    if (b)
                    {
                        GatherBuddy.Config.AntiAFK = b;
                    }
                });


        private static void DrawAntiAFKBox()
            => DrawCheckbox("Anti AFK",
                "Randomly move left and right every few minutes",
                GatherBuddy.Config.AntiAFK, b => GatherBuddy.Config.AntiAFK = b || GatherBuddy.Config.AutoFish);

        private static void DrawLittletThalaosBoxads()
        {
            using var group = ImGuiRaii.NewGroup();
            ImGui.Text("Fishing mode");
            ImGuiHelper.HoverTooltip("Fishing algorithem");


            var modes = Enum.GetValues(typeof(AutoFishingMode)).Cast<AutoFishingMode>().ToArray();
            var currentMode = (int) GatherBuddy.Config.AutoFishingMode;

            if (ImGui.Combo("", ref currentMode, modes.Select(e => e.ToString()).ToArray(), modes.Length) && currentMode != GatherBuddy.Config.AutoFishingMode) {
                GatherBuddy.Config.AutoFishingMode = (byte) currentMode;
            }
        }

        private static void DrawComboWithFilter(string v1, string v2, bool littleThalaosMode, System.Func<object, bool> p)
        {
            throw new System.NotImplementedException();
        }

        private static void DrawAutoFisherTab()
        {
            using var imgui = new ImGuiRaii();
            if (!imgui.BeginChild("##settingsList", -Vector2.One, true))
                return;

            var inputSize = 125 * _globalScale;

            DrawContinueToFishBox();
            DrawAntiAFKBox();
            DrawLittletThalaosBoxads();
        }
    }
}
