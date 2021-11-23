using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using GatherBuddy.Enums;
using GatherBuddy.Game;
using GatherBuddy.Managers;
using GatherBuddy.SeFunctions;
using GatherBuddy.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherBuddy.Classes.AutoFishers
{
    abstract class AutoFisher : IDisposable
    {
        private static XivChatType FishingCatchMessage = (XivChatType)2115;
        private static Stopwatch _timer;

        static protected readonly int CORDIAL_KEY = 0x35; // 5
        static protected readonly int HI_CORDIAL_KEY = 0x36; // 6
        static protected Random rand = new();
        static protected ActionManager _actionManager = new();
        static protected CommandManager _commandManager = new(Dalamud.SigScanner);
        static protected SeTugType _tugType = new(Dalamud.SigScanner);
        static protected Action? OnFishGetAway;
        static protected bool IsOn => GatherBuddy.Config.AutoFish;
        protected static unsafe bool IsMoochIIOnCooldown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, 268);
        protected static unsafe float MoochIIOnCooldownTimeLeft => ActionManager.Instance()->GetRecastTime(ActionType.Spell, 268) - ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, 268);
        protected static long TimeElapsedMilliseconds => Timer.ElapsedMilliseconds;
        protected static int TimeElapsedSeconds => (int)(Timer.ElapsedMilliseconds / 1000);

        public static Stopwatch Timer
        {
            get => _timer;
            set => _timer = value;
        }

        protected static bool isPatienceIIUp()
        {
            if (Dalamud.ClientState.LocalPlayer?.StatusList == null)
            {
                return false;
            }

            // 764 Inefficient hooking
            // 850 Gathering Fortune up
            return Dalamud.ClientState.LocalPlayer.StatusList.Any(status => status.StatusId == 764);
        }

        static AutoFisher()
        {
            Dalamud.Chat.ChatMessage += OnChatMessage;
        }

        protected static bool isLastFishHQ = false;

        private static bool _isMooching = false;
        public static bool IsMooching
        {
            get => _isMooching;
            set => _isMooching = value;
        }

        protected static unsafe void UseHiCordial() {
            PluginLog.Verbose("&&& USING HI-CORDIAL &&&");
            KeyboardUtil.Press(HI_CORDIAL_KEY);
        }
        protected static unsafe void UseCordial() {
            PluginLog.Verbose("&&& USING CORDIAL &&&");
            KeyboardUtil.Press(CORDIAL_KEY);
        }

        protected static Task DelayFor(int min, int max, string reason)
        {
            var delay = rand.Next(min, max);
            PluginLog.Verbose($"*-*-* {reason} : {delay} *-*-*");

            return Task.Delay(delay);
        }

        public virtual void Start()
        {
            if (GatherBuddy.Config.AutoFish)
            {
                IsMooching = false;

                FishingAction.Cast.UseAction(true);
            }
        }

        public virtual void OnBite() { }

        public virtual async void OnCatch()
        {
            IsMooching = false;

            if (GatherBuddy.Config.AutoFish)
            {
                await AutoFisher.DelayFor(2500, 3100, "Cast delay");
                AutoFisher._commandManager.Execute($"/ac Cast");
            }
        }

        public unsafe virtual void OnFishing()
        {
        }

        private static void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type == FishingCatchMessage && message.TextValue.StartsWith("You land a") && message.TextValue.EndsWith("ilms!"))
            {
                if (message.TextValue.Contains(""))
                {
                    PluginLog.Verbose($"Caught fish is high quality!");
                    isLastFishHQ = true;
                }
                else
                {
                    PluginLog.Verbose($"Caught fish is normal quality!");
                    isLastFishHQ = false;
                }

                PluginLog.Verbose($"{type} : {message}");
            } else if (message.TextValue == "The fish gets away...")
            {
                OnFishGetAway?.Invoke();
            }
        }

        public void Dispose()
            => Dalamud.Chat.ChatMessage -= OnChatMessage;
    }
}
