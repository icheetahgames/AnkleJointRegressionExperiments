using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class TCP : MonoBehaviour
{
    private bool socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    [SerializeField] private Text _espConnection;
    [SerializeField] String Host = "192.168.4.1";
    [SerializeField] Int32 Port = 80;
    // Start is called before the first frame update
    void Start()
    {
        setupSocket();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            _espConnection.text = "ESP_Connected : " + (theStream.DataAvailable).ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        
        print(readSocket());
    }
    
    public void setupSocket() {
        print("Trying to connect");
        try {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e) {
            Debug.Log("Socket error: " + e);
        }
    }
    public void writeSocket(string theLine) {
        if (!socketReady)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }
    public String readSocket() {
        if (!socketReady)
            return "";
        if (theStream.DataAvailable)
            return theReader.ReadLine();
        return "";
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }
}
