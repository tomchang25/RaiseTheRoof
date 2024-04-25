using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;
using System.Security.Cryptography;

namespace RaiseTheRoof
{
    [StaticConstructorOnStartup]
    static class Patches
    {
        public const BindingFlags allFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
        static Patches()
        {
            var harmony = new Harmony("raisetheroof.harmony");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(GlowGrid))]
        [HarmonyPatch("GroundGlowAt")]
        static class Patch_GlowGrid_GameGlowAt
        {
            static FieldInfo FI_map = typeof(GlowGrid).GetField("map", allFlags);
            static Map map(GlowGrid instance)
            {
                return (Map)FI_map.GetValue(instance);
            }

            //static MethodInfo MI_GetAccumulatedGlowAt = typeof(GlowGrid).GetMethod("GetAccumulatedGlowAt", BindingFlags.NonPublic | BindingFlags.Instance);
            static MethodInfo MI_GetAccumulatedGlowAt = typeof(GlowGrid).GetMethod("GetAccumulatedGlowAt", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(IntVec3), typeof(bool) }, null);
            static Color32 GetAccumulatedGlowAt(GlowGrid instance, IntVec3 c, bool ignoreCavePlants)
            {
                return (Color32)MI_GetAccumulatedGlowAt.Invoke(instance, new object[] { c, ignoreCavePlants });
            }

            static float GroundGlowAt(GlowGrid instance, IntVec3 c, bool ignoreCavePlants = false, bool ignoreSky = false)
            {
                float num = 0f;
                Map map = Patch_GlowGrid_GameGlowAt.map(instance);
                if (!ignoreSky && !map.roofGrid.Roofed(c) || map.roofGrid.RoofAt(c) == RoofDefOf.RTR_RoofTransparent || map.roofGrid.RoofAt(c) == RoofDefOf.RTR_RoofTransparentSolar)
                {
                    num = map.skyManager.CurSkyGlow;
                    if (num == 1f)
                    {
                        return num;
                    }
                }

                Color32 accumulatedGlowAt = GetAccumulatedGlowAt(instance, c, ignoreCavePlants);
                if (accumulatedGlowAt.a == 1)
                {
                    return 1f;
                }

                float b = (float)Mathf.Max(Mathf.Max(accumulatedGlowAt.r, accumulatedGlowAt.g), accumulatedGlowAt.b) / 255f * 3.6f;
                b = Mathf.Min(0.5f, b);
                return Mathf.Max(num, b);
            }

