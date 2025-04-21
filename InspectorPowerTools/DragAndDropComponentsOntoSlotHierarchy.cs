using FrooxEngine;
using HarmonyLib;
using MonkeyLoader.Resonite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPowerTools
{
    [HarmonyPatch(typeof(SlotInspector), nameof(SlotInspector.OnChanges))]
    [HarmonyPatchCategory(nameof(DragAndDropComponentsOntoSlotHierarchy))]
    internal sealed class DragAndDropComponentsOntoSlotHierarchy : ResoniteMonkey<DragAndDropComponentsOntoSlotHierarchy>
    {
        public override bool CanBeDisabled => true;

        private static void AddComponentReceiver(SlotRecord record)
            => record.Slot.AttachComponent<SlotComponentReceiver>().Target.Target = record.TargetSlot;

        private static void Postfix(SlotInspector __instance, bool __state)
        {
            // State already includes Enabled check
            if (!__state)
                return;

            __instance.Slot.ForeachComponentInChildren<SlotRecord>(AddComponentReceiver, cacheItems: true);
        }

        private static void Prefix(SlotInspector __instance, out bool __state)
            => __state = Enabled && __instance.World.IsAuthority && __instance._rootSlot.Target != __instance._setupRoot;
    }
}