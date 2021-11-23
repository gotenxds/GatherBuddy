using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using GatherBuddy.Enums;
using GatherBuddy.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherBuddy.Classes.AutoFishers
{
    class LittleThalaosFisher : AutoFisher
    {
        private bool isRecasting = false;
        private bool caughtThalaos = false;

        public LittleThalaosFisher() {
            OnFishGetAway += () => {
                AutoFisher.DelayFor(1000, 3000, "Fish gets away, recasting");
                Start();
            };
        }

        public override async void Start()
        {
            if (AutoFisher.IsOn)
            {
                caughtThalaos = false;
                if (AutoFisher.IsMoochIIOnCooldown)
                {
                    if (!isPatienceIIUp()) {
                        AutoFisher._commandManager.Execute($"/ac \"Patience II\"");
                        await AutoFisher.DelayFor(2000, 3000, "Using Patience II");
                        UseHiCordial();
                        UseCordial();
                        await AutoFisher.DelayFor(1000, 2000, "Using Cordial");
                    }
                    base.Start();
                }
                else {
                    base.Start();
                }
            }
        }

        public override async void OnBite()
        {
            if (AutoFisher.IsOn)
            {
                PluginLog.Verbose($"---- Fish bite with {AutoFisher._tugType.Tug} Tug | | Mooching is {IsMooching} ----");
                if (IsMooching) {
                    if (TimeElapsedMilliseconds < 18000 || AutoFisher._tugType.Tug == Enums.BiteType.Weak)
                    {
                        await AutoFisher.DelayFor(7500, 8000, "Wait for fish to let go");
                        Start();
                    }
                    else {
                        HookFish().ContinueWith(t => PluginLog.Verbose("Got Thalaos?!"));
                    }
                }
                else if (AutoFisher._tugType.Tug == Enums.BiteType.Weak)
                {
                    HookFish().ContinueWith(t => PluginLog.Verbose("Fished Ghost faerie"));
                }
                else
                {
                    await AutoFisher.DelayFor(7500, 8000, "Wait for fish to let go");
                    Start();
                }
            }
        }

        public override async void OnCatch()
        {
            if (AutoFisher.IsOn)
            {
                PluginLog.Verbose($"---- Fish Caught {(AutoFisher.isLastFishHQ ? "HQ" : "NQ")} ----");
                if (IsMooching)
                {
                    base.OnCatch();
                }
                else
                {
                    if (AutoFisher.isLastFishHQ)
                    {
                        await AutoFisher.DelayFor(4000, 6000, "Mooch delay");
                        AutoFisher._commandManager.Execute($"/ac Mooch");
                    }
                    else
                    {
                        if (IsMoochIIOnCooldown) {
                            IsMooching = false;
                            Start();
                        }
                        await AutoFisher.DelayFor(1000, 2000, "Mooch II delay");
                        AutoFisher._commandManager.Execute($"/ac \"Mooch II\"");
                    }
                }
            }
        }

        public override void OnFishing()
        {
            if (AutoFisher.IsOn)
            {
/*
                if (inSeconds > TimeElapsedSeconds)
                {
                    PluginLog.Verbose($"Fishing for {TimeElapsedSeconds} - Mooching is {(AutoFisher.IsMooching ? "ON" : "OFF")}");
                }
*/
                if (!isRecasting && !AutoFisher.IsMooching && TimeElapsedMilliseconds >= 14000)
                {
                    Recast();
                }

                base.OnFishing();
            }
        }

        private async void Recast() {
            PluginLog.Verbose($"Recasting rod on {TimeElapsedSeconds} seconds");
            isRecasting = true;
            await FishingAction.Hook.UseAction();
            await AutoFisher.DelayFor(3000, 3750, "Waiting for Hook animation");

            isRecasting = false;

            Start();
        }

        private async Task HookFish()
        {
            if (GatherBuddy.Config.AutoFish)
            {
                var hookAction = "Hook";

                if (isPatienceIIUp())
                {
                    hookAction = _tugType.Tug == Enums.BiteType.Weak ? "Precision Hookset" : "Powerful Hookset";
                }

                await AutoFisher.DelayFor(250, 1250, $"{hookAction} delay");
                AutoFisher._commandManager.Execute($"/ac \"{hookAction}\"");
            }
        }
    }
}
