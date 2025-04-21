﻿using FrooxEngine;
using HarmonyLib;
using MonkeyLoader;
using MonkeyLoader.Meta;
using MonkeyLoader.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Taken from ComponentSelectorAdditions
// https://github.com/ResoniteModdingGroup/ComponentSelectorAdditions

namespace InspectorPowerTools
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(CaseInsensitiveCustomGenerics))]
    internal sealed class CaseInsensitiveCustomGenerics : Monkey<CaseInsensitiveCustomGenerics>
    {
        public override bool CanBeDisabled => true;

        protected override bool OnLoaded()
        {
            if (!Enabled)
                return true;

            if (Mod.Loader.Get<Mod>().ById("ComponentSelectorAdditions") is not null || Mod.Loader.Get<Mod>().ById("CommunityBugFixCollection") is not null)
            {
                Logger.Info(() => "Skipping in favor of the ComponentSelectorAdditions / CommunityBugFixCollection fix.");
                return true;
            }

            GlobalTypeRegistry._nameToSystemType = new(GlobalTypeRegistry._nameToSystemType, StringComparer.OrdinalIgnoreCase);
            GlobalTypeRegistry._byName = new(GlobalTypeRegistry._byName, StringComparer.OrdinalIgnoreCase);

            return base.OnLoaded();
        }

        private static void Postfix(AssemblyTypeRegistry __instance)
        {
            __instance._typesByFullName = new(__instance._typesByFullName, StringComparer.OrdinalIgnoreCase);
            __instance._typesByName.dictionary = new(__instance._typesByName.dictionary, StringComparer.OrdinalIgnoreCase);
            __instance._movedTypes = new(__instance._movedTypes, StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<MethodBase> TargetMethods()
            => AccessTools.GetDeclaredConstructors(typeof(AssemblyTypeRegistry), false);
    }
}