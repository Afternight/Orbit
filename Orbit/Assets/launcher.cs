using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class launcher : MonoBehaviour {
	private GameObject controller;
	private GameController control;
	private DistanceJoint2D cable;
	private bool uidetect=false;
	private GameObject player;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		/*controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if (Input.GetMouseButton (0)&&control.paused==false&&control.launched==false){
			uidetect=control.UiDetect();
			if (uidetect==true){
				Debug.Log ("insideoutside");
			} else {
				Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
				diff.Normalize ();
				float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			}
		}*/
	}

	public void Launch(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		player=GameObject.Find("Rocket");
		control=controller.GetComponent<GameController>();
		control.launched=true;
		cable=gameObject.GetComponent<DistanceJoint2D>();
		player.transform.SetParent(null);
		Destroy(cable);
	}
}
