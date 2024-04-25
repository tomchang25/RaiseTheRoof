using Verse;
using System.Collections.Generic;

namespace RaiseTheRoof
{
	public static class RoofCollapser
	{
        private static List<IntVec3> roofsCollapsingBecauseTooFar = new List<IntVec3>();
        private static HashSet<IntVec3> visitedCells = new HashSet<IntVec3>();
        public static void ProcessRoofHolderDespawned(IntVec3 loc, Map map)
        {
            CheckCollapseFlyingRoofs(loc, map);
            RoofGrid roofGrid = map.roofGrid;
            roofsCollapsingBecauseTooFar.Clear();
            for (int i = 0; i < RoofCollapseUtility.RoofSupportRadialCellsCount; i++)
            {
                IntVec3 intVec = loc + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && roofGrid.Roofed(intVec.x, intVec.z) && !map.roofCollapseBuffer.IsMarkedToCollapse(intVec) && !RoofCollapseUtility.WithinRangeOfRoofHolder(intVec, map))
                {
                    Thing thing = RTRUtils.RemoveRoofExists(intVec, map);
                    if (thing != null)
                    {
                        thing.Kill();
                    }
                    map.roofCollapseBuffer.MarkToCollapse(intVec);
                    roofsCollapsingBecauseTooFar.Add(intVec);
                }
            }
            foreach (IntVec3 c in roofsCollapsingBecauseTooFar)
            {
                CheckCollapseFlyingRoofs(c, map);
            }
            roofsCollapsingBecauseTooFar.Clear();
        }
        public static void CheckCollapseFlyingRoofs(IntVec3 loc, Map map, bool removalMode = false, bool canRemoveThickRoof = false)
        {
            visitedCells.Clear();
            CheckCollapseFlyingRoofAtAndAdjInternal(loc, map, removalMode, canRemoveThickRoof);
            visitedCells.Clear();
        }
        private static bool CheckCollapseFlyingRoofAtAndAdjInternal(IntVec3 loc, Map map, bool removalMode, bool canRemoveThickRoof)
        {
            RoofCollapseBuffer roofCollapseBuffer = map.roofCollapseBuffer;
            if (removalMode && roofCollapseBuffer.CellsMarkedToCollapse.Count > 0)
            {
                map.roofCollapseBufferResolver.CollapseRoofsMarkedToCollapse();
            }
            for (int i = 0; i < 5; i++)
            {
                IntVec3 intVec = loc + GenAdj.CardinalDirectionsAndInside[i];
                if (!intVec.InBounds(map) || !intVec.Roofed(map) || visitedCells.Contains(intVec) || roofCollapseBuffer.IsMarkedToCollapse(intVec) || RoofCollapseCellsFinder.ConnectsToRoofHolder(intVec, map, visitedCells))
                {
                    continue;
                }
                map.floodFiller.FloodFill(intVec, (IntVec3 x) => x.Roofed(map), delegate (IntVec3 x)
                {
                    Thing thing = RTRUtils.RemoveRoofExists(x, map);
                    if (thing != null)
                    {
                        thing.Kill();
                    }
                    roofCollapseBuffer.MarkToCollapse(x);
                });
            }
            return false;
        }
    }
}
