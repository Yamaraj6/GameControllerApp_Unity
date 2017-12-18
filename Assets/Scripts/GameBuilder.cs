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
    private GameObject hand_controller;
    private Controller controller;
    private float timer;

    void Start ()
    {
        controller = hand_controller.GetComponent<Controller>();
    }

    void Update()
    {
        if (controller.IsConnected() && timer<=5)
        {
            txt_tutorial.gameObject.SetActive(true);
        } else
        {
            txt_tutorial.gameObject.SetActive(false);
        }

    }
    
}