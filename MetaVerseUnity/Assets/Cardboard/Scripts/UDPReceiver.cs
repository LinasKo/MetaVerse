/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

public class UDPReceiver : MonoBehaviour
{
    private int twisteeID = 9;
    private static GameObject point = null;
    private static GameObject cardboardView;
    // IP end point
    IPEndPoint receivePoint;

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // Neck of a person that will have his neck twisted :3
    Transform twistee;
    Quaternion headOrientation;

    // HeadOrientation
    ArrayList otherHeads;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!

    private bool allocateNow = false;
    private bool noCamera = true;

    // start from unity3d
    public void Start()
    {
        otherHeads = new ArrayList();
        otherHeads.Add(GameObject.Find("Thomas").transform.Find("hip/abdomen/chest/neck"));
        otherHeads.Add(GameObject.Find("Justin").transform.Find("Justin:Hips/Justin:Spine/Justin:Spine1/Justin:Spine2/Justin:Neck"));
        otherHeads.Add(GameObject.Find("Mia").transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck"));
        init();
    }

    public static void AllocateCamera()
    {
        cardboardView = GameObject.Find("CardboardMain");
        switch (UDPSender.id)
        {
            case (0):
                point = GameObject.FindGameObjectWithTag("View0");
                break;
            case (1):
                point = GameObject.FindGameObjectWithTag("View1");
                break;
            case (2):
                point = GameObject.FindGameObjectWithTag("View2");
                break;
        }
    }

    // TWIST NECKS
    public void Update()
    {
        // Twist main neck
        if (noCamera && allocateNow)
        {
            AllocateCamera();
            noCamera = false;
            allocateNow = false;
        }

        if (!noCamera)
        {
            cardboardView.transform.position = point.transform.position;
        }

        if (twisteeID != 9 && !noCamera)
        {
            twistee = (Transform)otherHeads[twisteeID];
            TwistNeck(twistee, headOrientation);
        }

        // Exit button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    // init
    private void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define port
        port = 5000;

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    IEnumerator CatchAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("last packet: " + getLatestUDPPacket());
    }

    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 5000);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                // ....
                allReceivedUDPPackets = allReceivedUDPPackets + text;

                // Action :3
                headOrientation = UnpackUDP(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }

    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }

    public Quaternion UnpackUDP(string packet)
    {
        UDPSender.id = (int) Char.GetNumericValue(packet[0]);
        twisteeID = (int) Char.GetNumericValue(packet[2]);
        if (noCamera)
        {
            allocateNow = true;
        }
        packet = packet.Substring(4);
        string[] temp = packet.Substring(1, packet.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        float w = float.Parse(temp[3]);
        Quaternion rValue = new Quaternion(x, y, z, w);
        return rValue;
    }

    public void TwistNeck(Transform neck, Quaternion twist)
    {
        neck.transform.rotation = twist;
    }
}

