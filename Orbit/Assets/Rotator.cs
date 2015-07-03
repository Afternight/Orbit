using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	private float y=0f;
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("Reset")==0){
			transform.localScale=new Vector3(0f,0f,0f);
		} else {
			transform.localScale=new Vector3(0.24f,0.24f,0.24f);
		}
		y=PlayerPrefs.GetFloat("Rotate"); //loads rotation value
	}
	
	// Update is called once per frame
	void Update () {
		y=y+0.1f;
		transform.rotation = Quaternion.Euler (0f, 0f, y);
		if ((PlayerPrefs.GetInt("Reset")==0)&&(transform.localScale.x!=0.24)&&(transform.localScale.y!=0.24)){
			transform.localScale+= new Vector3(0.01f,0.01f,0.01f);
		}
	}

	void OnCollisionEnter2D(Collision2D coll){
		PlayerPrefs.SetFloat("Rotate",y); //stores current rotation value
		PlayerPrefs.SetInt("Reset",1);
		Application.LoadLevel (Application.loadedLevel); //resets level 
	}
}
