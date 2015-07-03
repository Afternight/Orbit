using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public static GameController control;
	public int fuel=200;
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
}
