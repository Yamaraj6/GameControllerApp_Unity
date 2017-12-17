using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBuilder : MonoBehaviour
{
    [SerializeField]
    private Text txt_temp;
    [SerializeField]
    private GameObject gob_hand;
    private Hand hand;

    private BtConnect bt_connector;
    private String s_recived_data;
    private float[] d_gyro_position;
    private float[] d_acc_data;
    private float[] d_fingers_position;
    private String s_data_to_send;

    int i_temp;
    bool b_calibrate;
    bool b_was_calibrated;

    void Start ()
    {
        d_gyro_position = new float[4];
        d_acc_data = new float[3];
        d_fingers_position = new float[5];
        bt_connector = new BtConnect();
        hand = new Hand(gob_hand);

        b_calibrate = false;
        b_was_calibrated = false;
        i_temp = 0;

        bt_connector.DiscoverDevices();
    }

    void Update()
    {
        if (i_temp == 0 && bt_connector.bDeviceHasBeenFound())
        {
            GameObject.Find("Cube").transform.localScale =new Vector3(2, 2, 3);
            bt_connector.StartBtConnection();
            i_temp++;
        }
        else if (i_temp == 1 && bt_connector.bIsConnected())
        {
            TransformRecivedData();
            hand.ChangeRotation(d_gyro_position);
            //hand.ChangePosition(d_acc_data);
            s_data_to_send = "<";
            s_data_to_send += hand.sMoveFingers(d_fingers_position);
            SendData();
        }
    }

    public void DiscoverDevices()
    {
        bt_connector.DiscoverDevices();
    }
    public void StartBt()
    {
        bt_connector.StartBtConnection();
        txt_temp.text = bt_connector.sGetListOfDiscoveredDevices();
    }
    public void SendData()
    {
        if (b_was_calibrated)
        {
            b_calibrate = false;
            b_was_calibrated = false;
        }
        s_data_to_send += b_calibrate ? 1 + ">" : 0 + ">";
        bt_connector.SendData(s_data_to_send);
    }

    // BUTTON FUNCTION
    public void Calibration()
    {
        b_calibrate = true;
    }
    
    private void TransformRecivedData()
    {
        s_recived_data = bt_connector.sReceiveData();
        int j = 0;
        string temp = "";
        if (s_recived_data != null)
        {
            for (int i = 0; i < s_recived_data.Length; i++)
            {
                if (s_recived_data[i] != ';')
                    temp += s_recived_data[i];
                else
                {
                    if (j < 4)
                        d_gyro_position[j] = float.Parse(temp);
                    else if (j >= 4 && j < 7)
                        d_acc_data[j - 4] = float.Parse(temp);
                    else if (j < 12)
                        d_fingers_position[j - 7] = float.Parse(temp);
                    else
                        b_was_calibrated = bool.Parse(temp);
                    j++;
                    temp = "";
                }
            }
        }
    }
}