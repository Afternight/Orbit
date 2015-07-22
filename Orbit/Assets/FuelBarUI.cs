using UnityEngine;
using System.Collections;

public class FuelBarUI : MonoBehaviour {
	private GameObject controller;
	private GameController control;
	public int hash=Animator.StringToHash("Launchtofuel");
	public int hash2;
	// Use this for initialization
	void Start () {
		hash2=Animator.StringToHash("fuel");
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		control.animID=hash;
		control.animIDreset=hash2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
