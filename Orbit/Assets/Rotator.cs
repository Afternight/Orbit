using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	private float y=0f;
	private Vector2 velocity=new Vector2(0,0);
	public float speed;
	private Collider2D Poly;
	private Collider2D Base;
	private GameObject controller;
	private GameObject player;
	private Player playerscript;
	private Rigidbody2D body;
	private HingeJoint2D joint;
	private DistanceJoint2D joint2;
	private GameController control;
	private ContactPoint2D[] contacting;
	private JointAngleLimits2D jointlimit;
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
		control.hookedalpha=false;
		control.hookedbeta=false;
		Application.LoadLevel (Application.loadedLevel); //resets level
		if (control.paused==true){
			control.paused=false;
			Time.timeScale=1;
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		Debug.Log("collided");
		controller=GameObject.Find ("GameController");
		control=controller.GetComponent<GameController>();
		player=GameObject.Find("Rocket");
		playerscript=player.GetComponent<Player>();
		body=player.GetComponent<Rigidbody2D>();
		Poly=player.GetComponent<PolygonCollider2D>();
		Base=player.GetComponent<EdgeCollider2D>();
		velocity=body.velocity;
		speed=Mathf.Sqrt((velocity.x*velocity.x)+(velocity.y*velocity.y));
		jointlimit.max=0f;
		jointlimit.min=359f;
		if ((speed<2f)&&(coll.collider==Base)){
			contacting=coll.contacts;
			if (control.hookedalpha==false){
				//create first hinge joint
				joint=gameObject.AddComponent<HingeJoint2D>();
				joint.enableCollision=true;
				joint.useLimits=true;
				joint.limits=jointlimit;
				joint.connectedBody=coll.rigidbody;
				joint.anchor=gameObject.transform.InverseTransformPoint(contacting[0].point);
				joint.connectedAnchor=playerscript.impact;
				control.hookedalpha=true;
			} else if ((control.hookedalpha==true)&&(Mathf.Abs(Vector2.Distance(gameObject.transform.InverseTransformPoint(contacting[0].point),gameObject.GetComponent<HingeJoint2D>().anchor)))>=0.01f&&(control.hookedbeta==false)){
				//if previously hooked and the distance between the previous hook and the new hook is greater then one absolute then
				//create second normal joint
				//purpose is to force joint onto other side of rocket
				Debug.Log("in");
				joint2=gameObject.AddComponent<DistanceJoint2D>();
				joint2.enableCollision=true;
				joint2.connectedBody=coll.rigidbody;
				joint2.anchor=gameObject.transform.InverseTransformPoint(contacting[0].point);
				joint2.connectedAnchor=playerscript.impact;
				joint2.distance=0;
				control.hookedbeta=true;
			}
		} else {
			Reset();
		}
	}
}
