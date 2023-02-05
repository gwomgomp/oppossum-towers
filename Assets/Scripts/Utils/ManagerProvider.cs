using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// use this to get access to other manager classes instead of coding each as a singleton
/// </summary>
public class ManagerProvider {
    private static ManagerProvider instance = null;

    private readonly Dictionary<Type, object> managers = new();

    private ManagerProvider() {
    }

    public static ManagerProvider Instance {
        get {
            instance ??= new ManagerProvider();
            return instance;
        }
    }

    /// <summary>
    /// add a manager to be available from other classes. logs an error if another of the same type is already registered.
    /// </summary>
    /// <param name="manager">the manager to make available</param>
    /// <typeparam name="T">the type of manager</typeparam>
    public void RegisterManager<T>(T manager) {
        if (managers.ContainsKey(typeof(T))) {
            Debug.LogError($"Manager of type {typeof(T).Name} already registered");
        } else {
            managers.Add(typeof(T), manager);
        }
    }

    /// <summary>
    /// get a manager of a given type. logs an error if no matching manager was found.
    /// </summary>
    /// <typeparam name="T">the type to look for</typeparam>
    /// <returns>the manager</returns>
    public T GetManager<T>() {
        if (managers.ContainsKey(typeof(T)) && managers[typeof(T)] is T manager) {
            return manager;
        } else {
            Debug.LogError($"Could not find manager of type {typeof(T).Name}");
            return default;
        }
    }
}
