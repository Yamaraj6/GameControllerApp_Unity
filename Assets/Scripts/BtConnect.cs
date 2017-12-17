using UnityEngine;
using System;

public class BtConnect
{
#if !UNITY_EDITOR
    private AndroidJavaObject current_activity;
    private AndroidJavaObject bt_manager;

    public BtConnect()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        current_activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        bt_manager = new AndroidJavaObject("com.unityplugin.bluetoothplugin.BluetoothManager",
                                      current_activity);
    }
   
    public void BluetoothOn()
    {
            bt_manager.Call("EnableDisableBluetooth");
    }

    public void MakeDeviceDiscoverable()
    {
            bt_manager.Call("MakeDeviceDiscoverable", 200);
    }

    public void DiscoverDevices()
    {
            bt_manager.Call("DiscoverDevices");
    }

    public String sListOfPairedDevices()
    {
        return bt_manager.Call<String>("getPairedDevices");
    }

    public String sGetListOfDiscoveredDevices()
    {
        return bt_manager.Call<String>("getDiscoveredDevices"); 
    }

    public void BindWithDevice()
    {
            bt_manager.Call("BindWithDevice", 0);
    }

    public void StartBtConnection()
    {
            bt_manager.Call("StartBtConnection", "Hand Controller", "00001101-0000-1000-8000-00805F9B34FB");
    }

    public bool SendData(String sDataToSend)
    {
        bool _bDataSend = false;
        // PINKY;RING;MID;INDEX;THUMB _sExampleData = "<1;0.5;0.64;0.94;0;1>";
        current_activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            bt_manager.Call("Send", sDataToSend);
            _bDataSend = true;
        }));
        return _bDataSend;
    }

    public String sReceiveData()
    {
        // (QUATERNION: X; Y; Z; W)(ACCELERATION: X; Y; Z)(FINGER FLEX: THUMB; INDEX; MID; RING; PINKY)
        String temp = "0.00;0.00;0.00;0.00;0;0;0;0.00;0.00;0.00;0.00;0.00";
        temp = bt_manager.Call<String>("sReceiveData");
        return temp;
    }

    public bool bIsConnected()
    {
        return bt_manager.Call<Boolean>("bIsConnected");
    }

    public bool bDeviceHasBeenFound()
    {
        return bt_manager.CallStatic<Boolean>("bDeviceHasBeenFound");
    }
#endif
}