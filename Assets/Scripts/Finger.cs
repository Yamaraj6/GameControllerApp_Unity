using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FingerName
{
    Thumb = 0,
    Index = 1,
    Mid = 2,
    Ring = 3,
    Pinky = 4
}

public enum FingerPosition
{
    Bent,
    Straight
}

public class Finger
{
    public readonly float BENT = 0.5f;

    private FingerName finger_name;
    private FingerPosition flex_postion;
    private float finger_flex;

    private GameObject[] gob_bones;
    private Collider[] col_finger_colliders;
    private int i_collision_power = 0;

    public float GetFingerFlex()
    { return finger_flex; }

    public FingerName GetFingerName()
    { return finger_name; }

    public FingerPosition GetFingerPosition()
    { return flex_postion; }

    public int GetColPower()
    {
        if (gob_bones[0].name == "boneTh1") ///BEZ SEMSU?
            return i_collision_power;
        return -1;
    }

    public Finger(GameObject gobBone1, GameObject gobBone2, GameObject gobBone3, FingerName fingerType)
    {
        finger_name = fingerType;
        gob_bones = new GameObject[3];
        gob_bones[0] = gobBone1;
        gob_bones[1] = gobBone2;
        gob_bones[2] = gobBone3;
        if (gobBone1.name == "boneTh1")
        {
            col_finger_colliders = new Collider[2];
            Collider[] _colTemp = new Collider[2];
            _colTemp = gobBone3.GetComponentsInChildren<Collider>();
            col_finger_colliders[0] = _colTemp[0];
            col_finger_colliders[1] = _colTemp[1];
        }
        flex_postion = FingerPosition.Straight;
    }

    public void MoveFinger(float dPosition)
    {
        finger_flex = dPosition;
        gob_bones[0].transform.localEulerAngles = new Vector3(90 * dPosition * dPosition, 0, 0);
        if (1.5f * 90 * dPosition < 90)
            gob_bones[1].transform.localEulerAngles = new Vector3(1.5f * 90 * dPosition, 0, 0);
        gob_bones[2].transform.localEulerAngles = new Vector3(90 * dPosition, 0, 0);

        UpdateFingerPosition();
    }

    private void UpdateFingerPosition()
    {
        if (finger_flex <= BENT)
            flex_postion = FingerPosition.Straight;
        else
            flex_postion = FingerPosition.Bent;
    }

    public void UpdateCollider()
    {
        i_collision_power = 0;
            for (int i = 0; i < 2; i++)
            if (gob_bones[0].name == "boneTh1")
                i_collision_power += col_finger_colliders[i].GetComponent<FingerColliider>().i_collision;
    }
}
