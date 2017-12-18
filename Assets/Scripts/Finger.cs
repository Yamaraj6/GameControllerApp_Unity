using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FingerType
{
    Thumb = 0,
    Index = 1,
    Mid = 2,
    Ring = 3,
    Pinky = 4
}

public enum FingerPosition
{
    Closed,
    HalfOpen,
    Open
}

public class Finger
{
    public readonly float CLOSED = 0.5f;
    public readonly float OPEN = 0.5f;

    private FingerType finger_type;
    private FingerPosition flex_postion;
    private float finger_flex;

    private GameObject[] gob_bones;
    private Collider[] col_finger_colliders;
    private int i_collision_power = 0;

    public float GetFingerFlex()
    { return finger_flex; }

    public FingerType GetFingerType()
    { return finger_type; }

    public FingerPosition GetFingerPosition()
    { return flex_postion; }

    public int GetColPower()
    {
        if (gob_bones[0].name == "boneTh1") ///BEZ SEMSU?
            return i_collision_power;
        return -1;
    }

    public Finger(GameObject gobBone1, GameObject gobBone2, GameObject gobBone3, FingerType fingerType)
    {
        finger_type = fingerType;
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
        flex_postion = FingerPosition.Open;
    }

    public void MoveFinger(float dPosition)
    {
        finger_flex = dPosition;
            //  if(0.75f * 90 * dPosition<45)
            gob_bones[0].transform.localEulerAngles = new Vector3(90 * dPosition* dPosition, 0, 0);
     //   else
      //      gob_bones[0].transform.localEulerAngles = new Vector3(1.125f * 90 * dPosition*dPosition, 0, 0);
        if (1.5f * 90 * dPosition<90)
            gob_bones[1].transform.localEulerAngles = new Vector3(1.5f*90 * dPosition, 0, 0);
        gob_bones[2].transform.localEulerAngles = new Vector3(90 * dPosition, 0, 0);

        UpdateFingerFlex();
    }

    private void UpdateFingerFlex()
    {
        if (finger_flex <= OPEN)
            flex_postion = FingerPosition.Open;
        else if (finger_flex >= CLOSED)
            flex_postion = FingerPosition.Closed;
        else
            flex_postion = FingerPosition.HalfOpen;
    }

    public void UpdateCollider()
    {
        i_collision_power = 0;
            for (int i = 0; i < 5; i++)
            if (gob_bones[0].name == "boneTh1")
                i_collision_power += col_finger_colliders[i].GetComponent<FingerColliider>().i_collision;
    }
}
