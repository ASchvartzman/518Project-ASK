using UnityEngine;
using System.Collections;
using Vuforia;

public class NewBehaviourScript : MonoBehaviour {

	GameObject chan;

	// Use this for initialization
	void Start () {
		chan = GameObject.Find ("unitychan").gameObject;
		Debug.Log ("It begins!");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Return)) {
			Debug.Log ("Woohoo!");
		}
	}
}
