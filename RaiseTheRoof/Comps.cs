using RimWorld;
using Verse;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
    
namespace RaiseTheRoof
{
    public class CompProperties_RemoveCustomRoof : CompProperties
    {
        public RoofDef roofDef;
    }
    public class CompRemoveCustomRoof : ThingComp
    {
        public CompProperties_RemoveCustomRoof Props
        {
            get
            {
                return (CompProperties_RemoveCustomRoof)props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                RoofDef roof = parent.Map.roofGrid.RoofAt(parent.Position);
                if (roof == null || (roof != null && roof.defName == Props.roofDef.defName))
                {
                    parent.Map.roofGrid.SetRoof(parent.Position, null);
                    RTRUtils.RemoveRoof(parent.Position, parent.Map, roof);
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Destroyed)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
        }
    }
    public class CompProperties_BuildCustomRoof : CompProperties
    {
        public RoofDef roofDef;
    }
    public class CompBuildCustomRoof : ThingComp
    {
        public CompProperties_BuildCustomRoof Props
        {
            get
            {
                return (CompProperties_BuildCustomRoof)props;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
		{
            base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
                RoofDef roof = parent.Map.roofGrid.RoofAt(parent.Position);
                if (roof == null || (roof != null && roof.defName != Props.roofDef.defName))
                {
                    parent.Map.roofGrid.SetRoof(parent.Position, Props.roofDef);
                    MoteMaker.PlaceTempRoof(parent.Position, parent.Map);
                    ThingDef thingDef = RTRUtils.FindSolarArray(Props.roofDef);
                    if (thingDef != null)
                    {
                        RTRUtils.InstallSolarArray(thingDef, parent.Position, parent.Map);
                    }
                }
			}
		}
		public override void CompTick()
		{
			base.CompTick();
			if (!parent.Destroyed)
			{
				parent.Destroy(DestroyMode.Vanish);
			}
		}
	}
}
