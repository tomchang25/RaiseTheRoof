using System;
using System.Linq;
using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace RaiseTheRoof
{
    public class RaiseTheRoofSettings : ModSettings
    {
        public bool autoBuildRoof = true;
        public bool allowInfestations = false;
        public bool removeMountainousRoofEnabled = true;
        public bool steelRoofEnabled = true;
        public bool transparentRoofEnabled = true;
        public bool solarRoofEnabled = true;
        public bool transparentSolarRoofEnabled = true;
        public int chunkChance = 33;
        public int solarPowerOutput = 50;
        public int steelRoofSteelCost = 50;
        public int transparentRoofSteelCost = 50;
        public int transparentRoofSynthreadCost = 10;
        public int solarRoofSteelCost = 50;
        public int solarRoofComponentCost = 1;
        public int transparentSolarRoofSteelCost = 50;
        public int transparentSolarRoofSynthreadCost = 10;
        public int transparentSolarRoofComponentCost = 1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref autoBuildRoof, "autoBuildRoof", true);
            Scribe_Values.Look(ref allowInfestations, "allowInfestations", false);
            Scribe_Values.Look(ref removeMountainousRoofEnabled, "removeMountainousRoofEnabled", true);
            Scribe_Values.Look(ref steelRoofEnabled, "steelRoofEnabled", true);
            Scribe_Values.Look(ref transparentRoofEnabled, "transparentRoofEnabled", true);
            Scribe_Values.Look(ref solarRoofEnabled, "solarRoofEnabled", true);
            Scribe_Values.Look(ref transparentSolarRoofEnabled, "transparentSolarRoofEnabled", true);
            Scribe_Values.Look(ref chunkChance, "chunkChance", 33);
            Scribe_Values.Look(ref solarPowerOutput, "solarPowerOutput", 50);
            Scribe_Values.Look(ref steelRoofSteelCost, "roofSteelCost", 50);
            Scribe_Values.Look(ref transparentRoofSteelCost, "transparentRoofSteelCost", 50);
            Scribe_Values.Look(ref transparentRoofSynthreadCost, "transparentRoofSynthreadCost", 10);
            Scribe_Values.Look(ref solarRoofSteelCost, "solarRoofSteelCost", 50);
            Scribe_Values.Look(ref solarRoofComponentCost, "solarRoofComponentCost", 1);
            Scribe_Values.Look(ref transparentSolarRoofSteelCost, "transparentSolarRoofSteelCost", 50);
            Scribe_Values.Look(ref transparentSolarRoofSynthreadCost, "transparentSolarRoofSynthreadCost", 10);
            Scribe_Values.Look(ref transparentSolarRoofComponentCost, "transparentSolarRoofComponentCost", 1);
        }
    }

    [StaticConstructorOnStartup]
    public class RaiseTheRoofMod : Mod
    {
        public static RaiseTheRoofSettings settings;

        public RaiseTheRoofMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RaiseTheRoofSettings>();
        }
        static void ResetDefaults()
        {
            settings.autoBuildRoof = true;
            settings.allowInfestations = false;
            settings.removeMountainousRoofEnabled = true;
            settings.steelRoofEnabled = true;
            settings.transparentRoofEnabled = true;
            settings.solarRoofEnabled = true;
            settings.transparentSolarRoofEnabled = true;
            settings.chunkChance = 33;
            settings.solarPowerOutput = 50;
            settings.steelRoofSteelCost = 50;
            settings.transparentRoofSteelCost = 50;
            settings.transparentRoofSynthreadCost = 10;
            settings.solarRoofSteelCost = 50;
            settings.solarRoofComponentCost = 1;
            settings.transparentSolarRoofSteelCost = 50;
            settings.transparentSolarRoofSynthreadCost = 10;
            settings.transparentSolarRoofComponentCost = 1;
            settings.Write();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            Color backgroundColor = new Color(0.07f, 0.07f, 0.07f, 1f);
            Rect drawTexRect1 = new Rect(0f, inRect.position.y + 16f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect1, SolidColorMaterials.NewSolidColorTexture(backgroundColor));

            Rect labelRect1 = new Rect(inRect.position.x + 8, inRect.position.y + 16f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect1, "General");

            Rect checkboxRect1 = new Rect(inRect.position.x + 8, inRect.position.y + 48f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec1 = new Vector2(checkboxRect1.x, checkboxRect1.y);
            Widgets.Checkbox(vec1, ref settings.autoBuildRoof);
            Color darkerWhite = new Color(0.75f, 0.75f, 0.75f, 1f);
            GUI.color = darkerWhite;
            Rect labelRect2 = new Rect(inRect.position.x + 40, inRect.position.y + 48f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect2, "Automatically construct roof when room is built");
            GUI.color = Color.white;
            Rect checkboxRect2 = new Rect(inRect.position.x + 8, inRect.position.y + 80f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec2 = new Vector2(checkboxRect2.x, checkboxRect2.y);
            Widgets.Checkbox(vec2, ref settings.allowInfestations);
            GUI.color = darkerWhite;
            Rect labelRect10 = new Rect(inRect.position.x + 40, inRect.position.y + 80f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect10, "Allow infestations to spawn under the new roofs");
            GUI.color = Color.white;

            Rect checkboxRect3 = new Rect(inRect.position.x + 8, inRect.position.y + 112f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec3 = new Vector2(checkboxRect3.x, checkboxRect3.y);
            Widgets.Checkbox(vec3, ref settings.removeMountainousRoofEnabled);
            GUI.color = darkerWhite;
            Rect labelRect11 = new Rect(inRect.position.x + 40, inRect.position.y + 112f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect11, "Remove mountainous roofs enabled");
            GUI.color = Color.white;

            Rect checkboxRect4 = new Rect(inRect.position.x + 8, inRect.position.y + 144f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec4 = new Vector2(checkboxRect4.x, checkboxRect4.y);
            Widgets.Checkbox(vec4, ref settings.steelRoofEnabled);
            GUI.color = darkerWhite;
            Rect labelRect12 = new Rect(inRect.position.x + 40, inRect.position.y + 144f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect12, "Steel roofs enabled");
            GUI.color = Color.white;

            Rect checkboxRect5 = new Rect(inRect.position.x + 8, inRect.position.y + 176f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec5 = new Vector2(checkboxRect5.x, checkboxRect5.y);
            Widgets.Checkbox(vec5, ref settings.transparentRoofEnabled);
            GUI.color = darkerWhite;
            Rect labelRect13 = new Rect(inRect.position.x + 40, inRect.position.y + 176f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect13, "Transparent roofs enabled");
            GUI.color = Color.white;

            Rect checkboxRect6 = new Rect(inRect.position.x + 8, inRect.position.y + 208f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec6 = new Vector2(checkboxRect6.x, checkboxRect6.y);
            Widgets.Checkbox(vec6, ref settings.solarRoofEnabled);
            GUI.color = darkerWhite;
            Rect labelRect14 = new Rect(inRect.position.x + 40, inRect.position.y + 208f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect14, "Solar roofs enabled");
            GUI.color = Color.white;

            Rect checkboxRect7 = new Rect(inRect.position.x + 8, inRect.position.y + 240f, (inRect.width / 2) - 16, inRect.height - 560f);
            Vector2 vec7 = new Vector2(checkboxRect7.x, checkboxRect7.y);
            Widgets.Checkbox(vec7, ref settings.transparentSolarRoofEnabled);
            GUI.color = darkerWhite;
            Rect labelRect15 = new Rect(inRect.position.x + 40, inRect.position.y + 240f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect15, "Transparent solar roofs enabled");
            GUI.color = Color.white;

            Rect drawTexRect6 = new Rect(0f, inRect.position.y + 304f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect6, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect7 = new Rect(inRect.position.x + 8f, inRect.position.y + 304f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect7, "Misc");

            Rect sliderRect9 = new Rect(inRect.position.x + 8f, inRect.position.y + 352f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.solarPowerOutput = (int)Widgets.HorizontalSlider(sliderRect9, settings.solarPowerOutput, 1f, 500f, false, null, "Solar roof power output", settings.solarPowerOutput.ToString() + " W", -1);
            Rect sliderRect10 = new Rect(inRect.position.x + 8f, inRect.position.y + 384f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.chunkChance = (int)Widgets.HorizontalSlider(sliderRect10, settings.chunkChance, 1f, 100f, false, null, "Chunk mined from overhead mountain removal chance", settings.chunkChance.ToString() + "%", -1);

            Rect drawTexRect7 = new Rect(0f, inRect.position.y + 432f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect7, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect8 = new Rect(inRect.position.x + 8f, inRect.position.y + 432f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect8, "Settings");

            Rect labelRect9 = new Rect(inRect.position.x + 8f, inRect.position.y + 464f, (inRect.width / 2) - 48, inRect.height - 560f);
            Widgets.Label(labelRect9, "Restore default settings");
            Rect buttonTextRect1 = new Rect(inRect.position.x + 296f, inRect.position.y + 464f, (inRect.width / 2) - 304, inRect.height - 560f);
            bool bReset = Widgets.ButtonText(buttonTextRect1, "Reset", true);
            if (bReset)
            {
                ResetDefaults();
            }

            Rect drawTexRect2 = new Rect(inRect.position.x + 440f, inRect.position.y + 16f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect2, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect3 = new Rect(inRect.position.x + 448f, inRect.position.y + 16f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect3, "Steel roof material costs");

            Rect sliderRect1 = new Rect(inRect.position.x + 448f, inRect.position.y + 64f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.steelRoofSteelCost = (int)Widgets.HorizontalSlider(sliderRect1, settings.steelRoofSteelCost, 1f, 200f, false, null, "Steel", settings.steelRoofSteelCost.ToString(), -1);

            Rect drawTexRect3 = new Rect(inRect.position.x + 440f, inRect.position.y + 112f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect3, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect4 = new Rect(inRect.position.x + 448f, inRect.position.y + 112f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect4, "Transparent roof material costs");

            Rect sliderRect2 = new Rect(inRect.position.x + 448f, inRect.position.y + 160f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.transparentRoofSteelCost = (int)Widgets.HorizontalSlider(sliderRect2, settings.transparentRoofSteelCost, 1f, 200f, false, null, "Steel", settings.transparentRoofSteelCost.ToString(), -1);
            Rect sliderRect3 = new Rect(inRect.position.x + 448f, inRect.position.y + 192f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.transparentRoofSynthreadCost = (int)Widgets.HorizontalSlider(sliderRect3, settings.transparentRoofSynthreadCost, 1f, 100f, false, null, "Synthread", settings.transparentRoofSynthreadCost.ToString(), -1);

            Rect drawTexRect4 = new Rect(inRect.position.x + 440f, inRect.position.y + 240f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect4, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect5 = new Rect(inRect.position.x + 448f, inRect.position.y + 240f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect5, "Solar roof material costs");

            Rect sliderRect4 = new Rect(inRect.position.x + 448f, inRect.position.y + 288f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.solarRoofSteelCost = (int)Widgets.HorizontalSlider(sliderRect4, settings.solarRoofSteelCost, 1f, 200f, false, null, "Steel", settings.solarRoofSteelCost.ToString(), -1);
            Rect sliderRect5 = new Rect(inRect.position.x + 448f, inRect.position.y + 320f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.solarRoofComponentCost = (int)Widgets.HorizontalSlider(sliderRect5, settings.solarRoofComponentCost, 1f, 10f, false, null, "Components", settings.solarRoofComponentCost.ToString(), -1);

            Rect drawTexRect5 = new Rect(inRect.position.x + 440f, inRect.position.y + 368f, (inRect.width / 2) - 8, inRect.height - 560f);
            GUI.DrawTexture(drawTexRect5, SolidColorMaterials.NewSolidColorTexture(backgroundColor));
            Rect labelRect6 = new Rect(inRect.position.x + 448f, inRect.position.y + 368f, (inRect.width / 2) - 16, inRect.height - 560f);
            Widgets.Label(labelRect6, "Transparent solar roof material costs");

            Rect sliderRect6 = new Rect(inRect.position.x + 448f, inRect.position.y + 416f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.transparentSolarRoofSteelCost = (int)Widgets.HorizontalSlider(sliderRect6, settings.transparentSolarRoofSteelCost, 1f, 200f, false, null, "Steel", settings.transparentSolarRoofSteelCost.ToString(), -1);
            Rect sliderRect7 = new Rect(inRect.position.x + 448f, inRect.position.y + 448f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.transparentSolarRoofSynthreadCost = (int)Widgets.HorizontalSlider(sliderRect7, settings.transparentSolarRoofSynthreadCost, 1f, 100f, false, null, "Synthread", settings.transparentSolarRoofSynthreadCost.ToString(), -1);
            Rect sliderRect8 = new Rect(inRect.position.x + 448f, inRect.position.y + 480f, (inRect.width / 2) - 20, inRect.height - 560f);
            settings.transparentSolarRoofComponentCost = (int)Widgets.HorizontalSlider(sliderRect8, settings.transparentSolarRoofComponentCost, 1f, 10f, false, null, "Components", settings.transparentSolarRoofComponentCost.ToString(), -1);

            RTRUtils.ApplySettings();
            settings.Write();
        }

        public override string SettingsCategory() => "Raise The Roof Settings";
    }
}