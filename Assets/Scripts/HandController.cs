using System;
using System.Collections.Generic;
using System.Collections;
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

public class HandController
{
    public readonly FingerPosition[] OPEN_HAND = {FingerPosition.Straight, FingerPosition.Straight,
        FingerPosition.Straight, FingerPosition.Straight, FingerPosition.Straight };

    public readonly FingerPosition[] BEND_THUMB = {FingerPosition.Bent, FingerPosition.Straight,
        FingerPosition.Straight, FingerPosition.Straight, FingerPosition.Straight };

    public readonly FingerPosition[] THUMB_INDEX_CIRCLE = {FingerPosition.Bent, FingerPosition.Bent,
        FingerPosition.Straight, FingerPosition.Straight, FingerPosition.Straight };

    public readonly FingerPosition[] HOLD = {FingerPosition.Bent, FingerPosition.Straight,
        FingerPosition.Bent, FingerPosition.Bent, FingerPosition.Bent };

    public readonly FingerPosition[] FIST = {FingerPosition.Bent, FingerPosition.Bent,
        FingerPosition.Bent, FingerPosition.Bent, FingerPosition.Bent };


    private GameObject hand;
    public Finger[] fingers { get; private set; }
    public String collider_power_data { get; private set; }

    private LineRenderer laser_line;
    private GameObject gob_on_ray_end;
    private Gestures gestures;

    public HandPosition hand_position{ get; private set; }
    private List<HandPosition> last_hand_positions;

    private Vector3 v3_pos_zero;

    private static readonly double MIN_OFFSET = 20;
    private static readonly double MAX_HAND_POS = 1.5;
    public static readonly float RAYCAST_DISTANCE = 1000;

    public HandController(GameObject gobHand, GameObject spellToRespawn, GameObject gobToRespawn)
    {
        FindFingers();
        hand = gobHand;
        v3_pos_zero = new Vector3(0.8f, -1, 5);
        laser_line = hand.GetComponent<LineRenderer>();
        gob_on_ray_end = GameObject.Find("EndRayObject");
        gestures = new Gestures(spellToRespawn, gobToRespawn);

        last_hand_positions = new List<HandPosition>();
        hand.GetComponent<MonoBehaviour>().StartCoroutine(UpdateHandPositions());
        collider_power_data = "";
    }

    public void ChangeRotation(Quaternion gyroPosition)
    {
        hand.transform.rotation = new Quaternion(gyroPosition.z, -gyroPosition.x,
            -gyroPosition.w, -gyroPosition.y);
        DrawRaycastLine();
    }

    public void MoveFingers(float[] fingerFlex)
    {
        collider_power_data = "";
        for (int i = 0; i < 5; i++)
        {
            fingers[i].MoveFinger(fingerFlex[i]);
            fingers[i].UpdateCollider();
            if (fingers[i].GetColPower() != -1)
                collider_power_data += fingers[i].GetColPower().ToString("0.0") + ";";
            else
                collider_power_data += 0 + ";";
        }
        UpdateHandPosition();
    }

    private void UpdateHandPosition()
    {
        FingerPosition[] _fingersPosition = new FingerPosition[5];
        foreach (Finger finger in fingers)
        {
            _fingersPosition[(int)finger.GetFingerName()] = finger.GetFingerPosition();
        }
        if (_fingersPosition.IsEqual(OPEN_HAND))
        {
            hand_position = HandPosition.Open;
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
            switch (i)
            {
                case 0:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerName.Thumb);
                    _sFingeName = "In";
                    break;
                case 1:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerName.Index);
                    _sFingeName = "Mid";
                    break;
                case 2:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerName.Mid);
                    _sFingeName = "Ring";
                    break;
                case 3:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerName.Ring);
                    _sFingeName = "Pi";
                    break;
                case 4:
                    fingers[i] = new Finger(_bones[0], _bones[1], _bones[2], FingerName.Pinky);
                    break;
            }
        }
    }

    public void DrawRaycastLine()
    {
        laser_line.SetPosition(0, hand.transform.position);
        RaycastHit hit;
        Ray handRay = new Ray(hand.transform.position, -hand.transform.up);
        laser_line.SetPosition(1, gob_on_ray_end.transform.position);
        if (Physics.Raycast(handRay, out hit, RAYCAST_DISTANCE))
        {
            laser_line.SetPosition(1, hit.point);
        }
    }

    private IEnumerator UpdateHandPositions()
    {
        last_hand_positions.Add(hand_position);
        yield return new WaitForSeconds(0.01f);
        if (last_hand_positions.Count > 100)
        {
            gestures.RecognizeGesture(last_hand_positions[0], last_hand_positions[99], hand.GetComponent<MonoBehaviour>());
            last_hand_positions.RemoveAt(0);
        }
        hand.GetComponent<MonoBehaviour>().StartCoroutine(UpdateHandPositions());
    }
}