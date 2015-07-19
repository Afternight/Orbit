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
	private float distance=0f;
	private float scaleddistance=0f;
	private Vector3 launcherpos3=new Vector3(0,0,0);
	private Vector3 maxscale =new Vector3(10,15,10);
	private Vector3 minscale =new Vector3(10,3,10);
	private Vector3 trajscale= new Vector3 (10,0,10);

	//Cam controls
	private bool CamMode=false; //where false==dynacam,true=freecam.

	//DynaCam
	public GameObject MainCam;
	public Camera Cam;
	public float differencex=0f;
	public float differencey=0f;
	public Vector3 pinpoint= new Vector3 (0,0,-10f); //where -10f is cam zoom pos
	public Vector3 inter3 = new Vector3(0,0,-10f);
	public Vector2 inter = new Vector2 (0,0);
	public Vector2 maincam2 = new Vector2 (0,0);
	public Vector2 player2= new Vector2(0,0);
	public Vector3 CamOrig= new Vector3 (0,0,-10f);
	public float CamOrigZoom=10f;

	//DynaCam inputs
	public Vector3 CamTarget = new Vector3 (0,0,0);
	public float CamZoom=0f;
	public float CamScale=0.1f;
	public float CamBound=0.2f;
	
	//Arrays
	private float[] fuelinitials;

	//Gameobjects
	public GameObject PlayerCam;
	public GameObject player;
	public GameObject launcher;
	public GameObject trajectory;
	public GameObject planet;
	private Rotator planetscript;
	private launcher launchscript;
	public GameObject MainCamupdate;
	public Camera Camupdate;
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
		//set fuel array
		//level indexs of 0-4 reserved for nonplayables
		//0 is main menu
		//1 is level select
		//2 is options

		//fuelinitials[5]=200f; set values for levels here
		//also store initial camera locations
	}

	void Start () {
		//set player preferences if they are not already set
		if (PlayerPrefs.GetFloat("PlayZoom")==0f){
			PlayerPrefs.SetFloat("PlayZoom",4f);
		}
		//set menu origs
		MainCamupdate=GameObject.Find("Main Camera");
		Camupdate=MainCamupdate.GetComponent<Camera>();
		CamOrig=MainCamupdate.transform.position;
		CamOrigZoom=Camupdate.orthographicSize;
	}

	void OnLevelWasLoaded(int level) {
		//store original camera size
		Debug.Log(level);
		MainCamupdate=GameObject.Find("Main Camera");
		Camupdate=MainCamupdate.GetComponent<Camera>();
		CamOrig=MainCamupdate.transform.position;
		CamOrigZoom=Camupdate.orthographicSize;
		CamTarget=CamOrig;
		CamZoom=CamOrigZoom;
		CamBound=0.01f;
	}

	// Update is called once per frame
	void Update () {
		//if needing panning opening
		//set cam at earth
		//do we just want to lerp?
		//Event system
		if (inLevel){ //if in a playable level
			//player/component declarations
			player=GameObject.Find("Rocket");
			PlayerCam=GameObject.Find ("CamTarget");
			planet=GameObject.Find("Mars");
			planetscript=planet.GetComponent<Rotator>();
			launcher=GameObject.Find("Launcher");
			launchscript=launcher.GetComponent<launcher>();
			trajectory=GameObject.Find("Trajectory");
			An=player.GetComponent<Animator>();
		}
		//always present items
		MainCamupdate=GameObject.Find("Main Camera");
		Camupdate=MainCamupdate.GetComponent<Camera>();
		if ((launched==true)&&(paused==false)&&(rocketdestroyed==false)){ //if the rocket is grounded or not, also set to false for non-playing levels
			CamTarget=PlayerCam.transform.position; //set dynacam values for launched
			CamZoom=PlayerPrefs.GetFloat("PlayZoom");
			CamBound=0.4f;
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
		} else { //initiate free looks, launchercontrols
			if (Input.touchCount==1){
				touch=Input.GetTouch(0);
				if (touch.phase==TouchPhase.Began){
					if (((TargetDetect(touch)==true)||(previousTargetDetect(touch)==true))&&(UiDetect(touch)==false)){ //check if touch is incident with target object
						//Launcher orientation manipulation
						if (touch.phase==TouchPhase.Began){
							draginprogress=true;
						}
						Launcherrotate(launcher,touch);
					} else { // if its not incident then we want to have the camera drag capability possibly functionalise this code
						Vector2 touchprev=touch.position-touch.deltaPosition;
						Vector2 touchmagnitude=touch.position-touchprev;
						Vector3 newpos= new Vector3(-touchmagnitude.x*0.03f,-touchmagnitude.y*0.03f,0);
						Camupdate.transform.position+=newpos;
					}
				} else if ((touch.phase==TouchPhase.Moved)||(touch.phase==TouchPhase.Stationary)){
					if (draginprogress){
						Launcherrotate(launcher,touch);
					} else {
						Vector2 touchprev=touch.position-touch.deltaPosition;
						Vector2 touchmagnitude=touch.position-touchprev;
						Vector3 newpos= new Vector3(-touchmagnitude.x*0.03f,-touchmagnitude.y*0.03f,0);
						Camupdate.transform.position+=newpos;
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
					Camupdate.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;// change the orthographic size based on the change in distance between the touches.
				}
				Camupdate.orthographicSize = Mathf.Max(Camupdate.orthographicSize, 0.1f);// Make sure the orthographic size never drops below zero.
			}
		}
		//Debug.Log(CamMode);
		if (CamMode==false){
		//DynaCamprotections
			if (CamTarget==Vector3.zero){
				CamTarget=CamOrig;
			}
			if (CamZoom==0f){
				CamZoom=CamOrigZoom;
			}
			//call DynaCam
			DynaCam(CamTarget,CamZoom,CamScale,CamBound);
		}
		//reset strike system
		if ((zoomready==true)&&(camposready==true)&&(rocketdestroyed==true)){
			planetscript.Reset();
		}
		if ((launched==true)&&(paused==false)){
			time=time+1*Time.deltaTime;
		}
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
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	
	public bool TargetDetect(Touch touchdetect){
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
	public bool previousTargetDetect(Touch touchdetect){ //difference to target detect is raycasts from delta position, consider condensing
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

	private void Launcherrotate(GameObject launcher,Touch touch){
		Vector3 diff = Camera.main.ScreenToWorldPoint (touch.position) - launcher.transform.position;
		diff.Normalize ();
		float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
		launcher.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
		trajectory.GetComponent<SpriteRenderer>().sprite=launchscript.trajectorysolid;
		//calculate distance from touch to center of planet
		launcherpos3=launcher.transform.position;
		distance=Vector2.Distance(launcherpos3,Camera.main.ScreenToWorldPoint(touch.position));
		Debug.Log ("distance"+distance);
		//set a max
		if (Mathf.Abs(distance)>=15f){ //need to find the right value for this
			trajectory.transform.localScale=maxscale;
		} else if (Mathf.Abs(distance)<=1f){ //set a min
			trajectory.transform.localScale=minscale;
		} else {// if inbetween calculate scale factor by percentage of max reached
			scaleddistance=Mathf.Abs(distance);
			scaleddistance=scaleddistance/15f; // calculate scale
			trajscale.y=scaleddistance*15f+1f; //add minimum of 1 onto existing
			trajectory.transform.localScale=trajscale;
		}
	}

	private void DynaCam(Vector3 Target, float Zoom, float scale, float bound){
		MainCam=GameObject.Find("Main Camera");
		Cam=MainCam.GetComponent<Camera>();
		if ((MainCam.transform.position.x!=Target.x)||(MainCam.transform.position.y!=Target.y)){ //ensure transform
			maincam2.x=MainCam.transform.position.x;
			maincam2.y=MainCam.transform.position.y;
			player2.x=Target.x;
			player2.y=Target.y;
			inter=Vector2.Lerp(maincam2,player2,scale);
			inter3.x=inter.x;
			inter3.y=inter.y;
			camposready=false;
			MainCam.transform.position=inter3;
		} else if ((MainCam.transform.position.x==Target.x)&&(MainCam.transform.position.y==Target.y)){
			camposready=true;
			camhook=true;
		}
		if (Cam.orthographicSize!=Zoom){ //ensure zoom
			Cam.orthographicSize=Mathf.Lerp (Cam.orthographicSize,Zoom,0.1f);
			zoomready=false;
		} else if (Cam.orthographicSize==Zoom){
			zoomready=true;
		} 
		if (Mathf.Abs(Zoom-Cam.orthographicSize)<bound){
			Cam.orthographicSize=Zoom;
			zoomready=true;
		}
		differencex=MainCam.transform.position.x - Target.x;
		differencey=MainCam.transform.position.y - Target.y;
		if (((Mathf.Abs(differencex)<bound)&&(Mathf.Abs(differencey)<bound))||camhook==true){
			pinpoint.x=Target.x;
			pinpoint.y=Target.y;
			MainCam.transform.position=pinpoint;
			camposready=true;
			camhook=true;
		} else if ((Mathf.Abs(differencex)>bound)||(Mathf.Abs(differencey)>bound)){
			camhook=false;
		}
	}
}
