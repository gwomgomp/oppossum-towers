using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceSpawnLocationExtension
{
    public static bool HasFreeSpawnSpots(this List<ResourceSpawnLocation> locations) {
        return locations.Any(location => location.IsFree);
    }
}
