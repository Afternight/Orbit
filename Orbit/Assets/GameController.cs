using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public static GameController control;
	public float fuel=5f; //fuel now in seconds of power
	public float time=0f;
	public bool paused=false;
	public bool hookedalpha=false;
	public bool powered=false;
	public bool launched=false;
	public bool button=false;
	public bool inLevel=false;
	public float orthoZoomSpeed = 0.03f;
	public bool camhook=false;
	public Vector2 power = new Vector2 (0,0.1f);
	private bool draginprogress = false;
	public bool resetcam=false;
	public bool interpolate=false;

	//Gameobjects
	public GameObject player;
	public GameObject launcher;
	public GameObject MainCam;
	public GameObject trajectory;
	public GameObject planet;
	private Rotator planetscript;
	private launcher launchscript;
	public Camera Cam;
	Animator An;

	//Reset strikes
	public bool zoomready=false;
	public bool camposready=false;
	public bool rocketdestroyed=false;
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

	void Start () {
		//set player preferences if they are not already set
		if (PlayerPrefs.GetFloat("PlayZoom")==0f){
			PlayerPrefs.SetFloat("PlayZoom",4f);
		}
	}

	// Update is called once per frame
	void Update () {
		//Event system
		if (inLevel){ //if in a playable level
			//player/component declarations
			player=GameObject.Find("Rocket");
			planet=GameObject.Find("Mars");
			planetscript=planet.GetComponent<Rotator>();
			launcher=GameObject.Find("Launcher");
			launchscript=launcher.GetComponent<launcher>();
			trajectory=GameObject.Find("Trajectory");
			An=player.GetComponent<Animator>();
		}
		//always present declarations
		MainCam=GameObject.Find("Main Camera");
		Cam=MainCam.GetComponent<Camera>();
		if (launched){ //if the rocket is grounded or not, also set to false for non-playing levels
			if (Input.touchCount>=1){
				touch=Input.GetTouch(0);//gets the touch and assigns it to touch variable
				if (UiDetect(touch)==false){ //if the touch is not coincident with a UI element
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
				}
			} else { //if nothing else is touched make sure to reset rocket power
				powered=false;
				An.SetBool("Active",false);
			}
		} else {
			if (Input.touchCount==1){
				touch=Input.GetTouch(0);
				if (touch.phase==TouchPhase.Began){
					if (((TargetDetect(touch)==true)||(previousTargetDetect(touch)==true))&&(UiDetect(touch)==false)){ //check if touch is incident with target object
						//Launcher orientation manipulation
						if (touch.phase==TouchPhase.Began){
							draginprogress=true;
						}
						Vector3 diff = Camera.main.ScreenToWorldPoint (touch.position) - launcher.transform.position;
						diff.Normalize ();
						float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
						launcher.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
						trajectory.GetComponent<SpriteRenderer>().sprite=launchscript.trajectorysolid;
					} else { // if its not incident then we want to have the camera drag capability possibly functionalise this code
						Vector2 touchprev=touch.position-touch.deltaPosition;
						Vector2 touchmagnitude=touch.position-touchprev;
						Vector3 newpos= new Vector3(-touchmagnitude.x*0.03f,-touchmagnitude.y*0.03f,0);
						Cam.transform.position+=newpos;
					}
				} else if ((touch.phase==TouchPhase.Moved)||(touch.phase==TouchPhase.Stationary)){
					if (draginprogress){
						Vector3 diff = Camera.main.ScreenToWorldPoint (touch.position) - launcher.transform.position;
						diff.Normalize ();
						float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
						launcher.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
						trajectory.GetComponent<SpriteRenderer>().sprite=launchscript.trajectorysolid;
					} else {
						Vector2 touchprev=touch.position-touch.deltaPosition;
						Vector2 touchmagnitude=touch.position-touchprev;
						Vector3 newpos= new Vector3(-touchmagnitude.x*0.03f,-touchmagnitude.y*0.03f,0);
						Cam.transform.position+=newpos;
					}
				} else if ((touch.phase==TouchPhase.Ended)||(touch.phase==TouchPhase.Canceled)){
					draginprogress=false;
					if (inLevel){
						trajectory.GetComponent<SpriteRenderer>().sprite=launchscript.trajectorytrans;
					}
					//also put in momentum code here?
				}
			} else if (Input.touchCount==2){ //if two fingers are touching, meaning we want to pinch zoom
				if (inLevel){
					trajectory.GetComponent<SpriteRenderer>().sprite=launchscript.trajectorytrans;
				}
				Touch touchZero = touch;
				Touch touchOne = Input.GetTouch(1);
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; // Find the position in the previous frame of each touch.
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; // Find the magnitude of the vector (the distance) between the touches in each frame.
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;// Find the difference in the distances between each frame.
				if (Mathf.Abs(deltaMagnitudeDiff*orthoZoomSpeed)>=0.1f){ //ensure change is substantial to prevent flickering
					Cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;// change the orthographic size based on the change in distance between the touches.
				}
				Cam.orthographicSize = Mathf.Max(Cam.orthographicSize, 0.1f);// Make sure the orthographic size never drops below zero.
			}
		}

		//reset strike system
		if ((zoomready==true)&&(camposready==true)&&(rocketdestroyed==true)){
			planetscript.Reset();
		}
		time=time+1*Time.deltaTime;
	}
	public void SceneSwitchers (int target) {
		if (target>=1){
			inLevel=true;
		} else {
			inLevel=false;
		}
		launched=false;
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
	
	public bool TargetDetect(Touch touchdetect){ //currently stubbed till target implemented
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.position), Vector2.zero);
		//Debug.Log ("Target Position: " + hit.rigidbody);
		if (hit.rigidbody!=null&&hit.transform.parent!=null){
			if (hit.transform.name=="Trajectory"){
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	public bool previousTargetDetect(Touch touchdetect){ //currently stubbed till target implemented
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.deltaPosition), Vector2.zero);
		//Debug.Log ("Target Position: " + hit.rigidbody);
		if (hit.rigidbody!=null&&hit.transform.parent!=null){
			if (hit.transform.name=="Trajectory"){
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
}
