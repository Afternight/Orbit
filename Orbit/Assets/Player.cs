﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {
	public Vector2 y = new Vector2 (0,0.1f);
	private Vector2 mousePosition;
	public Vector3 impact = new Vector3(0,0,0);
	public Vector2 centermass= new Vector2(0,-2);
	public float maxGravDist = 100f;
	public float maxGravity = 0.5f;
	public ContactPoint2D[] rocketpoint;
	//public float gravitational=0.0000000000667f;
	GameObject[] planets;
	private GameObject controller;
	private GameController control;
	private Rigidbody2D center;

	// Use this for initialization
	void Start () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		//in future make reference to originalfuel level set by level by accessing level data script
		//fuel=control.fuel;
		planets = GameObject.FindGameObjectsWithTag("Planet");
		center=GetComponent<Rigidbody2D>();
		center.centerOfMass=centermass;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void FixedUpdate () {
		foreach(GameObject planet in planets) { //iterates through planets
			float dist = Vector3.Distance(planet.transform.position, transform.position);
			if (dist <= maxGravDist) {
				maxGravity= 0.3f;//need to modify
				maxGravity=maxGravity/dist*2;// distance gets smaller!!!!
				Vector3 v = planet.transform.position - transform.position;
				GetComponent<Rigidbody2D>().AddForce(v.normalized * (1.0f - dist / maxGravDist) * maxGravity,ForceMode2D.Impulse); //change max gravity based on distance from object
			}
		}
	}

	public void Pause(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if (control.paused==false){
			control.paused=true;
			Time.timeScale=0;
		} else if (control.paused==true) {
			control.paused=false;
			Time.timeScale=1;
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		rocketpoint=coll.contacts;
		impact=gameObject.transform.InverseTransformPoint(rocketpoint[0].point);
		
	}
}
