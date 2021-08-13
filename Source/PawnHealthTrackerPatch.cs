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
    internal static class PawnHealthTrackerPatch
    {
        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var settings = CustomDeathRandomnessMod.Instance.Settings;
            var animalReplace = false;
            var pawnReplace = false;
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (var instruction in instructionsList)
            {
                switch (instruction.operand)
                {
                    case MethodInfo mInfo when mInfo == typeof(RaceProperties).GetMethod("get_IsMechanoid"):
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
                    yield return new CodeInstruction(OpCodes.Ldc_R4, settings.AnimalDeathChance);

                    animalReplace = false;
                }
                else if (pawnReplace)
                {
                    var isMul = instruction.opcode != OpCodes.Mul;
                    var value = settings.PawnDeathChance;

                    if (!isMul)
                    {
                        if (settings.UseStorytellerPopulationIntent)
                            yield return instruction;

                        continue;
                    }

                    yield return new CodeInstruction(OpCodes.Ldc_R4, value);

                    if (settings.UseStorytellerPopulationIntent)
                        yield return new CodeInstruction(OpCodes.Mul);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}