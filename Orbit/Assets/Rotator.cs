using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	private float y=0f;
	// Use this for initialization
	void Start () {
		y=PlayerPrefs.GetFloat("Rotate"); //loads rotation value
	}
	
	// Update is called once per frame
	void Update () {
		y=y+0.1f;
		transform.rotation = Quaternion.Euler (0f, 0f, y);
	}

	void OnCollisionEnter2D(Collision2D coll){
		PlayerPrefs.SetFloat("Rotate",y); //stores current rotation value
		Application.LoadLevel (Application.loadedLevel); //resets level 
	}
}
