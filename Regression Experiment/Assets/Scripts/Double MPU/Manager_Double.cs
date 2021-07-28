using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Double : MonoBehaviour
{
    InputController_Double inputController;
    [SerializeField] private Text MPU_1_text;
    [SerializeField] private Text MPU_2_text;
    [SerializeField] private Text Connection;
    [SerializeField] private Text angle;
    [SerializeField] private Text angleA;
    [SerializeField] private Text time;
    [SerializeField] private Text _dataSaving;
    [SerializeField] private GameObject MPU_1;
    [SerializeField] private GameObject MPU_2;
    
    private float x_1, y_1, z_1, x_2, y_2, z_2, Arangle;

    #region Data Saving To txt file vaiables

    private string date,date_unix, data, title, path;

    private string year, month, day, hour, min, sec;

    private int i = 0;

    [SerializeField] private bool saveData = false;
    
    #endregion


    void Start()
    {
        //This will do the network stuff
        inputController = new InputController_Double();
        inputController.Begin("192.168.4.1", 80);
        Connection.text = "Connection : " + inputController.StateClient;

        
        #region Data Saving To txt file

        year = DateTime.Now.ToString("yyyy");
        month = DateTime.Now.ToString("MM");
        day = DateTime.Now.ToString("dd");
        hour = DateTime.Now.ToString("HH");
        min = DateTime.Now.ToString("mm");
        sec = DateTime.Now.ToString("ss");
        title ="ESP32 " + year + "-" + month + "-" + day + "-" + hour + "-" + min + "-" + sec;
        path = "I:/unity projects/ESP32/Logs/ESP32/";

        #endregion

        _dataSaving.text = "MPU Data Saving : " + saveData.ToString();
    }

    void FixedUpdate()
    {
        _dataSaving.text = "MPU Data Saving : " + saveData.ToString();
        x_1 = (float)Math.Round(inputController.x_1, 1);
        y_1 = (float)Math.Round(inputController.y_1, 1);
        z_1 = (float)Math.Round(inputController.z_1, 1);
        
        x_2 = (float)Math.Round(inputController.x_2, 1);
        y_2 = (float)Math.Round(inputController.y_2, 1);
        z_2 = (float)Math.Round(inputController.z_2, 1);
        Arangle = (float)Math.Round(inputController.Arangle, 1);


//        print("Foot Anlge : " +  x_1);
//        print("Shank Anlge : " +  x_2);
        MPU_1_text.text = "MPU_1 // "+" x : " + x_1.ToString() + "\ty : " + y_1 + "\tz : " + z_1.ToString();
        //MPU_1.transform.rotation = Quaternion.Euler(y_1,  0, x_1); // Activate this line if you want to get the rotation about 2 axes
        MPU_1.transform.rotation = Quaternion.Euler(0,  0,  y_1); // foot
        MPU_2_text.text = "MPU_2 // "+" x : " + x_2.ToString() + "\ty : " + y_2 + "\tz : " + z_2.ToString();
        //MPU_2.transform.rotation = Quaternion.Euler(y_2,  0, x_2);    // Activate this line if you want to get the rotation about 2 axes
        MPU_2.transform.rotation = Quaternion.Euler(0,  0, 90 + y_2 ); // shank
        angle.text = "Angle : " + (y_1 - y_2).ToString();
        angleA.text = "Angle A  : " + Arangle;
        

        #region Data saving to txt file

        //date = DateTime.Now.ToString("yyyy'-'MM'-'dd','HH':'mm':'ss.fffffffK");
        date = DateTime.Now.ToString("MM'-'dd','HH':'mm':'ss.fffffffK");
        time.text = date;
        date_unix = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds. ToString();
        data = i.ToString() +","+ date +","+ date_unix + "," + y_1.ToString() + "," +  y_2.ToString();
        if (saveData)
        {
            System.IO.File.AppendAllText(path + title + ".csv",data+ "\n");
        }
        ++i;

        #endregion
    }

    public void StartSavingData()
    {
        print("Start Saving Orientation data");
        saveData = true;
    }

    public void StopSavingData()
    {
        print("Start Saving Orientation data");
        saveData = false;
    }
}
