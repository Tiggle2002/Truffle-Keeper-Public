using System;
using Unity.VisualScripting;

public static class ActiveStructure
{
    public static StructureSite Site { get; private set; }

    public static void SetSite(this StructureSite site)
    {
        Site = site;
    }

    public static void UnsetSite(this StructureSite site)
    {
        if (Site == site)
        {
            StructureEvent.Trigger(StructureEventType.NoActiveSite);
            Site = null;
        }
    }

    public static bool CanHoldStructureSize(this StructureSite site, StructureSize size)
    {
        return site.CanHoldStructureOfSize(size);
    }

    public static void TryBuildAtCurrentLocation(StructureData data)
    {
        StructureEvent.Trigger(StructureEventType.Build, data, Site);
    }
}