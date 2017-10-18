using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    private GameObject gob_hand;
    private GameObject[] gob_bones;
    private Finger[] fingers;

    private Vector3 v3_pos_zero;

    private Vector3 acc = new Vector3(0, 0, 0);
    private static double MIN_OFFSET = 20;
    private static double MAX_HAND_POS = 1.5;

    public Hand(GameObject gobHand)
    {
        FindFingers();
        gob_hand = gobHand;

    }

    public void ChangeRotation(float[] dGyroPosition)
    {
        gob_hand.transform.rotation = new Quaternion(dGyroPosition[2], -dGyroPosition[0], 
            -dGyroPosition[3], -dGyroPosition[1]);
    }

    public void ChangePosition(float[] dAccData)
    {
        if ((dAccData[0] > MIN_OFFSET || dAccData[0] < -MIN_OFFSET ||
            dAccData[1] > MIN_OFFSET || dAccData[1] < -MIN_OFFSET ||
            dAccData[2] > MIN_OFFSET || dAccData[2] < -MIN_OFFSET) &&
            dAccData[0] != acc.x && dAccData[1] != acc.y && dAccData[2] != acc.z)
            acc += new Vector3(dAccData[0] / 1000 * Time.deltaTime,
                dAccData[1] / 1000 * Time.deltaTime, dAccData[2] / 1000 * Time.deltaTime);
        else
            acc = new Vector3(0, 0, 0);
        if ((gob_hand.transform.position.x < v3_pos_zero.x + MAX_HAND_POS && gob_hand.transform.position.x > v3_pos_zero.x - MAX_HAND_POS)
            || (gob_hand.transform.position.x > v3_pos_zero.x + MAX_HAND_POS && acc.x < 0)
            || (gob_hand.transform.position.x < v3_pos_zero.x - MAX_HAND_POS && acc.x > 0))
            gob_hand.transform.position += new Vector3(acc.x, 0, 0);

        if ((gob_hand.transform.position.y < v3_pos_zero.y + MAX_HAND_POS && gob_hand.transform.position.y > v3_pos_zero.y - MAX_HAND_POS)
            || (gob_hand.transform.position.y > v3_pos_zero.y + MAX_HAND_POS && acc.y < 0)
            || (gob_hand.transform.position.y < v3_pos_zero.y - MAX_HAND_POS && acc.y > 0))
            gob_hand.transform.position += new Vector3(0, acc.y, 0);

        if ((gob_hand.transform.position.z < v3_pos_zero.z + MAX_HAND_POS && gob_hand.transform.position.z > v3_pos_zero.z - MAX_HAND_POS)
            || (gob_hand.transform.position.z > v3_pos_zero.z + MAX_HAND_POS && acc.z < 0)
            || (gob_hand.transform.position.z < v3_pos_zero.z - MAX_HAND_POS && acc.z > 0))
            gob_hand.transform.position += new Vector3(0, 0, acc.z);

        //   if (kd())
        //       gob_cube.transform.Translate((d_recived_data[4] / 1000 * Time.deltaTime), (d_recived_data[5] / 1000 * Time.deltaTime), (d_recived_data[6] / 1000 * Time.deltaTime));
        Math.Pow(gob_hand.transform.position.x, 2);
    }


    public void Calibrate()
    {
        gob_hand.transform.localPosition = v3_pos_zero;
    }

    public String sMoveFingers(float[] dFingerPosition)
    {
        String _sDataToSend = "";
        for (int i =0; i<5; i++)
        {
            fingers[i].MoveFinger(dFingerPosition[i]);
            fingers[i].UpdateCollider();
            if (fingers[i].GetColPower() != -1)
                _sDataToSend += fingers[i].GetColPower().ToString("0.0") + ";";
            else
                _sDataToSend += 0 + ";";
        }
        return _sDataToSend;
    }

    private void FindFingers()
    {
        gob_bones = new GameObject[15];
        String _sFingeName = "Th";
        for (int i = 0; i < 15; i++)
        {
            if (i >= 3 && i <= 5)
                _sFingeName = "In";
            else if (i >= 6 && i <= 8)
                _sFingeName = "Mid";
            else if (i >= 9 && i <= 11)
                _sFingeName = "Ring";
            else if (i >= 12 && i <= 15)
                _sFingeName = "Pi";
            gob_bones[i] = GameObject.Find("bone" + _sFingeName + ((i % 3) + 1));
        }

        fingers = new Finger[5];
        int j = 0;
        while (j < 15)
        {
            fingers[j / 3] = new Finger(gob_bones[j], gob_bones[j + 1], gob_bones[j + 2]);
            j += 3;
        }
    }
}