            static bool Prefix(ref GlowGrid __instance, ref float __result, ref IntVec3 c, bool ignoreCavePlants = false)
            {
                __result = GroundGlowAt(__instance, c, ignoreCavePlants);
                return false;
            }
        }

        [HarmonyPatch(typeof(SectionLayer_LightingOverlay))]
        [HarmonyPatch("Regenerate")]
        static class Patch_SectionLayer_LightingOverlay_Regenerate
        {
            static PropertyInfo PI_Map = typeof(SectionLayer).GetProperty("Map", allFlags);
            static FieldInfo FI_sectRect = typeof(SectionLayer_LightingOverlay).GetField("sectRect", allFlags);
            static FieldInfo FI_firstCenterInd = typeof(SectionLayer_LightingOverlay).GetField("firstCenterInd", allFlags);
            static FieldInfo FI_glowGrid = typeof(SectionLayer_LightingOverlay).GetField("glowGrid", allFlags);
            static MethodInfo MI_MakeBaseGeometry = typeof(SectionLayer_LightingOverlay).GetMethod("MakeBaseGeometry", allFlags);

            static Map Map(SectionLayer_LightingOverlay instance)
            {
                return (Map)PI_Map.GetValue(instance, null);
            }
            static CellRect sectRect(SectionLayer_LightingOverlay instance)
            {
                return (CellRect)FI_sectRect.GetValue(instance);
            }
            static int firstCenterInd(SectionLayer_LightingOverlay instance)
            {
                return (int)FI_firstCenterInd.GetValue(instance);
            }
            static ColorInt[] glowGrid(SectionLayer_LightingOverlay instance)
            {
                return (ColorInt[])FI_glowGrid.GetValue(instance);
            }
            static void MakeBaseGeometry(SectionLayer_LightingOverlay instance, LayerSubMesh sm)
            {
                MI_MakeBaseGeometry.Invoke(instance, new object[] { sm });
            }
            static void CalculateVertexIndices(SectionLayer_LightingOverlay instance, int worldX, int worldZ, out int botLeft, out int topLeft, out int topRight, out int botRight, out int center)
            {
                int num = worldX - sectRect(instance).minX;
                int num2 = worldZ - sectRect(instance).minZ;
                botLeft = num2 * (sectRect(instance).Width + 1) + num;
                topLeft = (num2 + 1) * (sectRect(instance).Width + 1) + num;
                topRight = (num2 + 1) * (sectRect(instance).Width + 1) + (num + 1);
                botRight = num2 * (sectRect(instance).Width + 1) + (num + 1);
                center = firstCenterInd(instance) + (num2 * sectRect(instance).Width + num);
            }
            static bool Prefix(SectionLayer_LightingOverlay __instance)
            {
                LayerSubMesh subMesh = __instance.GetSubMesh(MatBases.LightOverlay);
                if (subMesh.verts.Count == 0)
                {
                    MakeBaseGeometry(__instance, subMesh);
                }

                Color32[] array = new Color32[subMesh.verts.Count];
                int maxX = sectRect(__instance).maxX;
                int maxZ = sectRect(__instance).maxZ;
                int width = sectRect(__instance).Width;
                Map map = Map(__instance);
                int x = map.Size.x;
                Thing[] innerArray = map.edificeGrid.InnerArray;
                Thing[] array2 = innerArray;
                int num = array2.Length;
                RoofGrid roofGrid = map.roofGrid;
                CellIndices cellIndices = map.cellIndices;
                CalculateVertexIndices(__instance, sectRect(__instance).minX, sectRect(__instance).minZ, out var botLeft, out var _, out var topRight, out var botRight, out var center);
                int num2 = cellIndices.CellToIndex(new IntVec3(sectRect(__instance).minX, 0, sectRect(__instance).minZ));
                int[] array3 = new int[4]
                {
                    -map.Size.x - 1,
                    -map.Size.x,
                    -1,
                    0
                };
                int[] array4 = new int[4] { -1, -1, 0, 0 };
                for (int i = sectRect(__instance).minZ; i <= maxZ + 1; i++)
                {
                    int num3 = num2 / x;
                    int num4 = sectRect(__instance).minX;
                    while (num4 <= maxX + 1)
                    {
                        ColorInt colorInt = new ColorInt(0, 0, 0, 0);
                        int num5 = 0;
                        bool flag = false;
                        for (int j = 0; j < 4; j++)
                        {
                            int num6 = num2 + array3[j];
                            if (num6 >= 0 && num6 < num && num6 / x == num3 + array4[j])
                            {
                                Thing thing = array2[num6];
                                RoofDef roofDef = roofGrid.RoofAt(num6);
                                if (roofDef != null)
                                {
                                    if (roofDef != RoofDefOf.RTR_RoofTransparent && roofDef != RoofDefOf.RTR_RoofTransparentSolar)
                                    {
                                        if(roofDef.isThickRoof || thing == null || !thing.def.holdsRoof || thing.def.altitudeLayer == AltitudeLayer.DoorMoveable)
                                        {
                                            flag = true;
                                        }
                                    }
                                }

                                if (thing == null || !thing.def.blockLight)
                                {
                                    colorInt += map.glowGrid.VisualGlowAt(num6);
                                    num5++;
                                }
                            }
                        }

                        if (num5 > 0)
                        {
                            array[botLeft] = (colorInt / num5).ProjectToColor32;
                        }
                        else
                        {
                            array[botLeft] = new Color32(0, 0, 0, 0);
                        }

                        if (flag && array[botLeft].a < 100)
                        {
                            array[botLeft].a = 100;
                        }

                        num4++;
                        botLeft++;
                        num2++;
                    }

                    int num7 = maxX + 2 - sectRect(__instance).minX;
                    botLeft -= num7;
                    num2 -= num7;
                    botLeft += width + 1;
                    num2 += map.Size.x;
                }

                CalculateVertexIndices(__instance, sectRect(__instance).minX, sectRect(__instance).minZ, out var botLeft2, out center, out botRight, out topRight, out var center2);
                int num8 = cellIndices.CellToIndex(sectRect(__instance).minX, sectRect(__instance).minZ);
                for (int k = sectRect(__instance).minZ; k <= maxZ; k++)
                {
                    int num9 = sectRect(__instance).minX;
                    while (num9 <= maxX)
                    {
                        ColorInt colorInt2 = default(ColorInt);
                        colorInt2 += array[botLeft2];
                        colorInt2 += array[botLeft2 + 1];
                        colorInt2 += array[botLeft2 + width + 1];
                        colorInt2 += array[botLeft2 + width + 2];
                        array[center2] = new Color32((byte)(colorInt2.r / 4), (byte)(colorInt2.g / 4), (byte)(colorInt2.b / 4), (byte)(colorInt2.a / 4));
                        if (roofGrid.RoofAt(num8) != RoofDefOf.RTR_RoofTransparent && roofGrid.RoofAt(num8) != RoofDefOf.RTR_RoofTransparentSolar)
                        {
                            if (array[center2].a < 100 && roofGrid.Roofed(num8))
                            {
                                Thing thing2 = array2[num8];
                                if (thing2 == null || !thing2.def.holdsRoof)
                                {
                                    array[center2].a = 100;
                                }
                            }
                        }
                        num9++;
                        botLeft2++;
                        center2++;
                        num8++;
                    }

                    botLeft2++;
                    num8 -= width;
                    num8 += map.Size.x;
                }

                subMesh.mesh.colors32 = array;
                return false;

            }

        }

        [HarmonyPatch(typeof(RoofGrid))]
        [HarmonyPatch("Color", MethodType.Getter)]
        static class Patch_RoofGrid_Color
        {
            static bool Prefix(ref Color __result)
            {
                __result = Color.white;
                return false;
            }
        }
        [HarmonyPatch(typeof(RoofGrid))]
        [HarmonyPatch("GetCellExtraColor")]
        static class Patch_RoofGrid_GetCellExtraColor
        {
            static FieldInfo FI_wantDraw = typeof(CellBoolDrawer).GetField("wantDraw", allFlags);
            static FieldInfo FI_drawer = typeof(Area).GetField("drawer", allFlags);
            static FieldInfo FI_map = typeof(RoofGrid).GetField("map", allFlags);
            static FieldInfo FI_roofGrid = typeof(RoofGrid).GetField("roofGrid", allFlags);
            static bool wantDraw(CellBoolDrawer instance)
            {
                return (bool)FI_wantDraw.GetValue(instance);
            }
            static CellBoolDrawer drawer(Area instance)
            {
                return (CellBoolDrawer)FI_drawer.GetValue(instance);
            }
            static Map map(RoofGrid instance)
            {
                return (Map)FI_map.GetValue(instance);
            }
            static RoofDef[] roofGrid(RoofGrid instance)
            {
                return (RoofDef[])FI_roofGrid.GetValue(instance);
            }
            public static Color GetCellExtraColor(RoofGrid instance, int index)
            {
                if (roofGrid(instance)[index] == RimWorld.RoofDefOf.RoofRockThick)
                {
                    Color orange = new Color(1.0f, 0.65f, 0f);
                    return orange;
                }
                else if (roofGrid(instance)[index] == RimWorld.RoofDefOf.RoofRockThin)
                {
                    return Color.yellow;
                }
                else if (roofGrid(instance)[index] == RimWorld.RoofDefOf.RoofConstructed)
                {
                    return Color.cyan;
                }
                else if (roofGrid(instance)[index] == RoofDefOf.RTR_RoofSteel)
                {
                    return Color.blue;
                }
                else if (roofGrid(instance)[index] == RoofDefOf.RTR_RoofTransparent)
                {
                    Color pink = new Color(0.93f, 0.51f, 0.93f);
                    return pink;
                }
                else if (roofGrid(instance)[index] == RoofDefOf.RTR_RoofSolar)
                {
                    Color teal = new Color(0f, 0.42f, 0.33f);
                    return teal;
                }
                else if (roofGrid(instance)[index] == RoofDefOf.RTR_RoofTransparentSolar)
                {
                    Color purple = new Color(0.49f, 0.15f, 0.80f);
                    return purple;
                }
                return Color.white;
            }
            static bool Prefix(RoofGrid __instance, ref Color __result, int index)
            {
                __result = GetCellExtraColor(__instance, index);
                return false;
            }
        }

        [HarmonyPatch(typeof(Designator_AreaBuildRoof))]
        [HarmonyPatch("CanDesignateCell")]
        static class Patch_Designator_AreaBuildRoof_CanDesignateCell
        {
            public static bool CanDesignateCell(Designator_AreaBuildRoof instance, IntVec3 c)
            {
                if (!c.InBounds(instance.Map)) return false;
                if (c.Fogged(instance.Map)) return false;

                if (RTRUtils.RoofThingDefExists(instance.Map.thingGrid.ThingsListAt(c))) return false;

                RoofDef roofDef = instance.Map.roofGrid.RoofAt(c);
                if (roofDef == null) return true;
                if (roofDef == RimWorld.RoofDefOf.RoofRockThin) return true;

                return false;
            }
            static bool Prefix(Designator_AreaBuildRoof __instance, ref AcceptanceReport __result, IntVec3 c)
            {
                __result = CanDesignateCell(__instance, c);
                return false;
            }
        }
        [HarmonyPatch(typeof(Designator_AreaBuildRoof))]
        [HarmonyPatch("SelectedUpdate")]
        static class Patch_Designator_AreaBuildRoof_SelectedUpdate
        {
            static bool Prefix(ref Designator_AreaBuildRoof __instance)
            {
                GenUI.RenderMouseoverBracket();
                __instance.Map.areaManager.BuildRoof.MarkForDraw();
                __instance.Map.areaManager.NoRoof.MarkForDraw();
                __instance.Map.roofGrid.Drawer.MarkForDraw();
                return false;
            }
        }
        [HarmonyPatch(typeof(Designator_AreaNoRoof))]
        [HarmonyPatch("CanDesignateCell")]
        static class Patch_Designator_AreaNoRoof_CanDesignateCell
        {
            public static bool CanDesignateCell(Designator_AreaNoRoof instance, IntVec3 c)
            {
                Log.Warning("Enter");
                if (!c.InBounds(instance.Map)) return false;
                if (c.Fogged(instance.Map)) return false;

                if (RTRUtils.RoofThingDefExists(instance.Map.thingGrid.ThingsListAt(c))) return false;

                RoofDef roofDef = instance.Map.roofGrid.RoofAt(c);
                if (roofDef != null)
                {
                    if (roofDef == RimWorld.RoofDefOf.RoofRockThin) return true;
                    if (roofDef == RimWorld.RoofDefOf.RoofConstructed) return true;
                    if (roofDef.isThickRoof) return false;
                    if (RTRUtils.RoofDefExists(roofDef)) return false;
                }                

                return !instance.Map.areaManager.NoRoof[c];
            }
            static bool Prefix(Designator_AreaNoRoof __instance, ref AcceptanceReport __result, IntVec3 c)
            {
                __result = CanDesignateCell(__instance, c);
                return false;
            }
        }
        [HarmonyPatch(typeof(Designator_AreaNoRoof))]
        [HarmonyPatch("SelectedUpdate")]
        static class Patch_Designator_AreaNoRoof_SelectedUpdate
        {
            static bool Prefix(ref Designator_AreaNoRoof __instance)
            {
                GenUI.RenderMouseoverBracket();
                __instance.Map.areaManager.NoRoof.MarkForDraw();
                __instance.Map.areaManager.BuildRoof.MarkForDraw();
                __instance.Map.roofGrid.Drawer.MarkForDraw();
                return false;
            }
        }

        [HarmonyPatch(typeof(JobDriver_BuildRoof))]
        [HarmonyPatch("DoEffect")]
        static class Patch_JobDriver_BuildRoof_DoEffect
        {
            static PropertyInfo PI_Cell = typeof(JobDriver_AffectRoof).GetProperty("Cell", allFlags);
            static PropertyInfo PI_Map = typeof(JobDriver).GetProperty("Map", allFlags);

            static IntVec3 Cell(JobDriver_AffectRoof instance)
            {
                return (IntVec3)PI_Cell.GetValue(instance, null);
            }
            static Map Map(JobDriver instance)
            {
                return (Map)PI_Map.GetValue(instance, null);
            }
            static bool Prefix(JobDriver_BuildRoof __instance)
            {
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 intVec = Cell(__instance) + GenAdj.AdjacentCellsAndInside[i];
                    if (intVec.InBounds(Map(__instance)) && Map(__instance).areaManager.BuildRoof[intVec] && !intVec.Roofed(Map(__instance)) && RoofCollapseUtility.WithinRangeOfRoofHolder(intVec, Map(__instance)) && RoofUtility.FirstBlockingThing(intVec, Map(__instance)) == null)
                    {
                        Map(__instance).roofGrid.SetRoof(intVec, RimWorld.RoofDefOf.RoofConstructed);
                        MoteMaker.PlaceTempRoof(intVec, Map(__instance));
                        Map(__instance).areaManager.BuildRoof[intVec] = false;
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(JobDriver_RemoveRoof))]
        [HarmonyPatch("DoEffect")]
        static class Patch_JobDriver_RemoveRoof_DoEffect
        {
            static PropertyInfo PI_Cell = typeof(JobDriver_AffectRoof).GetProperty("Cell", allFlags);
            static PropertyInfo PI_Map = typeof(JobDriver).GetProperty("Map", allFlags);

            static IntVec3 Cell(JobDriver_AffectRoof instance)
            {
                return (IntVec3)PI_Cell.GetValue(instance, null);
            }
            static Map Map(JobDriver instance)
            {
                return (Map)PI_Map.GetValue(instance, null);
            }
            static void Postfix(JobDriver_RemoveRoof __instance)
            {
                Map(__instance).areaManager.NoRoof[Cell(__instance)] = false;
            }
        }

        [HarmonyPatch(typeof(AutoBuildRoofAreaSetter))]
        [HarmonyPatch("TryGenerateAreaNow")]
        static class Patch_AutoBuildRoofAreaSetter_TryGenerateAreaNow
        {
            static bool Prefix()
            {
                if (RaiseTheRoofMod.settings != null)
                {
                    if (!RaiseTheRoofMod.settings.autoBuildRoof)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(InfestationCellFinder))]
        [HarmonyPatch("GetScoreAt")]
        static class Patch_InfestationCellFinder_GetScoreAt
        {
            static bool CanSpawnAt(IntVec3 cell, Map map)
            {
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofSteel) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofTransparent) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofSolar) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofTransparentSolar) return false;

                return true;
            }
            static void Postfix(ref float __result, IntVec3 cell, Map map)
            {
                if (RaiseTheRoofMod.settings != null)
                {
                    if (!RaiseTheRoofMod.settings.allowInfestations)
                    {
                        if (!CanSpawnAt(cell, map))
                        {
                            __result = 0f;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(DropCellFinder))]
        [HarmonyPatch("CanPhysicallyDropInto")]
        static class Patch_DropCellFinder_CanPhysicallyDropInto
        {
            static bool CanDropInto(IntVec3 cell, Map map)
            {
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofSteel) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofTransparent) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofSolar) return false;
                if (cell.GetRoof(map) == RoofDefOf.RTR_RoofTransparentSolar) return false;

                return true;
            }
            static void Postfix(ref bool __result, IntVec3 c, Map map)
            {
                if (!CanDropInto(c, map))
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(SavedGameLoaderNow))]
        [HarmonyPatch("LoadGameFromSaveFileNow")]
        static class Patch_SavedGameLoaderNow_LoadGameFromSaveFileNow
        {
            static void Prefix()
            {
                RTRUtils.ApplySettings();
            }
        }
        [HarmonyPatch(typeof(Area_BuildRoof))]
        [HarmonyPatch("Color", MethodType.Getter)]
        static class Patch_Area_BuildRoof_Color
        {
            static bool Prefix(ref Color __result)
            {
                __result = Color.green;
                return false;
            }
        }
        [HarmonyPatch(typeof(Area_NoRoof))]
        [HarmonyPatch("Color", MethodType.Getter)]
        static class Patch_Area_NoRoof_Color
        {
            static bool Prefix(ref Color __result)
            {
                __result = Color.red;
                return false;
            }
        }

        [HarmonyPatch(typeof(RoofCollapseCellsFinder))]
        [HarmonyPatch("Notify_RoofHolderDespawned")]
        static class Patch_RoofCollapseCellsFinder_Notify_RoofHolderDespawned
        {
            static bool Prefix(Thing t, Map map)
            {
                if (Current.ProgramState == ProgramState.Playing)
                {
                    RoofCollapser.ProcessRoofHolderDespawned(t.Position, map);
                }
                return false;
            }
        }


        [HarmonyPatch(typeof(RoofDef))]
        [HarmonyPatch("VanishOnCollapse", MethodType.Getter)]
        static class Patch_RoofDef_VanishOnCollapse
        {
            static bool Prefix(RoofDef __instance, ref bool __result)
            {
                if (__instance.ToString().Contains("RTR_"))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RoofGrid))]
        [HarmonyPatch("SetRoof")]
        static class Patch_RoofGrid_SetRoof
        {
            static FieldInfo FI_map = typeof(RoofGrid).GetField("map", allFlags);
            static Map map(RoofGrid instance)
            {
                return (Map)FI_map.GetValue(instance);
            }
            static void Postfix(RoofGrid __instance, IntVec3 c, RoofDef def)
            {
                if (def == null)
                {
                    foreach (Thing t in map(__instance).thingGrid.ThingsAt(c))
                    {
                        if (t.def == RaiseTheRoofDefOf.RTR_SolarArray)
                        {
                            t.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
            }
        }
        

        [HarmonyPatch]
        static class Patch_GenConstruct_CanConstruct
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(GenConstruct), "CanConstruct", new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool), typeof(JobDef) });
            }
            public static bool CanConstruct(Building b)
            {
                IntVec3 currentCell = b.Position;
                Map map = b.Map;

                if (!RoofCollapseUtility.WithinRangeOfRoofHolder(b.Position, b.Map)) return false;
                if (!RoofCollapseUtility.ConnectedToRoofHolder(b.Position, b.Map, assumeRoofAtRoot: true)) return false;

                return true;
            } 
            static void Postfix(ref bool __result, Thing t)
            {
                if (__result == true)
                {
                    if (t != null && t is Building && RTRUtils.RoofFrameOrBlueprintExists(t))
                    {
                        __result = CanConstruct(t as Building);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GenConstruct))]
        [HarmonyPatch("CanPlaceBlueprintOver")]
        static class Patch_GenConstruct_CanPlaceBlueprintOver
        {
            static bool Prefix(ref bool __result, BuildableDef newDef)
            {
                if (newDef.ToString().Contains("RTR_"))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Designator_Build))]
        [HarmonyPatch("Visible", MethodType.Getter)]
        static class Patch_Designator_Build_Visible
        {
            static FieldInfo FI_entDef = typeof(Designator_Build).GetField("entDef", allFlags);
            static BuildableDef entDef(Designator_Build instance)
            {
                return (BuildableDef)FI_entDef.GetValue(instance);
            }
            static bool Prefix(Designator_Build __instance, ref bool __result)
            {
                if (entDef(__instance) == RaiseTheRoofDefOf.RTR_SolarArray)
                {
                    __result = false;
                    return false;
                }
                if (RaiseTheRoofMod.settings != null)
                {
                    if (!RaiseTheRoofMod.settings.removeMountainousRoofEnabled)
                    {
                        if (entDef(__instance) == RaiseTheRoofDefOf.RTR_RemoveMountainousRoof)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    if (!RaiseTheRoofMod.settings.steelRoofEnabled)
                    {
                        if (entDef(__instance) == RaiseTheRoofDefOf.RTR_SteelRoof || entDef(__instance) == RaiseTheRoofDefOf.RTR_RemoveSteelRoof)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    if (!RaiseTheRoofMod.settings.transparentRoofEnabled)
                    {
                        if (entDef(__instance) == RaiseTheRoofDefOf.RTR_TransparentRoof || entDef(__instance) == RaiseTheRoofDefOf.RTR_RemoveTransparentRoof)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    if (!RaiseTheRoofMod.settings.solarRoofEnabled)
                    {
                        if (entDef(__instance) == RaiseTheRoofDefOf.RTR_SolarRoof || entDef(__instance) == RaiseTheRoofDefOf.RTR_RemoveSolarRoof)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    if (!RaiseTheRoofMod.settings.transparentSolarRoofEnabled)
                    {
                        if (entDef(__instance) == RaiseTheRoofDefOf.RTR_TransparentSolarRoof || entDef(__instance) == RaiseTheRoofDefOf.RTR_RemoveTransparentSolarRoof)
                        {
                            __result = false;
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MainTabWindow_Research))]
        [HarmonyPatch("VisibleResearchProjects", MethodType.Getter)]
        static class Patch_MainTabWindow_Research_VisibleResearchProjects
        {
            static FieldInfo FI_cachedVisibleResearchProjects = typeof(MainTabWindow_Research).GetField("cachedVisibleResearchProjects", allFlags);
            static List<ResearchProjectDef> cachedVisibleResearchProjects(MainTabWindow_Research instance)
            {
                return (List<ResearchProjectDef>)FI_cachedVisibleResearchProjects.GetValue(instance);
            }
            static bool IsHiddenResearch(ResearchProjectDef def)
            {
                if (!RaiseTheRoofMod.settings.removeMountainousRoofEnabled)
                {
                    if (def == RaiseTheRoofDefOf.RTR_OverheadMountainRemoval)
                    {
                        return true;
                    }
                }
                if (!RaiseTheRoofMod.settings.transparentRoofEnabled)
                {
                    if (def == RaiseTheRoofDefOf.RTR_TransparentRoofing)
                    {
                        return true;
                    }
                }
                if (!RaiseTheRoofMod.settings.solarRoofEnabled)
                {
                    if (def == RaiseTheRoofDefOf.RTR_SolarRoofing)
                    {
                        return true;
                    }
                }
                if (!RaiseTheRoofMod.settings.transparentSolarRoofEnabled)
                {
                    if (def == RaiseTheRoofDefOf.RTR_TransparentSolarRoofing)
                    {
                        return true;
                    }
                }
                return false;
            }
            static bool Prefix(MainTabWindow_Research __instance, ref List<ResearchProjectDef> __result)
            {
                if (cachedVisibleResearchProjects(__instance) == null)
                {
                    FI_cachedVisibleResearchProjects.SetValue(__instance, new List<ResearchProjectDef>(DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef d) => (Find.Storyteller.difficulty.AllowedBy(d.hideWhen) || d == Find.ResearchManager.GetProject()) && !IsHiddenResearch(d))));
                }
                __result = cachedVisibleResearchProjects(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(DebugToolsGeneral))]
        [HarmonyPatch("Kill")]
        static class Patch_DebugToolsGeneral_Kill
        {
            static bool Prefix()
            {
                foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
                {
                    if (item.def != RaiseTheRoofDefOf.RTR_SolarArray)
                    {
                        item.Kill();
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(Thing))]
        [HarmonyPatch("LabelCap", MethodType.Getter)]
        static class Patch_Thing_LabelCap
        {
            static bool Prefix(ref string __result, Thing __instance)
            {
                if (__instance != null)
                {
                    if (__instance.def == RaiseTheRoofDefOf.RTR_SolarArray)
                    {
                        int powerOutput = (int)Mathf.Lerp(0f, 50f, __instance.Map.skyManager.CurSkyGlow);
                        if (RaiseTheRoofMod.settings != null)
                        {
                            powerOutput = (int)Mathf.Lerp(0f, RaiseTheRoofMod.settings.solarPowerOutput, __instance.Map.skyManager.CurSkyGlow);
                        }
                        string labelCap = __instance.Label.CapitalizeFirst(__instance.def) + " (Power output: " + powerOutput + " W)";
                        __result = labelCap;
                        return false;
                    }
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(PowerNet))]
        [HarmonyPatch("CurrentEnergyGainRate")]
        static class Patch_PowerNet_CurrentEnergyGainRate
        {
            static bool Prefix(PowerNet __instance, ref float __result)
            {
                if (DebugSettings.unlimitedPower) return true;

                int powerOutput = (int)Mathf.Lerp(0f, 50f, __instance.Map.skyManager.CurSkyGlow);
                int solarPower = 0;

                if (RaiseTheRoofMod.settings != null)
                {
                    powerOutput = (int)Mathf.Lerp(0f, RaiseTheRoofMod.settings.solarPowerOutput, __instance.Map.skyManager.CurSkyGlow);
                }
                foreach (CompPower c in __instance.transmitters)
                {

                    if (c.parent.def == RaiseTheRoofDefOf.RTR_SolarArray)
                    {
                        solarPower += powerOutput;
                    }
                }
                if (solarPower == 0) return true;

                float rate = 0f;
                for (int i = 0; i < __instance.powerComps.Count; i++)
                {
                    if (__instance.powerComps[i].PowerOn)
                    {
                        rate += __instance.powerComps[i].EnergyOutputPerTick;
                    }
                }
                rate += solarPower * CompPower.WattsToWattDaysPerTick;
                __result = rate;
                return false;
            }
        }
    }
}