using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
  Rigidbody ownRigidbody;
  
  void Start()
  {
    ownRigidbody = GetComponent<Rigidbody>();
  }

  void Update()
  {
    ownRigidbody.velocity = transform.forward * 2.0f;
  }
}
