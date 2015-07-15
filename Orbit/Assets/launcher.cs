using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class launcher : MonoBehaviour {
	private GameObject controller;
	public float differencex=0f;
	public float differencey=0f;
	public Vector3 pinpoint= new Vector3 (0,0,-10f); //where -10f is cam zoom pos
	public Vector3 inter3 = new Vector3(0,0,-10f);
	public Vector2 inter = new Vector2 (0,0);
	public Vector2 maincam2 = new Vector2 (0,0);
	public Vector2 player2= new Vector2(0,0);
	private GameController control;
	private DistanceJoint2D cable;
	private GameObject player;
	public GameObject MainCam;
	public bool interpolate=false;
	private Camera Cam;
	private GameObject trajectory;
	public Sprite trajectorysolid;
	public Sprite trajectorytrans;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		player=GameObject.Find("Rocket");
		control=controller.GetComponent<GameController>();
		MainCam=GameObject.Find("Main Camera");
		Cam=MainCam.GetComponent<Camera>();
		if (interpolate==true){
			if ((MainCam.transform.position.x!=player.transform.position.x)||(MainCam.transform.position.y!=player.transform.position.y)){
				maincam2.x=MainCam.transform.position.x;
				maincam2.y=MainCam.transform.position.y;
				player2.x=player.transform.position.x;
				player2.y=player.transform.position.y;
				inter=Vector2.Lerp(maincam2,player2,0.1f);
				inter3.x=inter.x;
				inter3.y=inter.y;
				MainCam.transform.position=inter3;
			}
			if (Cam.orthographicSize!=PlayerPrefs.GetFloat("PlayZoom")){
				Cam.orthographicSize=Mathf.Lerp (Cam.orthographicSize,PlayerPrefs.GetFloat("PlayZoom"),0.1f);
			}
			differencex=MainCam.transform.position.x - player.transform.position.x;
			differencey=MainCam.transform.position.y - player.transform.position.y;
			if (((Mathf.Abs(differencex)<0.1f)&&(Mathf.Abs(differencey)<0.1f))||control.camhook==true){
				pinpoint.x=player.transform.position.x;
				pinpoint.y=player.transform.position.y;
				MainCam.transform.position=pinpoint;
				control.camhook=true;
			}
		}
	}

	public void Launch(){
		controller=GameObject.Find ("GameController"); //finds gamecontroller
		player=GameObject.Find("Rocket");
		control=controller.GetComponent<GameController>();
		MainCam=GameObject.Find("Main Camera");
		trajectory=GameObject.Find ("Trajectory");
		interpolate=true;
		cable=gameObject.GetComponent<DistanceJoint2D>();
		player.transform.SetParent(null);
		Destroy(trajectory); // need to change to fade away possibly
		Destroy(cable);
		control.launched=true;
	}
}
