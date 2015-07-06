using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Vector2 y = new Vector2 (0,0.1f);
	private Vector2 mousePosition;
	public Vector3 impact = new Vector3(0,0,0);
	public float maxGravDist = 100f;
	public float maxGravity = 0.5f;
	public ContactPoint2D[] rocketpoint;
	//public float gravitational=0.0000000000667f;
	GameObject[] planets;
	private GameObject controller;
	private GameController control;
	//private int fuel;
	Animator An;

	// Use this for initialization
	void Start () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		//in future make reference to originalfuel level set by level by accessing level data script
		//fuel=control.fuel;
		planets = GameObject.FindGameObjectsWithTag("Planet");
		An=GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if (Input.GetMouseButton (0)&&control.fuel>0&&control.paused==false) { //when mouse is held down NEED TO ADD EXCEPTION FOR PAUSEbutton
			An.SetBool("Active",true);
			control.fuel=control.fuel-1*Time.deltaTime;
			Rigidbody2D x = GetComponent<Rigidbody2D> ();
			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			x.AddRelativeForce (y, ForceMode2D.Impulse);
		} else {
			An.SetBool("Active",false);
		}
		control.time=control.time+1*Time.deltaTime;
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
