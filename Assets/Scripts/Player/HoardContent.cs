using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HoardContent : MonoBehaviour {
    public Hoard hoard;
    public float vanishDuration = 1;

    private ILookup<HoardState, Transform> stateTransforms;

    void Start() {
        stateTransforms = Enumerable.Range(0, gameObject.transform.childCount)
            .Select(index => gameObject.transform.GetChild(index))
            .SelectMany(child => child.childCount == 1 ? new List<Transform> { child, child.GetChild(0) } : new List<Transform> { child })
            .Where(child => IsScrap(child))
            .ToLookup(child => GetHoardState(child), child => child);

        if (hoard != null) {
            hoard.OnLootChange += UpdateState;
        }
    }

    void UpdateState(float lootPercentage) {
        var state = CalculateHoardState(lootPercentage);
        foreach (var stateTransform in stateTransforms) {
            var isActive = stateTransform.Key >= state;
            foreach (var current in stateTransform) {
                if (!DOTween.IsTweening(current, true)) {
                    if (isActive && !current.gameObject.activeSelf) {
                        current.gameObject.SetActive(true);
                        current.DOMoveY(current.position.y + 10, vanishDuration).SetEase(Ease.OutQuart);
                    } else if (!isActive && current.gameObject.activeSelf) {
                        current.DOMoveY(current.position.y - 10, vanishDuration).SetEase(Ease.InQuart)
                            .OnComplete(() => current.gameObject.SetActive(false));
                    }
                }
            }
        }
    }

    private bool IsScrap(Transform child) {
        return child.name.StartsWith("Full")
            || child.name.StartsWith("3/4")
            || child.name.StartsWith("Half")
            || child.name.StartsWith("1/4");
    }

    private HoardState GetHoardState(Transform transform) {
        var stateString = transform.name[..transform.name.IndexOf('_')];
        return stateString switch {
            "Full" => HoardState.Full,
            "3/4" => HoardState.ThreeQuarter,
            "Half" => HoardState.Half,
            "1/4" => HoardState.OneQuarter,
            _ => throw new InvalidOperationException($"{stateString} is not a valid state"),
        };
    }

    private HoardState CalculateHoardState(float lootPercentage) {
        if (lootPercentage == 1) {
            return HoardState.Full;
        } else if (lootPercentage >= 0.66) {
            return HoardState.ThreeQuarter;
        } else if (lootPercentage >= 0.33) {
            return HoardState.Half;
        } else if (lootPercentage > 0) {
            return HoardState.OneQuarter;
        }
        return HoardState.Empty;
    }

    public enum HoardState {
        Full = 0,
        ThreeQuarter = 1,
        Half = 2,
        OneQuarter = 3,
        Empty = 4
    }
}
