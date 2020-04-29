using HarmonyLib;
using Helpers;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(Mission), "CreateBlow")]
    public class CreateBlowPatch
    {        
        static void Postfix(ref Blow __result,
            Agent attackerAgent,
            Agent victimAgent,
            ref AttackCollisionData collisionData,
            CrushThroughState cts,
            Vec3 blowDir,
            Vec3 swingDir,
            bool cancelDamage)
        {
            if (SubModule.Settings.Battle_WarStomp_UnstoppableCharge)
            {
                __result.BlowFlag &= ~BlowFlags.MakesRear;
            }
            if (collisionData.IsHorseCharge && SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier != 1f)
            {
                if (victimAgent.IsRunningAway || (double)Vec3.DotProduct(swingDir, victimAgent.Frame.rotation.f) > 0.5)
                {
                    __result.InflictedDamage = (int)(__result.InflictedDamage * SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier);
                }
            }
        }

        static bool Prepare()
        {
            return SubModule.Settings.Battle_WarStomp;
        }
    }
}
