using RaiseTheRoof;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RaiseTheRoof
{
    [StaticConstructorOnStartup]
    public class CompPowerSolarArray : CompPowerPlant
    {
 
        protected override float DesiredPowerOutput
        {
            get
            {
                return Mathf.Lerp(0f, RaiseTheRoofMod.settings.solarPowerOutput, this.parent.Map.skyManager.CurSkyGlow);
            }
        }

    }
}

