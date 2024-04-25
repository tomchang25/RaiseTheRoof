using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection;

namespace RaiseTheRoof
{
    public class RTRUtils
    {
        public static ThingDef GetRoofThingDef(RoofDef roofDef)
        {
            if (roofDef == RoofDefOf.RTR_RoofSteel) return RaiseTheRoofDefOf.RTR_SteelRoof;
            if (roofDef == RoofDefOf.RTR_RoofTransparent) return RaiseTheRoofDefOf.RTR_TransparentRoof;
            if (roofDef == RoofDefOf.RTR_RoofSolar) return RaiseTheRoofDefOf.RTR_SolarRoof;
            if (roofDef == RoofDefOf.RTR_RoofTransparentSolar) return RaiseTheRoofDefOf.RTR_TransparentSolarRoof;

            return null;
        }
        public static List<ThingDef> ListOfRTRThingDefs()
        {
            List<ThingDef> list = new List<ThingDef>();
            list.Add(RaiseTheRoofDefOf.RTR_SteelRoof);
            list.Add(RaiseTheRoofDefOf.RTR_TransparentRoof);
            list.Add(RaiseTheRoofDefOf.RTR_SolarRoof);
            list.Add(RaiseTheRoofDefOf.RTR_TransparentSolarRoof);
            return list;
        }
        public static void ApplySettings()
        {
            if (RaiseTheRoofMod.settings != null)
            {
                if (!RaiseTheRoofMod.settings.transparentRoofEnabled)
                {
                    RaiseTheRoofMod.settings.transparentSolarRoofEnabled = false;
                }
                if (!RaiseTheRoofMod.settings.solarRoofEnabled)
                {
                    RaiseTheRoofMod.settings.transparentSolarRoofEnabled = false;
                }
                List<ThingDef> thingDefs = ListOfRTRThingDefs();
                if (!thingDefs.NullOrEmpty())
                {
                    foreach (ThingDef thingDef in thingDefs)
                    {
                        if (thingDef == RaiseTheRoofDefOf.RTR_SteelRoof)
                        {
                            List<ThingDefCountClass> list = thingDef.costList;
                            foreach (ThingDefCountClass def in list)
                            {
                                if (def.thingDef == RimWorld.ThingDefOf.Steel)
                                {
                                    def.count = RaiseTheRoofMod.settings.steelRoofSteelCost;
                                }
                            }
                        }
                        else if (thingDef == RaiseTheRoofDefOf.RTR_TransparentRoof)
                        {
                            List<ThingDefCountClass> list = thingDef.costList;
                            foreach (ThingDefCountClass def in list)
                            {
                                if (def.thingDef == RimWorld.ThingDefOf.Steel)
                                {
                                    def.count = RaiseTheRoofMod.settings.transparentRoofSteelCost;
                                }
                                if (def.thingDef == ThingDef.Named("Synthread"))
                                {
                                    def.count = RaiseTheRoofMod.settings.transparentRoofSynthreadCost;
                                }
                            }
                        }
                        else if (thingDef == RaiseTheRoofDefOf.RTR_SolarRoof)
                        {
                            List<ThingDefCountClass> list = thingDef.costList;
                            foreach (ThingDefCountClass def in list)
                            {
                                if (def.thingDef == RimWorld.ThingDefOf.Steel)
                                {
                                    def.count = RaiseTheRoofMod.settings.solarRoofSteelCost;
                                }
                                if (def.thingDef == RimWorld.ThingDefOf.ComponentIndustrial)
                                {
                                    def.count = RaiseTheRoofMod.settings.solarRoofComponentCost;
                                }
                            }
                        }
                        else if (thingDef == RaiseTheRoofDefOf.RTR_TransparentSolarRoof)
                        {
                            List<ThingDefCountClass> list = thingDef.costList;
                            foreach (ThingDefCountClass def in list)
                            {
                                if (def.thingDef == RimWorld.ThingDefOf.Steel)
                                {
                                    def.count = RaiseTheRoofMod.settings.transparentSolarRoofSteelCost;
                                }
                                if (def.thingDef == ThingDef.Named("Synthread"))
                                {
                                    def.count = RaiseTheRoofMod.settings.transparentSolarRoofSynthreadCost;
                                }
                                if (def.thingDef == RimWorld.ThingDefOf.ComponentIndustrial)
                                {
                                    def.count = RaiseTheRoofMod.settings.transparentSolarRoofComponentCost;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static bool RoofDefExists(Def def)
        {
            Log.Warning(def.GetType().ToString());
            if (def == RoofDefOf.RTR_RoofSteel) return true;
            if (def == RoofDefOf.RTR_RoofTransparent) return true;
            if (def == RoofDefOf.RTR_RoofSolar) return true;
            if (def == RoofDefOf.RTR_RoofTransparentSolar) return true;

            return false;
        }

        public static bool RoofThingDefExists(List<Thing> things)
        {
            if (!things.NullOrEmpty())
            {
                foreach (Thing thing in things)
                {
                    if (thing.def == RaiseTheRoofDefOf.RTR_SteelRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_TransparentRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_SolarRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_TransparentSolarRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveSteelRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveTransparentRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveSolarRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveTransparentSolarRoof) return true;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveMountainousRoof) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_SteelRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_TransparentRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_SolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_TransparentSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveSteelRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveTransparentRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveTransparentSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveMountainousRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_SteelRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_TransparentRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_SolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_TransparentSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveSteelRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveTransparentRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveTransparentSolarRoof")) return true;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveMountainousRoof")) return true;
                }
            }
            return false;
        }
        public static bool RoofFrameOrBlueprintExists(Thing thing)
        {
            if (thing != null)
            {
                if (thing.def == ThingDef.Named("Blueprint_RTR_SteelRoof")) return true;
                if (thing.def == ThingDef.Named("Blueprint_RTR_TransparentRoof")) return true;
                if (thing.def == ThingDef.Named("Blueprint_RTR_SolarRoof")) return true;
                if (thing.def == ThingDef.Named("Blueprint_RTR_TransparentSolarRoof")) return true;
                if (thing.def == ThingDef.Named("Frame_RTR_SteelRoof")) return true;
                if (thing.def == ThingDef.Named("Frame_RTR_TransparentRoof")) return true;
                if (thing.def == ThingDef.Named("Frame_RTR_SolarRoof")) return true;
                if (thing.def == ThingDef.Named("Frame_RTR_TransparentSolarRoof")) return true;
            }
            return false;
        }

        public static Thing RemoveRoofExists(IntVec3 cell, Map map)
        {
            IEnumerable<Thing> things = map.thingGrid.ThingsAt(cell);
            if (!things.EnumerableNullOrEmpty())
            {
                foreach (Thing thing in things)
                {
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveSteelRoof) return thing;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveTransparentRoof) return thing;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveSolarRoof) return thing;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveTransparentSolarRoof) return thing;
                    if (thing.def == RaiseTheRoofDefOf.RTR_RemoveMountainousRoof) return thing;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveSteelRoof")) return thing;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveTransparentRoof")) return thing;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveSolarRoof")) return thing;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveTransparentSolarRoof")) return thing;
                    if (thing.def == ThingDef.Named("Blueprint_RTR_RemoveMountainousRoof")) return thing;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveSteelRoof")) return thing;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveTransparentRoof")) return thing;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveSolarRoof")) return thing;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveTransparentSolarRoof")) return thing;
                    if (thing.def == ThingDef.Named("Frame_RTR_RemoveMountainousRoof")) return thing;
                }
            }
            return null;
        }

        public static void RemoveRoof(IntVec3 cell, Map map, RoofDef roofDef)
        {
            if (roofDef == null) return;

            RoofCollapser.ProcessRoofHolderDespawned(cell, map);
            ThingDef def = RTRUtils.GetRoofThingDef(roofDef);
            if (def != null)
            {
                List<ThingDefCountClass> list = def.costList;
                if (list == null || !list.Any())
                {
                    return;
                }
                foreach (ThingDefCountClass countClass in list)
                {
                    int stackCount = (int)(countClass.count * 0.8f);
                    if (stackCount > 0)
                    {
                        Thing thing = ThingMaker.MakeThing(countClass.thingDef, null);
                        thing.stackCount = stackCount;
                        GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Near);
                    }
                }
            }
            bool mountainousRoof = roofDef == RimWorld.RoofDefOf.RoofRockThick;
            if (mountainousRoof)
            {
                if (RaiseTheRoofMod.settings == null) return;

                ThingDef thingDef = CreateChunk(cell, map);
                if (thingDef != null)
                {
                    if (Rand.Range(1, 100) <= RaiseTheRoofMod.settings.chunkChance)
                    {
                        Thing thing = ThingMaker.MakeThing(thingDef, null);
                        GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Near);
                    }
                }
            }
        }
        static ThingDef CreateChunk(IntVec3 cell, Map map)
        {
            TerrainDef terrainDef = map.terrainGrid.TerrainAt(cell);
            if (terrainDef != null)
            {
                if (terrainDef == TerrainDef.Named("Sandstone_RoughHewn") || terrainDef == TerrainDef.Named("Sandstone_Rough"))
                {
                    return ThingDef.Named("ChunkSandstone");
                }
                else if (terrainDef == TerrainDef.Named("Granite_RoughHewn") || terrainDef == TerrainDef.Named("Granite_Rough"))
                {
                    return ThingDef.Named("ChunkGranite");
                }
                else if (terrainDef == TerrainDef.Named("Marble_RoughHewn") || terrainDef == TerrainDef.Named("Marble_Rough"))
                {
                    return ThingDef.Named("ChunkMarble");
                }
                else if (terrainDef == TerrainDef.Named("Limestone_RoughHewn") || terrainDef == TerrainDef.Named("Limestone_Rough"))
                {
                    return ThingDef.Named("ChunkLimestone");
                }
                else if (terrainDef == TerrainDef.Named("Slate_RoughHewn") || terrainDef == TerrainDef.Named("Slate_Rough"))
                {
                    return ThingDef.Named("ChunkSlate");
                }
            }
            List<Thing> allThings = map.listerThings.AllThings;
            if (allThings.NullOrEmpty()) return null;

            List<Thing> targetList = new List<Thing>();
            foreach (Thing t in allThings)
            {
                if (t != null && (t.def == ThingDef.Named("ChunkSandstone") || t.def == ThingDef.Named("ChunkGranite") || t.def == ThingDef.Named("ChunkMarble") || t.def == ThingDef.Named("ChunkLimestone") || t.def == ThingDef.Named("ChunkSlate")))
                {
                    targetList.Add(t);
                }
            }
            if (targetList.NullOrEmpty()) return null;

            Thing thing = null;
            thing = GenClosest.ClosestThing_Global_Reachable(cell, map, targetList, PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassAllDestroyableThings, Danger.Deadly, false), 9999, null);
            if (thing != null) return thing.def;
            return null;
        }
        public static ThingDef FindSolarArray(RoofDef roofDef)
        {
            if (roofDef.defName.Equals("RTR_RoofSolar") || roofDef.defName.Equals("RTR_RoofTransparentSolar"))
            {
                return RaiseTheRoofDefOf.RTR_SolarArray;
            }
            return null;
        }
        public static void InstallSolarArray(ThingDef def, IntVec3 cell, Map map)
        {
            Thing thing = ThingMaker.MakeThing(def, null);
            thing.SetFaction(Faction.OfPlayer, null);
            GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Direct);
        }
    }
}
