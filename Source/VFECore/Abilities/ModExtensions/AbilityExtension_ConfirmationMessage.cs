﻿namespace VFECore.Abilities
{
    using RimWorld;
    using System;
    using Verse;

    public class AbilityExtension_ConfirmationMessage : AbilityExtension_AbilityMod
	{
		public string message;
        public override void PreCast(LocalTargetInfo target, Ability ability, ref bool startAbilityJobImmediately, Action startJobAction)
        {
            base.PreCast(target, ability, ref startAbilityJobImmediately, startJobAction);
            startAbilityJobImmediately = false;
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(message.Formatted(ability.pawn.Named("PAWN")), startJobAction));
        }
    }
}