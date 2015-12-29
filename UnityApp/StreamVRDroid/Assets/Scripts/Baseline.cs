using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

using Vuforia;

class Tracker2: MonoBehaviour, ITrackableEventHandler {
	
	ConcurQueue<int> queue = new ConcurQueue<int> ();
	int[][] keys = new int[][]{new int[]{2,3}, new int[]{1,3}, new int[]{1,2}};
	string[] str = new string[]{"it1/ob1", "it2/ob2", "it3/ob3"};
	TrackableBehaviour tr;
	int me;

	public Tracker2(TrackableBehaviour b, int i, ConcurQueue<int> q){
		tr = b;
		me = i;
		queue = q;
		if (tr) {
			tr.RegisterTrackableEventHandler (this);
		}
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
		bool stateDelta = newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED;
		if (stateDelta){
			ASKWorker askworker = new ASKWorker ();
			Thread clientThread = new Thread(() => askworker.FetchObjects(queue, keys[me]));
			clientThread.Start ();
		}

	} 
}

public class Baseline : MonoBehaviour {

	bool pastStart = false;
	ASKWorker askWorker = new ASKWorker ();
	ConcurQueue<int> queue = new ConcurQueue<int> ();

	Dictionary<int, string> curList = new Dictionary<int, string>();

	Tracker2[] trackme = new Tracker2[3];
	string[] str = new string[]{"it1/ob1", "it2/ob2", "it3/ob3"};
	string[] str2 = new string[]{"it1", "it2", "it3"};

	void Start () {
		for(int i = 0; i<1; i++){
			int oId = askWorker.InsertObject (GameObject.Find (str [i]).SaveObjectTree (), new float[]{ i * 100, i * 100 });
			if(oId > 0)
				curList.Add(oId, str[i]);
			GameObject.Find (str [i]).transform.localPosition = new Vector3 (1000, 1000, 1000);
			new WaitForSeconds (1);
		}
		
		for (int i = 0; i < 1; i++) {
			trackme [i] = new Tracker2(GameObject.Find (str2 [i]).GetComponent<TrackableBehaviour> (), i, queue);
		}

		pastStart = true;
		Debug.Log ("End of the Beginning.");
	}

	void Update () {
		if (pastStart) {
			while (queue.Count > 0) {
				int newobj = queue.Dequeue ();
				GameObject.Find (curList [newobj]).transform.localPosition = new Vector3 (0, 0, 0);
			}
		}
	}
}
