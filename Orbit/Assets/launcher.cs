using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class launcher : MonoBehaviour {
	private GameObject controller;
	private GameController control;
	private DistanceJoint2D cable;
	private GameObject player;
	private float force=0f;
	private Vector2 power = new Vector2 (0,0);
	
	private GameObject trajectory;
	public Sprite trajectorysolid;
	public Sprite trajectorytrans;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	private void LaunchTimed(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		player=GameObject.Find("Rocket");
		trajectory=GameObject.Find ("Trajectory");
		if (control.camposready){
			cable=gameObject.GetComponent<DistanceJoint2D>();
			player.transform.SetParent(null);
			Destroy(trajectory); // need to change to fade away possibly
			Destroy(cable);
			force=control.ObtainScale();
			power.y=force;
			Rigidbody2D x = player.gameObject.GetComponent<Rigidbody2D>();
			x.AddRelativeForce (power, ForceMode2D.Impulse);
			control.GameStatus=3;
			//control.camhook=false;
			CancelInvoke("LaunchTimed");
		}
	}

	public void Launch(){
		//put series of invokes at different intervals for countdown
		Invoke ("LaunchTimed",3);
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		control.FuelUi.SetTrigger(control.animID);
		control.GameStatus=2;
		control.camhook=false;
		//should add disabling of certain aspects
	}
}
