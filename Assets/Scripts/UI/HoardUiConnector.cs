using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoardUiConnector : MonoBehaviour {
    private static readonly int LootPercentage = Shader.PropertyToID("_LootPercentage");

    private Material material;

    void Start() {
        material = GetComponent<RawImage>().material;
        material.SetFloat(LootPercentage, 1);
        FindAnyObjectByType<Hoard>().OnLootChange += UpdateUi;

    }

    private void UpdateUi(float lootPercentage) {
        material.SetFloat(LootPercentage, lootPercentage);
    }
}
