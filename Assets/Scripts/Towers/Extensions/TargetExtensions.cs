using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Towers.Extensions {
    public static class TargetExtensions {
        public static Target GetFarthestTarget(this IEnumerable<Target> targets, Vector3 towerPosition) {
            Target farthest = null;

            foreach (var target in targets) {
                if (farthest == null) {
                    farthest = target;
                } else {
                    farthest = GetFartherTarget(towerPosition, farthest, target);
                }
            }
            return farthest;
        }

        private static Target GetFartherTarget(Vector3 towerPosition, Target left, Target right) {
            var leftDistance = Vector3.Distance(towerPosition, left.Enemy.gameObject.transform.position);
            var rightDistance = Vector3.Distance(towerPosition, right.Enemy.gameObject.transform.position);
            if (leftDistance > rightDistance) {
                return left;
            } else {
                return right;
            }
        }

    }
}
