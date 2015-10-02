using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {
	public static GameController control;
	public float fuel=5f; //fuel now in seconds of power
	public float time=0f;
	public bool hookedalpha=false;
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

    //Constants
    public static int totallevels = 4;
    public static int totalmenus = 3;

	//Cam controls
	public bool CamMode=false; //where false==dynacam,true=freecam.

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
	public bool Revoke=false; //if true dynacam will switch modes to freecam after it hooks to the latest target
	
	/*GAMEOBJECTS*/

    //Player
	public GameObject PlayerCam;
	public GameObject player;

    //Launcher
	public GameObject launcher;
	public GameObject trajectory;
    private launcher launchscript;

    //Misc
    public GameObject Earth;
	public GameObject MainCamupdate;
	public Camera Camupdate;
	Animator An;
	public BoxCollider2D boxcoll;
	public int animID;
	public int animIDreset;

	//Reset strikes
	public bool zoomready=false;
	public bool camposready=false;

	//Event system variables
	private Touch touch;
	public int GameStatus=0;
							//0 = Starting
							//1 = Explore
							//2 = Launching
							//3 = Launched/inplay
							//4 = Landed (on non-ending planet)
							//5 = Finish/Success
							//6 = Paused
							//7 = Destroyed
                            //8 = InMenu
	public bool indicatorneeded=true;
	//public bool initial=true;
	public bool initial0=true;
	public bool initial1=true;
    public bool initiallaunched = true;
    public bool initialfuelset = true;
    public bool initialsuccessinvoke = true;

    //fuel bar
    public RectTransform fuelbartransform;
    //public Vector2 cache=new Vector2(0f,0f);

    //Gravitational stuff
    GameObject[] planets;
    public float maxGravDist = 100f;
    public float maxGravity = 0.5f;
    public float dist = 0f;

    //Mid point determinants
    public float StrongestGravit = 0f;
    public GameObject StrongestPlanet;

    //UI elements
    public GameObject FuelUiObject;
    public Animator FuelUi;
    public GameObject Indicator;
    public GameObject FuelBar;

    //DynaMove
    public bool hookedMove=false;
    public float dy = 0f;
    public float dx = 0f;
    public bool moveInaction = false;
    public RectTransform inputTransform;
    public float dynaMoveBound;
    public Vector3 DynaMoveTarget;

    //GameData
    public GameData DataPlay=new GameData();
    //Data is to be loaded on GameControllers creation
    //Saved whenever a rocket lands successfully

    //other
    public bool resetalready = false;
    public bool stratcam = false;

    //testing
    private int GamestatusStore = 999;

    void Awake () {
		if (control==null){ //this script ensures persistance, if one does not exist one is created
			DontDestroyOnLoad(gameObject);
			control=this;
            //make sure to only load or create once
            if (File.Exists(Application.persistentDataPath + "/GameData.dat")) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);
                DataPlay = (GameData)bf.Deserialize(file);
                file.Close();
                Debug.LogWarning("loaded level from previous");
            } else {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/GameData.dat");

                setinitials(); //initialise values

                bf.Serialize(file, DataPlay);
                file.Close();
                Debug.LogWarning("Created new and saved");
            }
        } else if (control!=this){
			Destroy(gameObject);
		}
    }

	void Start () {
		//set player preferences if they are not already set
		if (PlayerPrefs.GetFloat("PlayZoom")==0f){
			PlayerPrefs.SetFloat("PlayZoom",4f); //for now constant at 4 for testing
		}
		//set menu origs
		MainCamupdate=GameObject.Find("Main Camera");
		Camupdate=MainCamupdate.GetComponent<Camera>();
		//CamOrig=MainCamupdate.transform.position;
		//CamOrigZoom=Camupdate.orthographicSize;
	}

	void OnLevelWasLoaded(int level) {
		//store original camera size
		Debug.Log("loaded "+level);
        if (inLevel) {
            MainCamupdate = GameObject.Find("Main Camera");
            Camupdate = MainCamupdate.GetComponent<Camera>();
            CamOrig = MainCamupdate.transform.position;
            CamOrigZoom = Camupdate.orthographicSize;
            FuelUiObject = GameObject.Find("FuelUI");
            FuelUi = FuelUiObject.GetComponent<Animator>();
            CamTarget = CamOrig;
            CamZoom = CamOrigZoom;
            CamBound = 0.01f;
        }
		//set gamestate to starting
	}

	// Update is called once per frame
	void Update () {
        //Event system
        //always present items
		MainCamupdate=GameObject.Find("Main Camera");
		Camupdate=MainCamupdate.GetComponent<Camera>();
        if (inLevel){ //if in a playable level
			//player/component declarations
			player=GameObject.Find("Rocket");
			PlayerCam=GameObject.Find ("CamTarget");
			Earth=GameObject.Find("Earth");
			Indicator=GameObject.Find ("Indicator");
			launcher=GameObject.Find("Launcher");
			launchscript=launcher.GetComponent<launcher>(); //WHY IS THIS ONE TAKING SO LONG TO LOAD OR SOMETHING
			trajectory=GameObject.Find("Trajectory");
			An=player.GetComponent<Animator>();
			boxcoll=MainCamupdate.GetComponent<BoxCollider2D>();
        } else { //menus exception
			GameStatus=1;
		}
        if (GameStatus == 0) { //Starting
            CamMode = false;
            if (initial0) { //to prevent multiple invokes
                CamOrig= new Vector3((StrongestPlanet.transform.position.x + player.transform.position.x) / 2, (StrongestPlanet.transform.position.y + player.transform.position.y) / 2, -10f);
                CamOrigZoom=0.5f * dist;
                Invoke("CamStart", 2);//Invoke a function to target onto rocket launch pad after 2 seconds
                initial0 = false;
            }          

        } else if (GameStatus == 1) { //Explore
            if (resetalready) {
                //for some reason slips through when reset hit really early sometimes so reasigned
                CamOrig = new Vector3((StrongestPlanet.transform.position.x + player.transform.position.x) / 2, (StrongestPlanet.transform.position.y + player.transform.position.y) / 2, -10f);
                CamOrigZoom = 0.5f * dist;

                Camupdate.transform.position = new Vector3((StrongestPlanet.transform.position.x + player.transform.position.x) / 2, (StrongestPlanet.transform.position.y + player.transform.position.y) / 2, -10f);
                Camupdate.orthographicSize = 0.5f * dist;
                resetalready = false;
            }
            //Allow freecam control, plus trajectory modification
            if (stratcam == false) {
                CamMode = true; //set cam mode
                if (Input.touchCount == 1) {
                    touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began) {
                        if (((TargetDetect(touch) == true) || (previousTargetDetect(touch) == true)) && (UiDetect(touch) == false)) { //check if touch is incident with target object
                                                                                                                                      //Launcher orientation manipulation
                            if (touch.phase == TouchPhase.Began) {
                                draginprogress = true;
                            }
                            Launcherrotate(launcher, touch);
                        } else { // if its not incident then we want to have the camera drag capability possibly functionalise this code
                            Vector2 touchprev = touch.position - touch.deltaPosition;
                            Vector2 touchmagnitude = touch.position - touchprev;
                            Vector3 newpos = new Vector3(-touchmagnitude.x * 0.03f, -touchmagnitude.y * 0.03f, 0);
                            Camupdate.transform.position += newpos;
                        }
                    } else if ((touch.phase == TouchPhase.Moved) || (touch.phase == TouchPhase.Stationary)) {
                        if (draginprogress) {
                            Launcherrotate(launcher, touch);
                        } else {
                            Vector2 touchprev = touch.position - touch.deltaPosition;
                            Vector2 touchmagnitude = touch.position - touchprev;
                            Vector3 newpos = new Vector3(-touchmagnitude.x * 0.03f, -touchmagnitude.y * 0.03f, 0);
                            Camupdate.transform.position += newpos;
                        }
                    } else if ((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled)) {
                        draginprogress = false;
                        if (inLevel) {
                            trajectory.GetComponent<SpriteRenderer>().sprite = launchscript.trajectorytrans;
                        }
                        //also put in momentum code here?
                    }
                } else if (Input.touchCount == 2) { //if two fingers are touching, meaning we want to pinch zoom
                    if (inLevel) {
                        trajectory.GetComponent<SpriteRenderer>().sprite = launchscript.trajectorytrans;
                    }
                    Touch touchZero = touch;
                    Touch touchOne = Input.GetTouch(1);
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; // Find the position in the previous frame of each touch.
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;// Find the difference in the distances between each frame.
                    if (Mathf.Abs(deltaMagnitudeDiff * orthoZoomSpeed) >= 0.1f) { //ensure change is substantial to prevent flickering
                        Camupdate.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;// change the orthographic size based on the change in distance between the touches.
                    }
                    Camupdate.orthographicSize = Mathf.Max(Camupdate.orthographicSize, 0.1f);// Make sure the orthographic size never drops below zero.
                }
            } else {
                setStratCam(); //need to consider reset ability
            }
            
        } else if (GameStatus == 2) { // Launching
            CancelInvoke("CamStart"); // incase time has been skipped
            CamMode = false; //back to the dynacam
                             //CamTarget=PlayerCam.transform.position;
            if (stratcam == false) {
                //old suspense zoom in code
                /*if (initial) {
                    CamZoom = 0.5f * dist;
                    initial = false;
                } else {
                    CamZoom -= 0.1f * Time.deltaTime;
                }*/
                setActionCam();
            } else {
                setStratCam();
            }
        } else if (GameStatus == 3) { //Launched
            CamMode = false;
            if (stratcam == false) {
                if (initiallaunched) {
                    camhook = false;
                    initiallaunched = false;
                }
                setActionCam();
            } else {
                setStratCam();
            }
            if (Input.touchCount >= 1) {
                touch = Input.GetTouch(0);//gets the touch and assigns it to touch variable
                if (UiDetect(touch) == false) { //if the touch is not coincident with a UI element
                    if (fuel > 0) { //if the rocket has fuel
                        An.SetBool("Active", true);
                        fuel = fuel - 1 * Time.deltaTime;
                        Rigidbody2D x = player.gameObject.GetComponent<Rigidbody2D>();
                        Vector3 diff = Camera.main.ScreenToWorldPoint(touch.position) - player.transform.position;
                        diff.Normalize();
                        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                        player.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                        x.AddRelativeForce(power, ForceMode2D.Impulse);
                    } else {
                        An.SetBool("Active", false);
                    }
                }
            } else { //if nothing else is touched make sure to reset rocket power
                An.SetBool("Active", false);
            }
        } else if (GameStatus == 4) { //landed
                                      //not much to go in here at the moment as landing is buggy
        } else if (GameStatus == 5) { //Success
                                      //Application.LoadLevel(0); //placeholder for testing
        } else if (GameStatus == 6) { //Paused
                                      //eventually when freecam functionalised add here
                                      //make pause menue pop up
                                      //Consider a wipe function implementable that runs at initial of each gamestatus to ensure correct ui elements shown
        } else if (GameStatus == 7) {//destroyed/failed
                                     //reset cam back to initials
                                     //when cam is ready call reset
            if (initial1) {
                camposready = false;
                zoomready = false;
                initial1 = false;
            }
            CamMode = false;
            CamTarget = CamOrig;
            CamZoom = CamOrigZoom;
            CamScale = 0.1f;
            CamBound = 0.02f;
            FuelUi.SetTrigger(animIDreset); //trigger reset animation
            FuelUi.ResetTrigger(animID);
            if (camposready && zoomready) {
                ResetControl();
            }
        } else if (GameStatus == 8) {

		} else { //out of bounds
			Debug.LogError("Invalid GameStatus");
		}

		if (CamMode==false){
			//DynaCamprotections
			if (CamTarget==Vector3.zero){
				CamTarget=CamOrig;
			}
			if (CamZoom==0f){
				CamZoom=CamOrigZoom;
			}
			//call the glorious DynaCam
			DynaCam(CamTarget,CamZoom,CamScale,CamBound,Revoke);
		}

		if (GameStatus!=6f){
			time=time+1*Time.deltaTime;
		}
        if (GameStatus != GamestatusStore) {
            Debug.LogWarning("GameStatus now " + GameStatus);
            GamestatusStore = GameStatus;
        }

		//Cam collider pointer
		//can functionalise and increase for multiple indicators
		if (inLevel&&indicatorneeded){
			boxcoll.size=new Vector2((4f*Camupdate.orthographicSize)-3f,(2f*Camupdate.orthographicSize)-2f); //need to possibly change values here
			RaycastHit2D hit=Physics2D.Linecast(Earth.transform.position,MainCamupdate.transform.position,Physics2D.DefaultRaycastLayers,-Mathf.Infinity,-9);
			if (hit.rigidbody!=null){
				Vector3 indicatorv3=new Vector3 (hit.point.x,hit.point.y,-2);
				Indicator.transform.localScale=new Vector3(Camupdate.orthographicSize*0.02f,Camupdate.orthographicSize*0.02f,1f);//unsure of the value best suited here, but this code is ready for graphics pass
				Indicator.transform.position=indicatorv3;
			}
		}

        //Fuel bar percentage code
        //fuel = fuel - 1 * Time.deltaTime; //test line 
        if (inLevel){
            FuelBar = GameObject.Find("fuel");
            fuelbartransform = FuelBar.GetComponent<RectTransform>();
            Debug.Log("FUEL " + fuel);
            if (fuel > 0)
                fuelbartransform.localScale = new Vector3(fuel / 5f, 1f, 1f);//eventually change 5f to fuelinitial TODO
            else
                fuelbartransform.localScale = Vector3.zero;
        }

        //DynaMove activation
        if (moveInaction) {
            dynaMove(inputTransform, DynaMoveTarget, dynaMoveBound);
        }

        //Camera clamping for boundaries
        Camupdate.transform.position = new Vector3(Mathf.Clamp(Camupdate.transform.position.x, DataPlay.xboundsx[Application.loadedLevel], DataPlay.xboundsy[Application.loadedLevel]), Mathf.Clamp(Camupdate.transform.position.y, DataPlay.yboundsx[Application.loadedLevel], DataPlay.yboundsy[Application.loadedLevel]), -10f);
    }

    void FixedUpdate() {
        if (inLevel){
            player = GameObject.Find("Rocket");
            planets = GameObject.FindGameObjectsWithTag("Planet"); //think 5.2 update broke tags or something, had to code define
            foreach (GameObject planet in planets){ //iterates through planets
                dist = Vector3.Distance(planet.transform.position, player.transform.position);
                if (dist <= maxGravDist){
                    maxGravity = 0.3f;//need to modify
                    maxGravity = maxGravity / dist * 2;// distance gets smaller!!!!
                    Vector3 v = planet.transform.position - player.transform.position;
                    Vector2 force = v.normalized * (1.0f - dist / maxGravDist) * maxGravity;
                    if (force.magnitude >= StrongestGravit){ //add overlap section eventually to prevent binary setups causing a major issue
                        StrongestGravit = force.magnitude; //need to cause entering of this loop if level was reset
                        StrongestPlanet = planet;
                        camhook = false; //allow for dynamic change
                        Debug.Log("New strongest planet, name " + StrongestPlanet.name);
                    }
                    player.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse); //change max gravity based on distance from object
                }
            }
        }
    }

    public void SceneSwitchers (int target) { //consider using this for level load stat
        if (target>=3){ //modify this value to first level when pre levels finished
			inLevel=true;
            enteringResetControl();
            GameStatus = 0;
        } else {
			inLevel=false;
            GameStatus = 8; //set gamestatus to menu
		}
		Application.LoadLevel(target);
        fuel = DataPlay.InitialFuel[target];
    }

	public bool UiDetect(Touch touchdetect){
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.position), Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers,-8f);
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
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.position), Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers,-8f);
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
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchdetect.deltaPosition), Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers,-8f);
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
		//divide distance by current scale, so that
		//set a max
		if (Mathf.Abs(distance)>=15f){ //need to find the right value for this
			trajectory.transform.localScale=maxscale;
		} else if (Mathf.Abs(distance)<=1f){ //set a min
			trajectory.transform.localScale=minscale;
		} else {// if inbetween calculate scale factor by percentage of max reached
			scaleddistance=Mathf.Abs(distance);
			scaleddistance=scaleddistance/trajectory.transform.localScale.y; // calculate scale
			trajscale.y=scaleddistance*Mathf.Abs(distance)+3f; //add minimum of 1 onto existing
			trajectory.transform.localScale=trajscale;
		}
	}

	private void DynaCam(Vector3 Target, float Zoom, float scale, float bound,bool revoke){
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
		if (revoke&&camposready&&zoomready){
			CamMode=true;//relinquish
			GameStatus=1;
			Revoke=false; //remove the future relinquish
		}
	}

    public void dynaMove (RectTransform Transforming, Vector3 target, float bound) {
        if (Transforming.localPosition != target) {
            Transforming.localPosition = Vector3.Lerp(Transforming.localPosition, target, 0.1f);
        } 
        dx = Transforming.localPosition.x - target.x;
        dy = Transforming.localPosition.y - target.y;
        if (((Mathf.Abs(dx) < bound) && (Mathf.Abs(dy) < bound)) || hookedMove == true) {
            Transforming.localPosition = target;
            moveInaction = false;
        }
    }

	private void CamStart(){ //uses dynacam to target onto rocket but zoomed out a bit
		Debug.LogWarning("CamStart has beencalled"); //magical line that removes bugs when put in the general vicinity for some reason
		PlayerCam=GameObject.Find ("CamTarget");
		camhook=false;
        CamTarget = new Vector3((StrongestPlanet.transform.position.x + player.transform.position.x) / 2, (StrongestPlanet.transform.position.y + player.transform.position.y) / 2, player.transform.position.z);
        //CamTarget = PlayerCam.transform.position;
		CamBound=0.1f;
		CamScale=DataPlay.PanSpeed[Application.loadedLevel];
		CamZoom=0.5f*dist;
		Revoke=true;
        CamOrig = CamTarget;
        CamOrigZoom = CamZoom;
    }

	public void ResetControl(){
        fuel =DataPlay.InitialFuel[Application.loadedLevel];
		hookedalpha=false;
		camhook=false;
		time=0f; //why do I have this? Consider adding fastest mode later

		//initial=true;
		initial0=true;
		initial1=true;
        initiallaunched = true;
        initialfuelset = true;
        initialsuccessinvoke = true;

        //Strongest reset
        StrongestPlanet = null;
        StrongestGravit = 0f;

        //Reset strike system resets
        camposready =false;
		zoomready=false;

        resetalready = true;
        
        Application.LoadLevel (Application.loadedLevel); //resets level
        Debug.LogWarning("reset already is " + resetalready);
        //set cam values here
        if (GameStatus==6){
			Time.timeScale=1;
		}
		GameStatus=1; //set to explore
	}

    public void enteringResetControl() {
        //used when entering a playable level
        //could probably be cleaned up by calling this everytime success
        hookedalpha = false;
        camhook = false;
        time = 0f; //why do I have this? Consider adding fastest mode later

        //initial = true;
        initial0 = true;
        initial1 = true;
        initiallaunched = true;
        initialfuelset = true;
        initialsuccessinvoke = true;
        resetalready = false;

        //Strongest reset
        StrongestPlanet = null;
        StrongestGravit = 0f;

        //Reset strike system resets
        camposready = false;
        zoomready = false;
    }

	public float ObtainScale(){ //this function is used by launcher to determine what force is applied to the rocket on launch!
		//return trajectory.transform.localScale.y;
        //eventually perhaps add in fuel depletion, do not just suddenly change use exp to create smooth animation
		return 0.5f; //stubbed for testing and balancing
	}

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameData.dat");
        bf.Serialize(file, DataPlay); //saves data in
        Debug.LogWarning("Data saved");
        file.Close();
    }

    //unsure if need this function yet
    /*public void Load() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);
        DataPlay = (GameData)bf.Deserialize(file); //load data
        Debug.LogWarning("Data loaded");
        file.Close();
    }*/

    public void setStratCam() {
        CamMode = false; //set to dynacam
        CamTarget = new Vector3(DataPlay.stratviewposx[Application.loadedLevel], DataPlay.stratviewposy[Application.loadedLevel], DataPlay.stratviewposz[Application.loadedLevel]);
        CamZoom = DataPlay.stratviewsize[Application.loadedLevel];
        CamBound = 0.2f;
    }

    public void setActionCam() {
        CamTarget = new Vector3((StrongestPlanet.transform.position.x + player.transform.position.x) / 2, (StrongestPlanet.transform.position.y + player.transform.position.y) / 2, player.transform.position.z);
        CamZoom = 0.5f * dist;
        if (CamZoom <= 5f) { //dist less then like 9 at this point
            CamZoom = 5f;
        }
        CamBound = 0.2f;
        CamScale = 0.1f;
    }

    public void setinitials() {
        //Initialise
        DataPlay.PanSpeed = new float[totallevels];
        DataPlay.InitialFuel = new float[totallevels];
        DataPlay.xboundsx = new float[totallevels];
        DataPlay.xboundsy = new float[totallevels];
        DataPlay.yboundsx = new float[totallevels];
        DataPlay.yboundsy = new float[totallevels];
        DataPlay.stardirectx = new float[totallevels];
        DataPlay.stardirecty = new float[totallevels];
        DataPlay.stardirectz = new float[totallevels];
        DataPlay.stratviewposx = new float[totallevels];
        DataPlay.stratviewposy = new float[totallevels];
        DataPlay.stratviewposz = new float[totallevels];
        DataPlay.stratviewsize = new float[totallevels];
        DataPlay.BronzeRequirement = new float[totallevels];
        DataPlay.SilverRequirement = new float[totallevels];
        DataPlay.GoldRequirement = new float[totallevels];

        DataPlay.HighestFuel = new float[totallevels];
        DataPlay.TrophyLevel = new int[totallevels];
        DataPlay.completed = new int[totallevels];

        //Main Menu (index of 0)
        DataPlay.stardirectx[0] = 0.1f;
        DataPlay.stardirecty[0] = 0f;
        DataPlay.stardirectz[0] = 0f;

        //Level 0 (index of 3)
        DataPlay.GoldRequirement[3] = 3f;
        DataPlay.SilverRequirement[3] = 2f;
        DataPlay.BronzeRequirement[3] = 1f;
        DataPlay.PanSpeed[3] = 0.01f;
        DataPlay.InitialFuel[3] = 5f;
        DataPlay.stardirectx[3] = 0.1f;
        DataPlay.stardirecty[3] = 0f;
        DataPlay.stardirectz[3] = 0f;

        DataPlay.xboundsx[3] = -38f;
        DataPlay.xboundsy[3] = 38f;
        DataPlay.yboundsx[3] = -19f;
        DataPlay.yboundsy[3] = 19f;

        DataPlay.stratviewposx[3] = 0f;
        DataPlay.stratviewposy[3] = 0f;
        DataPlay.stratviewposz[3] = -10f;
        DataPlay.stratviewsize[3] = 17f;

        DataPlay.HighestFuel[3] =0f;
        DataPlay.completed[3] = 0;
        DataPlay.TrophyLevel[3] = 0;
    }
}

[Serializable]
public class GameData {

    //Configs
    public float[] PanSpeed; //load in whenever needed in CamStart()
    public float[] InitialFuel; //load in whenever level changed to an inlevel

    public float[] xboundsx; //on calc
    public float[] xboundsy;
    public float[] yboundsx;
    public float[] yboundsy;

    public float[] stardirectx; //load in on calc
    public float[] stardirecty;
    public float[] stardirectz;

    public float[] stratviewposx;
    public float[] stratviewposy;
    public float[] stratviewposz;

    public float[] stratviewsize; //load in on level change
    public float[] BronzeRequirement; //loaded in on calculation
    public float[] SilverRequirement; //^
    public float[] GoldRequirement;   //^


    //PlayerData
    public float[] HighestFuel; //caclulate on level success()
    public int[] TrophyLevel; //0=null, 1=bronze, 2=silver, 3=gold success()
    //todo, add unlocked levels
    public int[] completed; //0=no, 1=yes success()
}