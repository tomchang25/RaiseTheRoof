using RimWorld;
using Verse;
using System.Diagnostics;
using System.Collections.Generic;
    
namespace RaiseTheRoof
{
    public class Building_SolarArray : Building
    {
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            RoofDef roof = Map.roofGrid.RoofAt(Position);
            if (roof != null)
            {
            	if (!Map.roofCollapseBuffer.IsMarkedToCollapse(Position))
                {
                    Map.roofGrid.SetRoof(Position, null);
                }
            }
            base.DeSpawn(mode);
        }
    }
}
