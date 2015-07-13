using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public static GameController control;
	public float fuel=5f; //fuel now in seconds of power
	public float time=0f;
	public bool paused=false;
	public bool hookedalpha=false;
	public bool hookedbeta=false;
	public bool powered=false;
	public bool launched=false;
	public bool button=false;
	public bool inLevel=false;

	public Vector2 power = new Vector2 (0,0.1f);
	//Gameobjects
	public GameObject player;
	public GameObject launcher;
	Animator An;
	//event system variables

	private Touch touch;

	void Awake () {
		if (control==null){ //this script ensures persistance, if one does not exist one is created
			DontDestroyOnLoad(gameObject);
			control=this;
		} else if (control!=this){
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Event system
		if (inLevel){
			//player/component declarations
			player=GameObject.Find("Rocket");
			launcher=GameObject.Find("Launcher");
			An=player.GetComponent<Animator>();
			Debug.Log ("something is working");

			if (Input.touchCount==1){
				Debug.Log ("touched");
				touch=Input.GetTouch(0);//gets the touch and assigns it to touch variable
				if (UiDetect(touch)==false){ //if the touch is not coincident with a UI element
					if (launched){ //if the rocket has been launched
						if (fuel>0){ //if the rocket has fuel
							An.SetBool("Active",true);
							powered=true;
							fuel=fuel-1*Time.deltaTime;
							Rigidbody2D x = player.gameObject.GetComponent<Rigidbody2D>();
							Vector3 diff = Camera.main.ScreenToWorldPoint (touch.position) - player.transform.position;
							diff.Normalize ();
							float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
							player.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
							x.AddRelativeForce (power, ForceMode2D.Impulse);
						} else {
							An.SetBool("Active",false);
							powered=false;
						}
					} else { //if the rocket is yet to launch
						Vector3 diff = Camera.main.ScreenToWorldPoint (touch.position) - launcher.transform.position;
						diff.Normalize ();
						float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
						launcher.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
					}
				}
			} else { //if nothing else is touched make sure to reset rocket power
				powered=false;
				An.SetBool("Active",false);
			}
		}
	}
	public void SceneSwitchers (int target) {
		if (target>=1){
			inLevel=true;
		} else {
			inLevel=false;
		}
		Application.LoadLevel(target);
	}
	public bool UiDetect(Touch touchdetect){
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.position), Vector2.zero);
		//Debug.Log ("Target Position: " + hit.rigidbody);
		if (hit.rigidbody!=null&&hit.transform.parent!=null){
			if (hit.transform.parent.name=="Canvas"){
				//Debug.Log("canvaspansas");
				return true;
			} else {
				//Debug.Log ("nothing");
				return false;
			}
		}
		return false;
	}
}
