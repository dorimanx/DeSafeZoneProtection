using System;
using System.Collections.Generic;
using System.Reflection;
using DeSafeZoneProtection.CoolDown;
using NLog;
using SpaceEngineers.Game.Entities.Blocks.SafeZone;
using Torch.Managers.PatchManager;

namespace DeSafeZoneProtection
{
    [PatchShim]
    public static class SafeZonePatch
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static Dictionary<MySafeZoneBlock, DeSafeZone> cachedSafeZones = new Dictionary<MySafeZoneBlock, DeSafeZone>();
        public static readonly Dictionary<long, DateTime> cooldowns = new Dictionary<long, DateTime>();
        private static bool TimerStarted = false;
        private static bool LoadTimerStarted = false;
        private static readonly SteamIdCooldownKey SaveLoopRequestID = new SteamIdCooldownKey(76000000000000001);
        private static readonly SteamIdCooldownKey LoadLoopRequestID = new SteamIdCooldownKey(76000000000000002);

        public static void Patch(PatchContext ctx)
        {
            ctx.GetPattern(typeof(MySafeZoneBlock).GetMethod("UpdateAfterSimulation100", BindingFlags.Public | BindingFlags.Instance)).
                Prefixes.Add(typeof(SafeZonePatch).GetMethod(nameof(UpdateSafeZone), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
            Log.Info("Patching Successful!");
        }

        private static void UpdateSafeZone(MySafeZoneBlock __instance)
        {
            if (!DeSafeZoneProtectionPlugin.Instance.Config.Enabled)
                return;

            if (__instance == null || __instance.MarkedForClose || __instance.Closed || !__instance.IsWorking)
                return;

            try
            {
                if (cachedSafeZones.ContainsKey(__instance))
                    cachedSafeZones[__instance].UpdateBlock();
                else
                    cachedSafeZones.Add(__instance, new DeSafeZone(__instance));

                if (!TimerStarted)
                {
                        CooldownManager.StartCooldown(SaveLoopRequestID, null, 240000);
                        TimerStarted = true;
                }

                _ = CooldownManager.CheckCooldown(SaveLoopRequestID, null, out var remainingSecondsSave);

                if (remainingSecondsSave < 2)
                {
                    DeSafeZoneProtectionPlugin.Instance.Cache.Save();
                    TimerStarted = false;
                    CooldownManager.StartCooldown(LoadLoopRequestID, null, 25000);
                    LoadTimerStarted = true;
                }

                if (LoadTimerStarted)
                {
                    _ = CooldownManager.CheckCooldown(LoadLoopRequestID, null, out var remainingSecondsload);

                    if (remainingSecondsload < 2)
                    {
                        DeSafeZoneProtectionPlugin.Instance.Cache.Load();
                        LoadTimerStarted = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
