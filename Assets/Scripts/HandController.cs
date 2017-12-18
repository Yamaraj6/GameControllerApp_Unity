using System;
using System.Collections.Generic;
using UnityEngine;

public enum HandPosition
{
    Open = 0,
    HalfOpen = 1,
    BentThumb = 4,
    ThumbIndexCircle = 5,
    Hold = 9,
    Fist = 10,
    Unknown = -1
};

public enum HandRotation
{
     Vertical,
     Horizontal
};


public class HandController
{
    public readonly FingerPosition[] OPEN_HAND = {FingerPosition.Open, FingerPosition.Open,
        FingerPosition.Open, FingerPosition.Open, FingerPosition.Open };

    public readonly FingerPosition[] HALF_OPEN_HAND = {FingerPosition.HalfOpen, FingerPosition.HalfOpen,
        FingerPosition.HalfOpen, FingerPosition.HalfOpen, FingerPosition.HalfOpen };

    public readonly FingerPosition[] BEND_THUMB = {FingerPosition.Closed, FingerPosition.Open,
        FingerPosition.Open, FingerPosition.Open, FingerPosition.Open };

    public readonly FingerPosition[] THUMB_INDEX_CIRCLE= {FingerPosition.Closed, FingerPosition.Closed,
        FingerPosition.Open, FingerPosition.Open, FingerPosition.Open };

    public readonly FingerPosition[] HOLD = {FingerPosition.Closed, FingerPosition.Closed,
        FingerPosition.Closed, FingerPosition.Open, FingerPosition.Closed };

    public readonly FingerPosition[] FIST = {FingerPosition.Closed, FingerPosition.Closed,
        FingerPosition.Closed, FingerPosition.Closed, FingerPosition.Closed };


    private GameObject hand;
    private Finger[] fingers;

    private HandPosition hand_position;
    private HandRotation hand_rotation;

    private Vector3 v3_pos_zero;
    
    private static double MIN_OFFSET = 20;
    private static double MAX_HAND_POS = 1.5;

    public HandController(GameObject gobHand)
    {
        FindFingers();
        hand = gobHand;
        v3_pos_zero = new Vector3(0.8f, -1, 5);
    }

    public HandPosition GetHandPosition()
    { return hand_position; }

    public HandRotation GetHandRotation()
    { return hand_rotation; }

    public Finger[] GetFingers()
    { return fingers; }

    public void ChangeRotation(Quaternion gyroPosition)
    {
        hand.transform.rotation = new Quaternion(gyroPosition.z, -gyroPosition.x,
            -gyroPosition.w, -gyroPosition.y);
    }

    private HandPosition CheckFingerPosition()
    {
        foreach(Finger finger in fingers)
        {
            if(finger.GetFingerFlex() > finger.CLOSED);
        }
        return HandPosition.Fist;
    }

    public void ChangePosition(float[] dAccData)
    {
        Vector3 acc = new Vector3(0, 0, 0);
        if ((dAccData[0] > MIN_OFFSET || dAccData[0] < -MIN_OFFSET ||
            dAccData[1] > MIN_OFFSET || dAccData[1] < -MIN_OFFSET ||
            dAccData[2] > MIN_OFFSET || dAccData[2] < -MIN_OFFSET) &&
            dAccData[0] != acc.x && dAccData[1] != acc.y && dAccData[2] != acc.z)
            acc += new Vector3(dAccData[0] / 1000 * Time.deltaTime,
                dAccData[1] / 1000 * Time.deltaTime, dAccData[2] / 1000 * Time.deltaTime);
        if ((hand.transform.position.x < v3_pos_zero.x + MAX_HAND_POS && hand.transform.position.x > v3_pos_zero.x - MAX_HAND_POS)
            || (hand.transform.position.x > v3_pos_zero.x + MAX_HAND_POS && acc.x < 0)
            || (hand.transform.position.x < v3_pos_zero.x - MAX_HAND_POS && acc.x > 0))
            hand.transform.position += new Vector3(acc.x, 0, 0);

        if ((hand.transform.position.y < v3_pos_zero.y + MAX_HAND_POS && hand.transform.position.y > v3_pos_zero.y - MAX_HAND_POS)
            || (hand.transform.position.y > v3_pos_zero.y + MAX_HAND_POS && acc.y < 0)
            || (hand.transform.position.y < v3_pos_zero.y - MAX_HAND_POS && acc.y > 0))
            hand.transform.position += new Vector3(0, acc.y, 0);

        if ((hand.transform.position.z < v3_pos_zero.z + MAX_HAND_POS && hand.transform.position.z > v3_pos_zero.z - MAX_HAND_POS)
            || (hand.transform.position.z > v3_pos_zero.z + MAX_HAND_POS && acc.z < 0)
            || (hand.transform.position.z < v3_pos_zero.z - MAX_HAND_POS && acc.z > 0))
            hand.transform.position += new Vector3(0, 0, acc.z);

        //   if (kd())
        //       gob_cube.transform.Translate((d_recived_data[4] / 1000 * Time.deltaTime), (d_recived_data[5] / 1000 * Time.deltaTime), (d_recived_data[6] / 1000 * Time.deltaTime));
        Math.Pow(hand.transform.position.x, 2);
    }

