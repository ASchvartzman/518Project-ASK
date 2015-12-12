using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using Vuforia;
using Newtonsoft.Json;
using AskTest;

class ConcurQueue<T> {

	private Queue<T> queue = new Queue<T> ();

	public int Count {
		get {
			return queue.Count;
		}
	}

	public void Enqueue (T obj) {
		lock(queue) {
			queue.Enqueue(obj);
			Monitor.PulseAll(queue);
		}
	}

	public T Dequeue () {
		T t;
		lock (queue) {
			t = queue.Dequeue ();
			Monitor.PulseAll(queue);
		}
		return t;
	}
}

class ASKWorker {

	public void FetchObjects (ConcurQueue<byte[]> queue) {
		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint(IPAddress.Parse("10.9.101.248"), 1234));
		FetchQuery fq = new FetchQuery (new float[]{0,0}, new float[]{0,0}, 1000, 1000, new int[0]);
		Tuple2<string, string> fuqq = new Tuple2<string, string> ("FetchQuery", JsonConvert.SerializeObject(fq));
		socket.Send (Encoding.ASCII.GetBytes(JsonConvert.SerializeObject (fuqq)));
		byte[] instream = new byte[100000];
		int rec = socket.Receive (instream);
		string inrec = Encoding.ASCII.GetString (instream);
		Tuple2<string, string> unfold = JsonConvert.DeserializeObject<Tuple2<string, string> > (inrec);
		if (!unfold.Item1.Equals ("ObjectResult")) {
			Debug.Log ("Wrong type of object received.");
			return;
		}
		Debug.Log ("Fuck me.");
		ObjectResult or = JsonConvert.DeserializeObject<ObjectResult> (unfold.Item2);
		for (int i = 0; i<or.length; i++) {
			instream = new byte[100000];
			socket.Receive(instream);
			queue.Enqueue(instream);
		}
	}

	public void InsertObject(){
		byte[] data = GameObject.Find ("ImageTarget/Cube").SaveObjectTree ();

	}
}

public class Client : MonoBehaviour {
	LocationService location = new LocationService ();	
	ASKWorker askWorker = new ASKWorker ();
	Thread clientThread;
	ConcurQueue<byte[]> queue;

	// Use this for initialization
	void Start () {
		queue = new ConcurQueue<byte[]> ();
		clientThread = new Thread (() => askWorker.FetchObjects(queue));
		clientThread.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		while (queue.Count > 0){
			byte[] newobj = queue.Dequeue();
			newobj.LoadObjectTree();
		}
		if (!clientThread.IsAlive) {
			//clientThread = new Thread (() => askWorker.FetchObjects(queue));
			//clientThread.Start ();
		}
	}
}
