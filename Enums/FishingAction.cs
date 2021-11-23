using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherBuddy.Enums
{
    public enum FishingAction : int
    {
        Cast = 289,
        Hook = 296,
    }


    public static class FishingActionExtensions
    {
        private static unsafe void UseActionUnsafe(uint id)
        {
            ActionManager.Instance()->UseAction(ActionType.Spell, id);
        }

        private static unsafe bool IsActionCastableUnsafe(uint id)
        {
            return ActionManager.Instance()->GetActionStatus(ActionType.Spell, id) == 0;
        }


        public static async Task<bool> UseAction(this FishingAction action, bool repeat = false, int repeatCount = 10, int repeatOffset = 1000)
        {
            PluginLog.Verbose($"$$ Trying to use action {action} $$");

            if (!IsActionCastableUnsafe((uint)action))
            {
                PluginLog.Verbose($"$$ Action {action} is not currently castable {(repeat ? $", retrying in {repeatOffset / 1000}s" : "")}$$");

                if (repeat)
                {
                    if (repeatCount > 0)
                    {
                        PluginLog.Verbose($"$$ Repeat number ${repeatCount} $$");
                        await Task.Delay(repeatOffset);
                        return await UseAction(action, repeat, repeatCount - 1, repeatOffset);
                    }
                    else {
                        PluginLog.Verbose($"$$ Could not Use action {action} $$");
                        return false;
                    }
                }
                else
                {
                    PluginLog.Verbose($"$$ Could not Use action {action} $$");
                    return false;
                }
            }
            else {
                PluginLog.Verbose($"$$ Action {action} is ready, casting...$$");
                UseActionUnsafe((uint)action);

                return true;
            }
        }
    }
}