    public String MoveFingers(float[] fingerFlex)
    {
        String _sDataToSend = "";
        for (int i =0; i<5; i++)
        {
            fingers[i].MoveFinger(fingerFlex[i]);
            fingers[i].UpdateCollider();
            if (fingers[i].GetColPower() != -1)
                _sDataToSend += fingers[i].GetColPower().ToString("0.0") + ";";
            else
                _sDataToSend += 0 + ";";
        }
        UpdateHandPosition();
        UpdateHandRotation();
        return _sDataToSend;
    }

    private void UpdateHandPosition()
    {
        FingerPosition[] _fingersPosition = new FingerPosition[5];
        foreach (Finger finger in fingers)
        {
            _fingersPosition[(int)finger.GetFingerType()] = finger.GetFingerPosition();
        }
        if (_fingersPosition.IsEqual(OPEN_HAND))
        {
            hand_position = HandPosition.Open;
        }
        else if (_fingersPosition.IsEqual(HALF_OPEN_HAND))
        {
            hand_position = HandPosition.HalfOpen;
        }
        else if (_fingersPosition.IsEqual(BEND_THUMB))
        {
            hand_position = HandPosition.BentThumb;
        }
        else if (_fingersPosition.IsEqual(THUMB_INDEX_CIRCLE))
        {
            hand_position = HandPosition.ThumbIndexCircle;
        }
        else if (_fingersPosition.IsEqual(HOLD))
        {
            hand_position = HandPosition.Hold;
        }
        else if (_fingersPosition.IsEqual(FIST))
        {
            hand_position = HandPosition.Fist;
        }
        else
        {
            hand_position = HandPosition.Unknown;
        }
    }

    private void UpdateHandRotation()
    {
        if (hand.transform.rotation.x >= -0.2 && hand.transform.rotation.x <= 0.2 &&
              hand.transform.rotation.z > -0.2 && hand.transform.rotation.z <= 0.2)
            hand_rotation = HandRotation.Horizontal;
        else
            hand_rotation = HandRotation.Vertical;
    }

    public void Calibrate()
    {
        hand.transform.localPosition = v3_pos_zero;
    }

    private void FindFingers()
    {
        fingers = new Finger[5];
        GameObject[] _bones = new GameObject[3];
        String _sFingeName = "Th";
        for (int i = 0; i < 5; i++)
        {
            _bones[0] = GameObject.Find("bone" + _sFingeName + 1);
            _bones[1] = GameObject.Find("bone" + _sFingeName + 2);
            _bones[2] = GameObject.Find("bone" + _sFingeName + 3);
            switch(i)
            {
                case 0:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerType.Thumb);
                    _sFingeName = "In";
                    break;
                case 1:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerType.Index);
                    _sFingeName = "Mid";
                    break;
                case 2:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerType.Mid);
                    _sFingeName = "Ring";
                    break;
                case 3:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerType.Ring);
                    _sFingeName = "Pi";
                    break;
                case 4:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerType.Pinky);
                    break;
            }
        }
    }
}
