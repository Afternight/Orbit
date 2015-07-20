using UnityEngine;
using System.Collections;

public class FuelPower : MonoBehaviour {
	private float y=0f;
	public int fuelamount=3;
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
		if (control.GameStatus==6){
			y=y+0.1f;
			transform.rotation = Quaternion.Euler (0f, 0f, y);
		}

	}

	void OnTriggerEnter2D(Collider2D coll){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		control.fuel=control.fuel+fuelamount;
		Destroy(gameObject);
	}
}
