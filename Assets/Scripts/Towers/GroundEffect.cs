using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Purely for demonstration atm, will affect enemies via trigger in the future
public class GroundEffect : MonoBehaviour
{
  public float duration;
  public bool snapToGround;
  
  private float timer = 0;
  
  void Start()
  {
    if (snapToGround)
    {
      // Ground level should be defined by the level in the future via global variable
      transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
  }

  void Update()
  {
    timer += Time.deltaTime;
    
    if (timer >= duration)
    {
      Destroy(gameObject);
    }
  }
}
