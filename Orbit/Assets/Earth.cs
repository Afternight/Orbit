using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {
	private float y=0f;
	private Vector2 velocity=new Vector2(0,0);
	public float speed;
	private Collider2D Base;
	private GameObject controller;
	private GameObject player;
	private Player playerscript;
	private Rigidbody2D body;
	private HingeJoint2D joint;
	private GameController control;
	private ContactPoint2D[] contacting;
	private JointAngleLimits2D jointlimit;
	private GameObject launch;
	private launcher launchscript;
	Animator An;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		control=controller.GetComponent<GameController>();
		if(control.GameStatus!=6){ 
			y=y+0.1f;
			transform.rotation = Quaternion.Euler (0f, 0f, y);
		}
	}

	void OnCollisionEnter2D(Collision2D coll){
		controller=GameObject.Find ("GameController");
		control=controller.GetComponent<GameController>();
		//control.launched=false;
		player=GameObject.Find("Rocket");
		An=player.GetComponent<Animator>();
		playerscript=player.GetComponent<Player>();
		body=player.GetComponent<Rigidbody2D>();
		Base=player.GetComponent<EdgeCollider2D>();
		velocity=body.velocity;
		speed=Mathf.Sqrt((velocity.x*velocity.x)+(velocity.y*velocity.y));
		jointlimit.max=0f;
		jointlimit.min=359f;
		if ((speed<1f)&&(coll.collider==Base)){
			//contacting=coll.contacts;
			if (control.hookedalpha==false){
				//create first hinge joint
				joint=gameObject.AddComponent<HingeJoint2D>();
				joint.enableCollision=true;
				joint.useLimits=true;
				joint.limits=jointlimit;
				joint.connectedBody=coll.rigidbody;
				joint.anchor=gameObject.transform.InverseTransformPoint(playerscript.rocketpoint[0].point);
				joint.connectedAnchor=playerscript.impact;
				player.transform.SetParent(gameObject.transform);
				control.hookedalpha=true;
			}
			Invoke("Success",3);
		} else {
			An.SetBool("Collision",true);
			control.camhook=false;
			control.GameStatus=7;
			//Reset();
		}
	}

	void Success(){
		controller=GameObject.Find ("GameController");
		control=controller.GetComponent<GameController>();
		control.GameStatus = 5; //assign success status
	}
}