using FrooxEngine;
using HarmonyLib;
using MonkeyLoader.Resonite;

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

            // Lenient allocating user check on children to allow it with MyInspectors as well.
            // Checking on the instance wouldn't be correct, since that could've been created by anyone -
            // so we add the SlotComponentReceivers whenever we just created the updated slots.
            // This will only be the case when we're the Authority (host), or we locally updated the inspector's content.
            if (!__instance.Slot.Children.FirstOrDefault().TryGetAllocatingUser(out var user) || !user.IsLocalUser)
                return;

            __instance.Slot.ForeachComponentInChildren<SlotRecord>(AddComponentReceiver, cacheItems: true);
        }

        private static void Prefix(SlotInspector __instance, out bool __state)
            => __state = Enabled && __instance._rootSlot.Target != __instance._setupRoot;
    }
}