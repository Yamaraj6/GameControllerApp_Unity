using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger
{
    private GameObject[] gob_bones;
    private Collider[] col_finger_colliders;
    private int i_collision_power = 0;

    public int GetColPower()
    {
        if (gob_bones[0].name == "boneTh1")
            return i_collision_power;
        return -1;
    }

    public Finger(GameObject gobBone1, GameObject gobBone2, GameObject gobBone3)
    {
        gob_bones = new GameObject[3];
        gob_bones[0] = gobBone1;
        gob_bones[1] = gobBone2;
        gob_bones[2] = gobBone3;
        if (gobBone1.name == "boneTh1")
        {
            col_finger_colliders = new Collider[5];
            col_finger_colliders[0] = gobBone1.GetComponentInChildren<Collider>();
            col_finger_colliders[1] = gobBone2.GetComponentInChildren<Collider>();
            Collider[] _colTemp = new Collider[3];
            _colTemp = gobBone3.GetComponentsInChildren<Collider>();
            col_finger_colliders[2] = _colTemp[0];
            col_finger_colliders[3] = _colTemp[1];
            col_finger_colliders[4] = _colTemp[2];
        }
    }

    public void MoveFinger(float dPosition)
    {
      //  if(0.75f * 90 * dPosition<45)
            gob_bones[0].transform.localEulerAngles = new Vector3(90 * dPosition* dPosition, 0, 0);
     //   else
      //      gob_bones[0].transform.localEulerAngles = new Vector3(1.125f * 90 * dPosition*dPosition, 0, 0);
        if (1.5f * 90 * dPosition<90)
            gob_bones[1].transform.localEulerAngles = new Vector3(1.5f*90 * dPosition, 0, 0);
        gob_bones[2].transform.localEulerAngles = new Vector3(90 * dPosition, 0, 0);
    }

    public void UpdateCollider()
    {
        i_collision_power = 0;
            for (int i = 0; i < 5; i++)
            if (gob_bones[0].name == "boneTh1")
                i_collision_power += col_finger_colliders[i].GetComponent<FingerColliider>().i_collision;
    }
}
