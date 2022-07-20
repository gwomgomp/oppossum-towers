using UnityEngine;

public class Hoard : LaneCheckpoint
{
    [SerializeField]
    private int availableLoot;

    public bool TakeLoot() {
        if (availableLoot > 0) {
            availableLoot--;
            return true;
        } else {
            return false;
        }
    }
}
