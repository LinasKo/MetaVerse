using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
	NetworkClient myClient;

	void Update() {
		if (Cardboard.SDK != null) {
			SendInfo (1, Cardboard.SDK.HeadPose);
		}
	}
	
	public class MyMsgType {
		public static short Info = MsgType.Highest + 1;
	};
	
	public class InfoMessage : MessageBase
	{
		public int id;
		public Pose3D pose;
	}
	
	public void SendInfo(int personID, Pose3D headPose)
	{
		InfoMessage msg = new InfoMessage();
		msg.id = personID;
		msg.pose = headPose;
		
		NetworkServer.SendToAll(MyMsgType.Info, msg);
	}
	
	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = new NetworkClient();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);
		myClient.RegisterHandler(MyMsgType.Info, OnInfo);
		myClient.Connect("10.250.235.162", 3000);
	}
	
	public void OnInfo(NetworkMessage netMsg)
	{
		InfoMessage msg = netMsg.ReadMessage<InfoMessage>();
		Debug.Log("OnInfoMessage: id=" + msg.id + "pose=" + msg.pose);
	}
	
	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log("Connected to server");
	}
}