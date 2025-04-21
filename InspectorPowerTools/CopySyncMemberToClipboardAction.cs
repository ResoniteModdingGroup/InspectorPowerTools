﻿using FrooxEngine;
using MonkeyLoader;
using MonkeyLoader.Meta;
using MonkeyLoader.Resonite;
using MonkeyLoader.Resonite.UI.Inspectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InspectorPowerTools
{
    internal sealed class CopySyncMemberToClipboardAction
        : ResoniteAsyncEventHandlerMonkey<CopySyncMemberToClipboardAction, InspectorMemberActionsMenuItemsGenerationEvent>
    {
        public override int Priority => HarmonyLib.Priority.Normal;

        protected override bool AppliesTo(InspectorMemberActionsMenuItemsGenerationEvent eventData)
            => base.AppliesTo(eventData) && eventData.Target is IField && eventData.Target is not ISyncRef;

        protected override Task Handle(InspectorMemberActionsMenuItemsGenerationEvent eventData)
        {
            var field = (IField)eventData.Target;
            var menuItem = eventData.ContextMenu.AddItem(Mod.GetLocaleString("CopyToClipboard"),
                OfficialAssets.Graphics.Icons.General.Duplicate, RadiantUI_Constants.Sub.GREEN);

            // Context Menu is local user only anyways, no need to use local action button
            menuItem.Button.LocalPressed += (button, _) =>
            {
                button.World.InputInterface.Clipboard.SetText(field.BoxedValue.ToString());
                button.World.LocalUser.CloseContextMenu(eventData.MemberActions);
            };

            return Task.CompletedTask;
        }

        protected override bool OnEngineReady()
        {
            if (Mod.Loader.Get<Mod>().ById("CommunityBugFixCollection") is not null)
            {
                Logger.Info(() => "Skipping in favor of the CommunityBugFixCollection implementation.");
                return true;
            }

            return base.OnEngineReady();
        }
    }
}