using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetExtensions {
    public static Target GetFarthestTarget(this IEnumerable<Target> targets, Vector3 towerPosition) {
        return targets
            .OrderByDescending(target => Vector3.Distance(towerPosition, target.Enemy.gameObject.transform.position))
            .FirstOrDefault();
    }
}
