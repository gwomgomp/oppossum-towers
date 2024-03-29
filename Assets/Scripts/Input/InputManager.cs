using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private static readonly Dictionary<InputType, string> inputMapping = new() {
        {InputType.Interact, "Interact"},
        {InputType.Camera, "Camera"},
    };

    private readonly Dictionary<string, SortedList<int, Func<bool>>> registeredInputs = new();

    void Awake() {
        ManagerProvider.Instance.RegisterManager(this);
    }

    void Update() {
        foreach (var input in registeredInputs.Keys) {
            HandleInput(input);
        }
    }

    private void HandleInput(string input) {
        if (Input.GetButtonDown(input)) {
            foreach (var func in registeredInputs[input].Values) {
                if (func.Invoke()) {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// register some action that should be executed on some input.
    /// </summary>
    /// <param name="input">the input to watch for</param>
    /// <param name="action">the action to execute, returning true if no further input should be considered and false otherwise</param>
    /// <param name="prioritization">the prioritization for the action, lower numbers being executed before higher ones</param>
    public void RegisterInput(InputType input, Func<bool> action, int prioritization) {
        var mappedInput = inputMapping[input];
        var actions = registeredInputs.GetValueOrDefault(mappedInput, new SortedList<int, Func<bool>>());
        if (!registeredInputs.ContainsKey(mappedInput)) {
            registeredInputs.Add(mappedInput, actions);
        }
        actions.Add(prioritization, action);
    }

    public enum InputType {
        Interact,
        Camera
    }
}
