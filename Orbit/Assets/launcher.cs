using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class launcher : MonoBehaviour {
	private GameObject controller;
	private GameController control;
	private DistanceJoint2D cable;
	private GameObject player;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Launch(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		player=GameObject.Find("Rocket");
		control=controller.GetComponent<GameController>();
		control.launched=true;
		cable=gameObject.GetComponent<DistanceJoint2D>();
		player.transform.SetParent(null);
		Destroy(cable);
	}
}
