using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {
	private float y=0f;
	/*private Vector2 velocity=new Vector2(0,0);
	public float speed;
	private Collider2D Base;*/
	private GameObject controller;
    private GameController control;
    private GameObject Menu;
    private RectTransform MenuTransform;
    private GameObject WinAnimation;
    private RectTransform WinAnimationTransform;
    /*private GameObject player;
	private Player playerscript;
	private Rigidbody2D body;
	private HingeJoint2D joint;
	private ContactPoint2D[] contacting;
	private JointAngleLimits2D jointlimit;
	private GameObject launch;
	private launcher launchscript;
	Animator An;*/
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
        controller = GameObject.Find("GameController");
        control = controller.GetComponent<GameController>();
        /*controller=GameObject.Find ("GameController");
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
		}*/

        //old code for landing above, deemed a bit to hard for most players, perhaps offer only in certain levels
        if (control.initialsuccessinvoke) {
            WinAnimation = GameObject.Find("WinAnimation");
            WinAnimationTransform = WinAnimation.GetComponent<RectTransform>();
            WinAnimationTransform.localPosition = control.player.transform.position;
            Invoke("Success", 2);
            //calculate fuel here to prevent accidents?
            control.GameStatus = 5; //lock out further input
            control.initialsuccessinvoke = false;
        }
    }

	void Success(){
		controller=GameObject.Find ("GameController");
		control=controller.GetComponent<GameController>();
		control.GameStatus = 5; //assign success status

        Menu = GameObject.Find("Menu");
        MenuTransform = Menu.GetComponent<RectTransform>();
        control.DynaMoveTarget = Vector3.zero;
        control.dynaMoveBound = 1f;
        control.inputTransform = MenuTransform;
        control.moveInaction = true;

        //DataPlay nessasaries
        control.DataPlay.completed[Application.loadedLevel] = 1;
        if (control.fuel > control.DataPlay.HighestFuel[Application.loadedLevel]) {
            Debug.LogWarning("NEW HIGH SCORE");
            control.DataPlay.HighestFuel[Application.loadedLevel] = control.fuel;
        }
        control.DataPlay.TrophyLevel[Application.loadedLevel] = 3;//temp set at gold forever
        control.Save();
	}
}