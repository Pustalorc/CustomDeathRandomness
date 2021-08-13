using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Verse;
#if V10
using Harmony;
#else
using HarmonyLib;
#endif

namespace Torann.CustomDeathRandomness
{
    [UsedImplicitly]
    public class CustomDeathRandomnessMod : Mod
    {
        public static CustomDeathRandomnessMod Instance { get; private set; }

        public CustomDeathRandomnessSettings Settings { get; }

#if V10
        public HarmonyInstance HarmonyInstance { get; }
#else
        public Harmony HarmonyInstance { get; }
#endif

        public CustomDeathRandomnessMod(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<CustomDeathRandomnessSettings>();
#if V10
            HarmonyInstance = HarmonyInstance.Create("CustomDeathRandomness.Torann");
#else
            HarmonyInstance = new Harmony("CustomDeathRandomness.Torann");
#endif
            HarmonyInstance.PatchAll(Assembly.GetCallingAssembly());
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            var listingStandard = new Listing_Standard {ColumnWidth = canvas.width / 2f};

            listingStandard.Begin(canvas);

            listingStandard.Gap();

            listingStandard.CheckboxLabeled("CDR_UseStorytellerModifier".Translate(), ref Settings.UseStorytellerPopulationIntent, "CDR_UseStorytellerModifierTooltip".Translate());

            listingStandard.Label("CDR_AnimalDeathRate".Translate(Settings.AnimalDeathChance));
            GUI.contentColor = Color.yellow;
            Settings.AnimalDeathChance = Widgets.HorizontalSlider(listingStandard.GetRect(20f), Settings.AnimalDeathChance, 0, 100, false, $"{Settings.AnimalDeathChance}%", "0%", "100%", 1f);
            listingStandard.Gap();

            GUI.contentColor = Color.white;
            listingStandard.Label("CDR_PawnDeathRate".Translate(Settings.PawnDeathChance));
            GUI.contentColor = Color.yellow;
            Settings.PawnDeathChance = Widgets.HorizontalSlider(listingStandard.GetRect(20f), Settings.PawnDeathChance, 0, 100, false, $"{Settings.PawnDeathChance}%", "0%", "100%", 1f);
            listingStandard.Gap();

            listingStandard.ColumnWidth = canvas.width / 4f;

            if (listingStandard.ButtonText("CDR_Reset".Translate()))
            {
                Settings.AnimalDeathChance = 0.5f;
                Settings.PawnDeathChance = 0.67f;
                Settings.UseStorytellerPopulationIntent = true;
            }

            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "Custom Death Randomness";
        }
    }
}
