using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class ManagerFinal : MonoBehaviour
{
    public bool StateClient;
    // Start is called before the first frame update
    void Start()
    {
        var client = new TcpClient();
        client.Connect("192.168.4.1", 80);
        var stream = new StreamReader(client.GetStream());
        var buffer = new List<byte>();
        StateClient = client.Connected;
        print("StateClient : " + StateClient);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
