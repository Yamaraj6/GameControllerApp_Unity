using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBuilder : MonoBehaviour
{
    [SerializeField]
    private Text txt_tutorial;

    [SerializeField]
    private Button connect_controller;

    [SerializeField]
    private GameObject hand_controller;
    private Controller controller;

    void Start ()
    {
        controller = hand_controller.GetComponent<Controller>();
    }

    void Update()
    {
        if (!controller.IsConnected())
        {
            connect_controller.gameObject.SetActive(true);
        } else
        {
            connect_controller.gameObject.SetActive(false);
//            if(controller)
            txt_tutorial.gameObject.SetActive(true);
        }

    }
    
}