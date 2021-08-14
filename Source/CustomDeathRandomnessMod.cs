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
            HarmonyInstance.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "CheckForStateChange"),
                transpiler: new HarmonyMethod(typeof(PawnHealthTrackerPatch), "Transpiler"));
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            var listingStandard = new Listing_Standard {ColumnWidth = canvas.width / 2f};

            listingStandard.Begin(canvas);

            listingStandard.Gap();

            listingStandard.CheckboxLabeled("CDR_UseStorytellerModifier".Translate(),
                ref Settings.UseStorytellerPopulationIntent, "CDR_UseStorytellerModifierTooltip".Translate());

            var animalDeathValue = Settings.AnimalDeathChance * 100f;
            listingStandard.Label("CDR_AnimalDeathRate".Translate(animalDeathValue));
            GUI.contentColor = Color.yellow;
            Settings.AnimalDeathChance = Widgets.HorizontalSlider(listingStandard.GetRect(20f), animalDeathValue, 0,
                100, false, $"{animalDeathValue}%", "0%", "100%", 1f) / 100f;
            listingStandard.Gap();
            GUI.contentColor = Color.white;

            var pawnDeathValue = Settings.PawnDeathChance * 100f;
            listingStandard.Label("CDR_PawnDeathRate".Translate(pawnDeathValue));
            GUI.contentColor = Color.yellow;
            Settings.PawnDeathChance = Widgets.HorizontalSlider(listingStandard.GetRect(20f), pawnDeathValue, 0, 100,
                false, $"{pawnDeathValue}%", "0%", "100%", 1f) / 100f;
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