using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerColliider : MonoBehaviour
{
    public int i_collision = 0;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        i_collision = 1;
    }
    
    private void OnCollisionExit(Collision collision)
    {
        i_collision = 0;
    }
}
