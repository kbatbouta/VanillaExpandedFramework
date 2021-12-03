﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFECore.SoundTest
{
    internal class EditWindow_SoundTest : EditWindow
    {
        private Vector2 scrollPosition;
        private Vector2 scrollPositionBis;
        private string search = "";
        private List<SoundDef> soundDefs;
        private SoundDef soundToTest;

        public override Vector2 InitialSize => new Vector2(UI.screenWidth * 0.5f, UI.screenHeight * 0.75f);
        public override bool IsDebug => true;

        public EditWindow_SoundTest()
        {
            this.resizeable = false;
            this.draggable = false;
            this.preventCameraMotion = false;
            this.doCloseX = true;
        }

        public override void PostOpen()
        {
            base.PostOpen();
            this.soundDefs = DefDatabase<SoundDef>.AllDefsListForReading.FindAll(s => !s.modContentPack.IsOfficialMod && !s.modContentPack.IsCoreMod);
            this.soundToTest = null;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (soundToTest == null)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                float num = 0f;

                // Search bar
                Rect labelRect = new Rect(0f, 10f + num, 150f, 30f);
                Widgets.Label(labelRect, "Search:");
                Rect searchRect = new Rect(labelRect.width + 10f, 10f + num, inRect.width - labelRect.width - 116f, 30f);
                search = Widgets.TextField(searchRect, search);
                num += 50f;
                // View rect
                Rect soundsRect = new Rect(0f, 10f + num, inRect.width, inRect.height - 65f);

                List<SoundDef> searchResult = soundDefs.FindAll(s => s.defName.ToLower().Contains(search.ToLower())).ToList();
                Rect soundsViewRect = new Rect(0f, 10f + num, inRect.width - 16f, 30f * (Mathf.RoundToInt(searchResult.Count / 3) + 1));

                Widgets.BeginScrollView(soundsRect, ref scrollPosition, soundsViewRect);
                int bCount = 0;
                int line = 0;

                for (int i = 0; i < searchResult.Count; i++)
                {
                    Rect button = new Rect(bCount * soundsViewRect.width / 3, 10f + num + (line * 30f), soundsViewRect.width / 3, 30f);
                    if (Widgets.ButtonText(button, searchResult[i].defName))
                    {
                        soundToTest = searchResult[i];
                        break;
                    }
                    bCount++;
                    if (bCount % 3 == 0)
                    {
                        bCount = 0;
                        line++;
                    }
                }
                Widgets.EndScrollView();
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                float num = 0f;

                Rect changeButton = new Rect(0f, 20f + num, inRect.width, 30f);
                if (Widgets.ButtonText(changeButton, $"Change sound (currently testing: {soundToTest.defName})"))
                {
                    soundToTest = null;
                }
                else
                {
                    Text.Anchor = TextAnchor.UpperLeft;
                    num += 40f;
                    Rect subSoundsRect = new Rect(0f, 10f + num, inRect.width, inRect.height - 115f);
                    Rect subSoundsViewRect = new Rect(0f, 10f + num, inRect.width - 16f, 200f * soundToTest.subSounds.Count);
                    Widgets.BeginScrollView(subSoundsRect, ref scrollPositionBis, subSoundsViewRect);
                    for (int i = 0; i < soundToTest.subSounds.Count; i++)
                    {
                        Rect volumeRangeLabel = new Rect(0f, 20f + num, inRect.width, 30f);
                        Widgets.Label(volumeRangeLabel, $"Subsound {i + 1} <volumeRange>");
                        num += 30f;
                        Rect volumeRangeButton = new Rect(0f, 20f + num, inRect.width, 30f);
                        Widgets.FloatRange(volumeRangeButton, Rand.Int, ref soundToTest.subSounds[i].volumeRange, 0.5f, 500, valueStyle: ToStringStyle.FloatThree);
                        num += 50f;
                        Rect pitchRangeLabel = new Rect(0f, 20f + num, inRect.width, 30f);
                        Widgets.Label(pitchRangeLabel, $"Subsound {i + 1} <pitchRange>");
                        num += 30f;
                        Rect pitchRangeButton = new Rect(0f, 20f + num, inRect.width, 30f);
                        Widgets.FloatRange(pitchRangeButton, Rand.Int, ref soundToTest.subSounds[i].pitchRange, 0, 100, valueStyle: ToStringStyle.FloatThree);
                        num += 50f;

                        Text.Anchor = TextAnchor.MiddleCenter;
                        Rect saveButton = new Rect(0f, 20f + num, inRect.width, 30f);
                        if (Widgets.ButtonText(saveButton, $"Copy settings of subsound {i + 1}"))
                        {
                            GUIUtility.systemCopyBuffer = $"<volumeRange>{soundToTest.subSounds[i].volumeRange}</volumeRange>\n<pitchRange>{soundToTest.subSounds[i].pitchRange}</pitchRange>";
                        }
                        num += 30f;
                        Text.Anchor = TextAnchor.UpperLeft;
                    }
                    Widgets.EndScrollView();

                    Text.Anchor = TextAnchor.MiddleCenter;
                    Rect playButton = new Rect(0f, inRect.height - 50f, inRect.width, 30f);
                    if (Widgets.ButtonText(playButton, $"Play sound"))
                    {
                        if (soundToTest.subSounds.Any(sub => sub.onCamera)) soundToTest.PlayOneShotOnCamera();
                        else soundToTest.PlayOneShot((SoundInfo)new TargetInfo(Find.CurrentMap.Center, Find.CurrentMap));
                    }
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
