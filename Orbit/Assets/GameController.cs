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
	
	}
	public void SceneSwitchers (int target) {
		Application.LoadLevel(target);
	}
	public bool UiDetect(){
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		//Debug.Log ("Target Position: " + hit.rigidbody);
		if (hit.rigidbody!=null&&hit.transform.parent!=null){
			if (hit.transform.parent.name=="Canvas"){
				Debug.Log("canvaspansas");
				return true;
			} else {
				Debug.Log ("nothing");
				return false;
			}
		}
		return false;
	}
}
