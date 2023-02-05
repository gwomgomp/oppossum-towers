using UnityEngine;

public class Hoard : LaneCheckpoint, Storage {
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

    public void DepositLoot() {
        availableLoot++;
    }

    public bool Store(Cargo cargo) {
        if (cargo is Loot) {
            DepositLoot();
            return true;
        } else {
            return false;
        }
    }
}
