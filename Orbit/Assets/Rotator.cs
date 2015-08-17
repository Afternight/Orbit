using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	private float y=0f;
	private GameObject controller;
	private GameObject player;
	private GameController control;
	Animator An;
	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(gameObject);
		//y=PlayerPrefs.GetFloat("Rotate"); //loads rotation value
	}
	
	// Update is called once per frame
	void Update () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if(control.GameStatus!=6){ 
			y=y+0.1f;
			transform.rotation = Quaternion.Euler (0f, 0f, y);
		}
		/*if (control.powered==true&&control.hookedalpha==true){
			Destroy(joint);
			player.transform.SetParent(null);
			control.hookedalpha=false;
		}*/
	}
	public void Reset(){ //legacy for reset button and rotate storage at the moment
		//PlayerPrefs.SetFloat("Rotate",y); //stores current rotation value
		//Gamecontroller resets
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		control.ResetControl();
	}
	void OnCollisionEnter2D(Collision2D coll){
		controller=GameObject.Find ("GameController");
		control=controller.GetComponent<GameController>();
		player=GameObject.Find("Rocket");
		An=player.GetComponent<Animator>();
		An.SetBool("Collision",true);
		control.camhook=false;
		control.GameStatus=7;
	}
}
