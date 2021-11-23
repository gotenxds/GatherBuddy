using Dalamud.Logging;
using GatherBuddy.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherBuddy.Classes.AutoFishers
{
    class DefaultAutoFisher : AutoFisher
    {
        public override void OnBite() {
            HookFish().ContinueWith(t => PluginLog.Verbose("Finished Hooking fish."));
        }

        public override async void OnCatch()
        {
            if (GatherBuddy.Config.AutoFish)
            {
                await DelayFor(500, 2000, "Chum delay");
                AutoFisher._commandManager.Execute($"/ac Chum");
            }

            base.OnCatch();
        }

        private async Task HookFish()
        {
            if (GatherBuddy.Config.AutoFish)
            {
                await AutoFisher.DelayFor(500, 2000, "Hook delay");
                AutoFisher._commandManager.Execute($"/ac Hook");
            }
        }
    }
}
