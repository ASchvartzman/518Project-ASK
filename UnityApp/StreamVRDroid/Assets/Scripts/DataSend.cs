using UnityEngine;

using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using ASKLib;

public class DataSend : MonoBehaviour {

	void Start () {
		BinaryFormatter bf = new BinaryFormatter ();
		MemoryStream ms = new MemoryStream ();

		string[] str = new string[]{"ImageTarget", "ImageTarget (1)", "ImageTarget (2)"};
		for(int i = 0; i<str.Length; i++){
			Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect (new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));

			byte[] obj = GameObject.Find(str[i]).SaveObjectTree();
			InsertQuery iq = new InsertQuery(new AskObject(new float[]{i*100,i*100}, obj));

			ms = new MemoryStream();
			bf.Serialize(ms, iq);
			socket.Send(ms.ToArray());
			socket.Close();
		}
	}

	void Update () {

	}
}
