using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;
using UnityEngine.UI;

public enum ConnectionStatus { Disconnected, Connecting, Connected};

public class Controller : MonoBehaviour
{
    private HandController hand_controller;
    private List<HandPosition> hand_positions;
    private List<HandRotation> hand_rotations;
    private ConnectionStatus connection_status;
    private BtConnect bt_connector;

    private Quaternion rotation_received;
    private float[] acceleration_received;
    private float[] fingers_flex_received;

    private int calibrate;

    public GameObject testcube;
    public Text txt_debug;

    void Start()
    {
        hand_controller = new HandController(gameObject);

#if !UNITY_EDITOR
        bt_connector = new BtConnect();
#endif
        connection_status = ConnectionStatus.Disconnected;

        rotation_received = new Quaternion();
        acceleration_received = new float[3];
        fingers_flex_received = new float[5];
        calibrate = 0;

#if !UNITY_EDITOR
        StartCoroutine(ConnectController());
#endif

        hand_positions = new List<HandPosition>();
        hand_rotations = new List<HandRotation>();

        StartCoroutine(UpdateHandPositions());
    }

    void Update()
    {
        txt_debug.text = GetDebugInfo();
        CommunicationUpdate();
        if (Input.GetMouseButton(0))
        {
            Calibrate();
        }
    }

    private String GetDebugInfo()
    {
        String temp = connection_status.ToString() + '\n';
        temp += "Hand Pos: ";
        temp += hand_controller.GetHandPosition().ToString() + "\n";
        temp += "Hand Rot: ";
        temp += hand_controller.GetHandRotation().ToString() + "\n"; ;
        for (int i = 0; i < hand_controller.GetFingers().Length; i++)
            temp += hand_controller.GetFingers()[i].GetFingerPosition() + " ";

        return temp;
    }

    public bool IsConnected()
    {
        return (connection_status == ConnectionStatus.Connected
            || connection_status == ConnectionStatus.Connecting);
    }

#if !UNITY_EDITOR
    public IEnumerator ConnectController()
    {
        bt_connector.DiscoverDevices();
        yield return new WaitUntil(() => bt_connector.bDeviceHasBeenFound() || (Time.time % 15 == 0 && Time.time > 1));
        bt_connector.StartBtConnection();
        connection_status = ConnectionStatus.Connecting;
        yield return new WaitUntil(() => bt_connector.bIsConnected() || (Time.time % 15 == 0 && Time.time > 1));
        if (bt_connector.bIsConnected())
            connection_status = ConnectionStatus.Connected;
        else
            connection_status = ConnectionStatus.Disconnected;
        yield return new WaitUntil(() => !bt_connector.bIsConnected());
        connection_status = ConnectionStatus.Disconnected;
        ConnectController();
    }
#endif

    private void CommunicationUpdate()
    {
        if (connection_status == ConnectionStatus.Connected)
        {
            UpdateRecivedData();
            hand_controller.ChangeRotation(rotation_received);
            // hand_controller.ChangePosition(acc_data);
            SendData();
        }
    }

    public void Calibrate()
    {
        calibrate = 1;
    }

    public void SendData()
    {
        String _data_to_send = "<";
        _data_to_send += hand_controller.MoveFingers(fingers_flex_received);
        _data_to_send += calibrate + ">";
#if !UNITY_EDITOR
        if (bt_connector.SendData(_data_to_send) && calibrate==1)
            calibrate = 0;
#endif
    }

    private void UpdateRecivedData()
    {
        String _recived_data = null;
#if !UNITY_EDITOR
        _recived_data = bt_connector.sReceiveData();
#endif
        int j = 0;
        string temp = "";
        if (_recived_data != null)
        {
            for (int i = 0; i < _recived_data.Length; i++)
            {
                if (_recived_data[i] != ';')
                    temp += _recived_data[i];
                else
                {
                    if (j < 4)
                        rotation_received[j] = float.Parse(temp);
                    else if (j >= 4 && j < 7)
                        acceleration_received[j - 4] = float.Parse(temp);
                    else
                        fingers_flex_received[j - 7] = float.Parse(temp);
                    j++;
                    temp = "";
                }
            }
        }
    }

    private IEnumerator UpdateHandPositions()
    {
        hand_positions.Add(hand_controller.GetHandPosition());
        hand_rotations.Add(hand_controller.GetHandRotation());
        yield return new WaitForSeconds(0.01f);
        if(hand_positions.Count>100)
        {
            RecognizeGesture();
            hand_positions.RemoveAt(0);
            hand_rotations.RemoveAt(0);
        }
        StartCoroutine(UpdateHandPositions());
    }

    private void RecognizeGesture()
    {
        if (hand_positions.First<HandPosition>() == HandPosition.Open &&
            hand_positions.Last<HandPosition>() == HandPosition.Fist)
        {
            GameObject go = Instantiate(testcube);
            go.SetActive(true);
        }

    }
}