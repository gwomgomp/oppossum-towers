using UnityEngine;

public class Hoard : LaneCheckpoint, Storage {
    public delegate void LootChange(float lootPercentage);
    public event LootChange OnLootChange;

    [SerializeField]
    private int availableLoot;

    private int maxLoot;

    public void Start()
    {
        maxLoot = availableLoot;
    }

    public bool TakeLoot() {
        if (availableLoot > 0) {
            availableLoot--;
            OnLootChange((float) availableLoot / maxLoot);
            return true;
        } else {
            return false;
        }
    }

    public void DepositLoot() {
        availableLoot++;
        OnLootChange((float) availableLoot / maxLoot);
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
