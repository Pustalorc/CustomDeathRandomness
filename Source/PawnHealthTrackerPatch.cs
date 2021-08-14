using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using Verse;
#if V10
using Harmony;
#else
using HarmonyLib;

#endif

namespace Torann.CustomDeathRandomness
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    [UsedImplicitly]
    public static class PawnHealthTrackerPatch
    {
        [UsedImplicitly]
        public static float GetAnimalDeathChance()
        {
            return CustomDeathRandomnessMod.Instance.Settings.AnimalDeathChance;
        }

        [UsedImplicitly]
        public static float GetPawnDeathChance(float popIntent)
        {
            var settings = CustomDeathRandomnessMod.Instance.Settings;

            if (!settings.UseStorytellerPopulationIntent)
                popIntent = 1f;

            return popIntent * settings.PawnDeathChance;
        }

        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var animalReplace = false;
            var pawnReplace = false;
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (var instruction in instructionsList)
            {
                switch (instruction.operand)
                {
                    case MethodInfo mInfo when mInfo == typeof(RaceProperties).GetMethod("get_Animal"):
                        animalReplace = true;
                        break;
                    case FieldInfo fInfo when fInfo ==
                                              typeof(HealthTuning).GetField(
                                                  "DeathOnDownedChance_NonColonyHumanlikeFromPopulationIntentCurve"):
                        pawnReplace = true;
                        break;
                }

                if (animalReplace && instruction.opcode == OpCodes.Ldc_R4)
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(PawnHealthTrackerPatch).GetMethod("GetAnimalDeathChance"));

                    animalReplace = false;
                }
                else if (pawnReplace)
                {
                    var isMul = instruction.opcode == OpCodes.Mul;
                    yield return instruction;

                    if (!isMul)
                        continue;

                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(PawnHealthTrackerPatch).GetMethod("GetPawnDeathChance"));

                    pawnReplace = false;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}