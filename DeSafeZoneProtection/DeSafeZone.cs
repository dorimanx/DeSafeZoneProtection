using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using SpaceEngineers.Game.Entities.Blocks.SafeZone;
using VRage.Game;
using VRage.Game.ObjectBuilders.Components;
using VRageMath;

namespace DeSafeZoneProtection
{
    public class DeSafeZone
    {
        private int tick = 0;
        private MySafeZoneBlock instance;

        public DeSafeZone(MySafeZoneBlock instance) => this.instance = instance;

        internal void UpdateBlock()
        {
            if (!DeSafeZoneProtectionPlugin.Instance.Config.Enabled || instance == null || ++tick < 5)
                return;

            tick = 0;
            IEnumerable<MyPlayer> myPlayers = MySession.Static.Players.GetOnlinePlayers().Where(p => p.Controller?.ControlledEntity != null).Where(e => Vector3D.Distance(e.Controller.ControlledEntity.Entity.PositionComp.GetPosition(), instance.PositionComp.GetPosition()) < DeSafeZoneProtectionPlugin.Instance.Config.Distance);
            bool flag = false;
            foreach (MyPlayer myPlayer in myPlayers)
            {
                if (myPlayer.GetRelationTo(instance.OwnerId) != MyRelationsBetweenPlayerAndBlock.Enemies)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                MySafeZone mySafeZone = MySessionComponentSafeZones.SafeZones.ToList().Find(z => z.SafeZoneBlockId == instance.EntityId);
                if (mySafeZone == null || !mySafeZone.Enabled)
                    return;
                if (!DeSafeZoneProtectionPlugin.Instance.Cache.Cache.ContainsKey(instance.CubeGrid.EntityId))
                    DeSafeZoneProtectionPlugin.Instance.Cache.Cache.Add(instance.CubeGrid.EntityId, Enum.Format(typeof(MySafeZoneAction), mySafeZone.AllowedActions, "d"));
                DeSafeZoneConfig config = DeSafeZoneProtectionPlugin.Instance.Config;
                mySafeZone.AllowedActions = (MySafeZoneAction)(
                    (!config.Damage ? 1 : 0) |
                    (!config.Shooting ? 2 : 0) |
                    (!config.Drilling ? 4 : 0) |
                    (!config.Welding ? 8 : 0) |
                    (!config.Grinding ? 16 : 0) |
                    (!config.VoxelHands ? 32 : 0) |
                    (!config.Building ? 64 : 0) |
                    (!config.LandingGearsLock ? 128 : 0) |
                    (!config.ConvertToStation ? 256 : 0) |
                    (!config.BuildingProjections ? 512 : 0));

                MySessionComponentSafeZones.UpdateSafeZone((MyObjectBuilder_SafeZone)mySafeZone.GetObjectBuilder(false), true);
            }
            else if (DeSafeZoneProtectionPlugin.Instance.Cache.Cache.ContainsKey(instance.CubeGrid.EntityId))
            {
                string str = DeSafeZoneProtectionPlugin.Instance.Cache.Cache[instance.CubeGrid.EntityId];
                DeSafeZoneProtectionPlugin.Instance.Cache.Cache.Remove(instance.CubeGrid.EntityId);
                if (Enum.TryParse(str, out MySafeZoneAction result))
                {
                    MySafeZone mySafeZone = MySessionComponentSafeZones.SafeZones.ToList().Find(z => z.SafeZoneBlockId == instance.EntityId);
                    if (mySafeZone == null || !mySafeZone.Enabled)
                        return;
                    mySafeZone.AllowedActions = result;
                    MySessionComponentSafeZones.UpdateSafeZone((MyObjectBuilder_SafeZone)mySafeZone.GetObjectBuilder(false), true);
                }
                else
                    DeSafeZoneProtectionPlugin.Log.Fatal("Error parsing actions from block storage! " + str);
            }
        }
    }
}
