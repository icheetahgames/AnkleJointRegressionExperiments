using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    InputController inputController;
    [SerializeField] private Text MPU_1_text;
    [SerializeField] private GameObject MPU_1;
    private float x_1, y_1, z_1;
    void Start()
    {
        //This will do the network stuff
        inputController = new InputController();
        inputController.Begin("192.168.4.1", 80);
    }

    void Update()
    {
        x_1 = (float)Math.Round(inputController.x_1, 1);
        y_1 = (float)Math.Round(inputController.y_1, 1);
        z_1 = (float)Math.Round(inputController.z_1, 1);
        
        MPU_1_text.text = "MPU_1 // "+" x : " + x_1.ToString() + "\ty : " + y_1.ToString() + "\tz : " + z_1.ToString();
        MPU_1.transform.rotation = Quaternion.Euler(y_1,  0, x_1);

    }


}
