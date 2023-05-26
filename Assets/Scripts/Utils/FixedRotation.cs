using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    public Vector3 rotation;

    void Update()
    {
        transform.SetPositionAndRotation(transform.parent.position, Quaternion.Euler(rotation));
    }
}
