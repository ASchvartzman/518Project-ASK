using UnityEngine;

using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using ASKLib;

public class DataSend : MonoBehaviour {
	bool pastStart = false;
	ASKWorker askWorker = new ASKWorker ();
	Thread clientThread;
	ConcurQueue<int> queue = new ConcurQueue<int> ();

	Dictionary<int, string> curList = new Dictionary<int, string>();

	string[] str = new string[]{"it1/ob1", "it2/ob2", "it3/ob3"};

	void Start () {	
		for(int i = 0; i<str.Length; i++){
			int oId = askWorker.InsertObject (GameObject.Find (str [i]).SaveObjectTree (), new float[]{ i * 100, i * 100 });
			if(oId > 0)
				curList.Add(oId, str[i]);
			GameObject.Find (str [i]).transform.localPosition = new Vector3 (1000, 1000, 1000);
		}
			
		clientThread = new Thread (() => askWorker.FetchObjects(queue, new int[0]));
		clientThread.Start ();
		pastStart = true;
		Debug.Log ("End of the Beginning.");
	}

	void Update () {
		if (pastStart) {
			while (queue.Count > 0) {
				int newobj = queue.Dequeue ();
				GameObject.Find (curList [newobj]).transform.localPosition = new Vector3 (0, 0, 0);
			}
			if (!clientThread.IsAlive) {
				int[] keys = new int[curList.Count];
				curList.Keys.CopyTo (keys, 0);
				clientThread = new Thread (() => askWorker.FetchObjects (queue, keys));
				clientThread.Start ();
			}
		}
	}
}
