using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class launcher : MonoBehaviour {
	private GameObject controller;
	private GameController control;
	private DistanceJoint2D cable;
	private GameObject player;
	private float force=0f;
	private Vector2 power = new Vector2 (0,0);
	
	private GameObject trajectory;
	public Sprite trajectorysolid;
	public Sprite trajectorytrans;
    public GameObject launchbutton;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	private void LaunchTimed(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		player=GameObject.Find("Rocket");
		trajectory=GameObject.Find ("Trajectory");
        control.fadeneeded = false;
		if (control.camposready){
			cable=gameObject.GetComponent<DistanceJoint2D>();
			player.transform.SetParent(null);
			Destroy(trajectory); // need to change to fade away possibly TODO
			Destroy(cable);
			//force=control.ObtainScale(); //stubbed here
            force = control.trajectory.transform.localScale.y / 10;
			power.y=force;
			Rigidbody2D x = player.gameObject.GetComponent<Rigidbody2D>();
			x.AddRelativeForce (power, ForceMode2D.Impulse);
			control.GameStatus=3;
            control.fuelloading = false;
            control.fuel = control.DataPlay.InitialFuel[Application.loadedLevel];
            //control.camhook=false;
            CancelInvoke("LaunchTimed");
		}
	}

	public void Launch(){
		//put series of invokes at different intervals for countdown
		Invoke ("LaunchTimed",3);
        controller =GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
        control.fadevalue = -0.1f;
        control.fadeneeded = true;
        control.FuelUi.SetTrigger(control.animID);
		control.GameStatus=2;
		control.camhook=false;
        Invoke("FuelLoading", 1);

	}

    public void FuelLoading() { //purrfffeecctt
        controller = GameObject.Find("GameController"); //finds gamecontroller
        control = controller.GetComponent<GameController>();
        control.fuelloading = true;
        CancelInvoke("FuelLoading");
    }
}
