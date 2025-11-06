using RimWorld;
using Verse;
using Verse.Sound;

namespace AlphaBehavioursAndEvents
{
    public class HediffComp_TurnWhenDead : HediffComp
    {
       

        public HediffCompProperties_TurnWhenDead Props => (HediffCompProperties_TurnWhenDead)props;

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            //base.Notify_PawnDied();
            float severityToTurn = Props.severityToTurn;

            Map map = parent.pawn.Corpse?.Map;
            if (map == null || !(parent.Severity > severityToTurn)) return;
            Gender oldGender = parent.pawn.gender;
            Faction faction = null;
            if (Props.isHostile)
            {
                faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AncientsHostile);
            }
            int numToSpawn = Rand.RangeInclusive(Props.numberOfSpawn[0], Props.numberOfSpawn[1]);
            for (int i = 0; i < numToSpawn; i++) {
                PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(Props.thingToTurnTo), faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true,  1f, false, true, true, false, false);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent.pawn.Corpse);
                if (Props.keepGender)
                {
                    pawn.gender = oldGender;
                }
                if (Props.isHostile)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true);
                }

            }

            for (int i = 0; i < 20; i++)
            {
                CellFinder.TryFindRandomReachableCellNearPosition(parent.pawn.Corpse.Position, parent.pawn.Corpse.Position, map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors), null, null, out IntVec3 c);
                   
                FilthMaker.TryMakeFilth(c, parent.pawn.Corpse.Map, ThingDefOf.Filth_Blood);
                    
            }
                
               
            InternalDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(parent.pawn.Corpse.Position, map));
            parent.pawn.Corpse.Destroy();

        }

       

       /* public override void CompPostTick(ref float severityAdjustment)
        {
            position = this.parent.pawn.Position;
            map = this.parent.pawn.Map;
        }*/
    }
}
