using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {
	public Vector2 y = new Vector2 (0,0.1f);
	private Vector2 mousePosition;
	public Vector3 impact = new Vector3(0,0,0);
	public Vector2 centermass= new Vector2(0,-2);
	public ContactPoint2D[] rocketpoint;
	//public float gravitational=0.0000000000667f;
	private GameObject controller;
	private GameController control;
	private Rigidbody2D center;

	//Pause
	private int previousstate=0;
	// Use this for initialization
	void Start () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		//in future make reference to originalfuel level set by level by accessing level data script
		//fuel=control.fuel;
		center=GetComponent<Rigidbody2D>(); //offsets rocket center of mass for some reason, probs to be better
		center.centerOfMass=centermass;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public void Pause(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if (control.GameStatus!=6){
			previousstate=control.GameStatus;
			control.GameStatus=6;
			Time.timeScale=0;
		} else if (control.GameStatus==6) {
			control.GameStatus=previousstate;
			Time.timeScale=1;
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		rocketpoint=coll.contacts;
		impact=gameObject.transform.InverseTransformPoint(rocketpoint[0].point);
		
	}
}
