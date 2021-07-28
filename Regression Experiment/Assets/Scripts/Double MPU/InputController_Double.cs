using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public class InputController_Double : MonoBehaviour
{
    public float x_1, y_1, z_1, x_2, y_2, z_2, Arangle;
    public bool StateClient;

    public void Begin(string ipAddress, int port)
    {
        print("TCP Client Start");
        try
        {
      var thread = new Thread(() =>
        {
            //This class makes it super easy to do network stuff
            var client = new TcpClient();
            //Change this to your real device address
            client.Connect(ipAddress, port);
            var stream = new StreamReader(client.GetStream());
            //We'll read values and buffer them up in here
            var buffer = new List<byte>();
            StateClient = client.Connected;
            print("StateClient : " + StateClient);
            while (client.Connected)
            {
                //Read the next byte
                var read = stream.Read();
                //We split readings with a carriage return, so check for it
                if (read == 13)
                {
                    //Once we have a reading, convert our buffer to a string, since the values are comming as strings
                    var str = Encoding.ASCII.GetString(buffer.ToArray());
                    string[] MPUs = str.Split(',');
                   // print("MPUs.Length : " + MPUs.Length);
                   // string[] MPU_1 = MPUs[0].Split('/');
                    //print("MPU_1.length : " + MPU_1.Length);
                  //  string[] MPU_2 = MPUs[1].Split('/');
                    //print("MPU_2.Length : " + MPU_2.Length);
                    //x_1    = float.Parse(MPU_1[0]);
                    y_1    = float.Parse(MPUs[0]);
                    //z_1    = float.Parse(MPU_1[2]);
                    
                    //x_2    = float.Parse(MPU_2[0]);
                    y_2    = float.Parse(MPUs[1]);
                    //z_2    = float.Parse(MPU_2[2]);
                    //Arangle = float.Parse(MPUs[3]);


                    //Clear the buffer ready for another reading
                    buffer.Clear();
                }
                else
                    //if this was not the end of a reading, then just add this new byte to our buffer
                    buffer.Add((byte)read);
            }
            print("DisConnected");
        });

        thread.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        //Give the network stuff its own special thread
  
    }
}
