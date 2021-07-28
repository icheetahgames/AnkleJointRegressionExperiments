using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Principal;
using System;

public class TrySavingData : MonoBehaviour
{
    private int i = 0;
    private int g = 5;
    private int b = 2;
    
    private string date;

    private string year, month, day, hour, min, sec;

    private string title;

    private string path;
    // Start is called before the first frame update
    void Start()
    {
        year = DateTime.Now.ToString("yyyy");
        month = DateTime.Now.ToString("MM");
        day = DateTime.Now.ToString("dd");
        hour = DateTime.Now.ToString("HH");
        min = DateTime.Now.ToString("mm");
        sec = DateTime.Now.ToString("ss");
        title = year + "-" + month + "-" + date + "-" + hour + "-" + min + "-" + sec;
        path = "I:/unity projects/ESP32/Logs/";
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //System.IO.File.AppendAllText("D:/Unity Projects/ESP32/Assets/Scripts/"+"t" + g.ToString() +".txt", i.ToString() + ",");
        //System.IO.File.AppendAllText("D:/Unity Projects/ESP32/Assets/Scripts/t.txt", b.ToString() + "\n");
        date = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK");
        System.IO.File.AppendAllText(path + title + ".txt",i.ToString() + " , " + date + "\n");
        print(i);
        ++i;
    }
}
