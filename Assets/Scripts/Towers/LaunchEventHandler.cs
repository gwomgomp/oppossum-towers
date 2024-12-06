using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEventHandler : MonoBehaviour
{
    public delegate void LaunchDelegate();
    public LaunchDelegate launchEvent;

    public void OnLaunch() {
        launchEvent?.Invoke();
    }
}
