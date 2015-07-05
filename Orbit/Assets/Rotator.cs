using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	private float y=0f;
	private GameObject controller;
	private GameController control;
	// Use this for initialization
	void Start () {
		y=PlayerPrefs.GetFloat("Rotate"); //loads rotation value
	}
	
	// Update is called once per frame
	void Update () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if(control.paused==false){ 
			y=y+0.1f;
			transform.rotation = Quaternion.Euler (0f, 0f, y);
		}
	}
	public void Reset(){
		PlayerPrefs.SetFloat("Rotate",y); //stores current rotation value
		PlayerPrefs.SetInt("Reset",1);
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		control.fuel=200f; //here make reference to level data script for fuel level to reset to
		Application.LoadLevel (Application.loadedLevel); //resets level
		//need to consider bug where planets arn't reset to original rotation values on reset when paused
		if (control.paused==true){
			/*y=PlayerPrefs.GetFloat("Rotate");
			y=y+0.1f;
			transform.rotation = Quaternion.Euler (0f, 0f, y);*/
			control.paused=false;
			Time.timeScale=1;
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		Reset();
	}

}
