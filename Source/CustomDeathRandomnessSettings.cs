using Verse;

namespace Torann.CustomDeathRandomness
{
    public sealed class CustomDeathRandomnessSettings : ModSettings
    {
        public float AnimalDeathChance = 0.5f;

        public float PawnDeathChance = 0.67f;

        public bool UseStorytellerPopulationIntent;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AnimalDeathChance, "animalDeathChance", 0.5f);
            Scribe_Values.Look(ref PawnDeathChance, "pawnDeathChance", 0.67f);
            Scribe_Values.Look(ref UseStorytellerPopulationIntent, "useStorytellerPopulationIntent");
        }
    }
}