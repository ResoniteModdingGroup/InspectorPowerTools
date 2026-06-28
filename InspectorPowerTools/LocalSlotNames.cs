using FrooxEngine;
using HarmonyLib;
using MonkeyLoader.Resonite;

namespace InspectorPowerTools
{
    [HarmonyPatchCategory(nameof(LocalSlotNames))]
    [HarmonyPatch(typeof(SlotInspector), nameof(SlotInspector.UpdateText))]
    internal sealed class LocalSlotNames : ResoniteMonkey<LocalSlotNames>
    {
        public override bool CanBeDisabled => true;

        private static void Prefix(SlotInspector __instance)
        {
            if (!Enabled)
                return;

            if (__instance._slotNameText.Target.Content.IsDriven)
                return;

            if (!__instance.Slot.TryGetAllocatingUser(out var user) || !user.IsLocalUser)
                return;

            __instance._slotNameText.Target.Content.DriveFrom(__instance._slotNameText.Target.Content, true);
        }
    }
}