using RimWorld;
using Verse;
using UnityEngine;

namespace RaiseTheRoof
{
    public class PlaceWorker_BuildRoof : PlaceWorker
    {
	    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
	    {
            if (!loc.InBounds(map))
            {
                return false;
            }
            if (loc.Fogged(map))
            {
                return false;
            }
            RoofDef roofDef = map.roofGrid.RoofAt(loc);
            if (roofDef != null && roofDef.isThickRoof)
            {
                return false;
            }
            if (RTRUtils.RoofThingDefExists(map.thingGrid.ThingsListAt(loc)))
            {
                return false;
            }
            return true;
        }
        public override void PostPlace(Map map, BuildableDef def, IntVec3 loc, Rot4 rot)
        {
            foreach (Thing current in map.thingGrid.ThingsAt(loc))
            {
                if (current.def.plant != null && current.def.plant.interferesWithRoof)
                {
                    Messages.Message("MessageRoofIncompatibleWithPlant".Translate(current), MessageTypeDefOf.CautionInput, false);
                }
            }
        }
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            GenUI.RenderMouseoverBracket();
            currentMap.areaManager.BuildRoof.MarkForDraw();
            currentMap.areaManager.NoRoof.MarkForDraw();
            currentMap.roofGrid.Drawer.MarkForDraw();
        }
    }
}