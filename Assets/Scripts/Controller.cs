using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ConnectionStatus { Disconnected, Connecting, Connected};

public class Controller : MonoBehaviour
{
    private HandController hand_controller;
    private ConnectionStatus connection_status;
    private BtConnect bt_connector;

    private Quaternion rotation_received;
    private Vector3 acceleration_received;
    private float[] fingers_flex_received;

    private int calibrate;

    private Features feature;

    public GameObject spell_to_respawn;
    public GameObject gob_to_respawn;
    public GameObject testcube;
    public Text txt_debug;
    public Text txt_debug2;

    void Start()
    {
        hand_controller = new HandController(gameObject, spell_to_respawn, gob_to_respawn);
        
        connection_status = ConnectionStatus.Disconnected;

        rotation_received = new Quaternion();
        acceleration_received = new Vector3();
        fingers_flex_received = new float[5];
        calibrate = 0;

#if !UNITY_EDITOR
        StartCoroutine(ConnectController());
#endif


        feature = new Features(gameObject);
    }

    void Update()
    {
        txt_debug.text = GetDebugInfo();
        CommunicationUpdate();

        if (Input.GetMouseButton(0))
        {
            Calibrate();
            float z = 6.166702f+ 2.5f;
            var cubes = GameObject.FindGameObjectsWithTag("RigidBodyObject");
            foreach(var cube in cubes)
            {
                cube.transform.position = new Vector3(0, 3, z);
                z += 2.5f;
            }
        }

#if UNITY_EDITOR
        Test();
#endif
    }

    private String GetDebugInfo()
    {
        String temp = connection_status.ToString() + '\n';
        temp += "Hand Pos: ";
        temp += hand_controller.hand_position.ToString() + "\n";
        for (int i = 0; i < hand_controller.fingers.Length; i++)
            temp += hand_controller.fingers[i].GetFingerPosition() + " ";

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
        bt_connector = new BtConnect();
        bt_connector.DiscoverDevices();
        yield return new WaitUntil(() => bt_connector.bDeviceHasBeenFound());
        bt_connector.StartBtConnection();
        connection_status = ConnectionStatus.Connecting;
        yield return new WaitUntil(() => bt_connector.bIsConnected());
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
            UpdateRecivedData(null);
            hand_controller.ChangeRotation(rotation_received);
            hand_controller.MoveFingers(fingers_flex_received);
         //   SendData();
        }
    }

    public void Calibrate()
    {
        calibrate = 1;
    }

    public void SendData()
    {
        if (hand_controller.collider_power_data != "")
        {
            String _data_to_send = "<";
            _data_to_send += hand_controller.collider_power_data;
            _data_to_send += calibrate + ">";
#if !UNITY_EDITOR
        if (bt_connector.SendData(_data_to_send) && calibrate==1)
            calibrate = 0;
#endif
        }
    }

    private void UpdateRecivedData(String receivedData)
    {
        String _recived_data = receivedData;
#if !UNITY_EDITOR
        _recived_data = bt_connector.sReceiveData();
#endif
        int j = 0;
        string temp = "";
        if (_recived_data != null)
        {
            for (int i = 0; i <= _recived_data.Length; i++)
            {
                if (_recived_data.Length != i && _recived_data[i] != ';')
                {
                    temp += _recived_data[i];
                }
                else
                {
                    if (j < 4)
                        rotation_received[j] = float.Parse(temp);
                    else if (j < 7)
                        acceleration_received[j - 4] = float.Parse(temp);
                    else
                        fingers_flex_received[j - 7] = float.Parse(temp);
                    j++;
                    temp = "";
                }
            }
            String a="";
            for (int i = 0; i < 5; i++)
            {
                a += fingers_flex_received[i];
                a += ";";
            }
            txt_debug2.text = a;
        }
    }
    
    public void Test()
    {
        hand_controller.DrawRaycastLine();
        if (Input.GetKeyDown("o"))
            UpdateRecivedData("0.00;0.00;0.00;0.00;0;0;0;0.00;0.00;0.00;0.00;0.00");
        if (Input.GetKeyDown("f"))
            UpdateRecivedData("0.00;0.00;0.00;0.00;0;0;0;1.00;1.00;1.00;1.00;1.00");
        if (Input.GetKeyDown("t"))
            UpdateRecivedData("0.00;0.00;0.00;0.00;0;0;0;1.00;1.00;0.00;0.00;0.00");
        if (Input.GetKeyDown("h"))
            UpdateRecivedData("0.00;0.00;0.00;0.00;0;0;0;1.00;0.00;1.00;1.00;1.00");
        if (Input.GetKeyDown("b"))
            UpdateRecivedData("0.00;0.00;0.00;0.00;0;0;0;1.00;0.00;0.00;0.00;0.00");
        hand_controller.MoveFingers(fingers_flex_received);
    }
 }