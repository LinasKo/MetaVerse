
/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSender : MonoBehaviour
{ 
    private static int localPort;
    private float sendingDelay = 0.1f;

    // prefs
    private string IP;  // define in init
    public int port;  // define in init
    public static int id = 9;

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage = "";

    // start from unity3d
    public void Start()
    {
        init();
        StartCoroutine(SendAfterSeconds(id + "," + id + "," + Cardboard.SDK.HeadPose.Orientation, sendingDelay));
    }

    // init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define
        IP = "10.250.235.162";
        port = 4000;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // sendData
    private void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString(testStr);


        }
        while (true);

    }

    IEnumerator SendAfterSeconds(string text, float time)
    {
        yield return new WaitForSeconds(time);
        sendString(text);
        StartCoroutine(SendAfterSeconds(id + "," + id + "," + Cardboard.SDK.HeadPose.Orientation, sendingDelay));
    }

}