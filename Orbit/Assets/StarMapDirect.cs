using UnityEngine;
using System.Collections;

public class StarMapDirect : MonoBehaviour {
    public static StarMapDirect StarMap;
    public Vector3 MapSpeed = new Vector3(0.1f, 0f, 0f);

    void Awake() {
        if (StarMap == null) { //this script ensures persistance, if one does not exist one is created
            DontDestroyOnLoad(gameObject);
            StarMap = this;
        } else if (StarMap != this) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //Star map code
        //perhaps put in here a load value which loads the movement amount from a level store
        //that way can make stars move towards goal every time to make it more clear subconciously
        //Modify map speed globally from gamecontroller to do stuff
        this.transform.position += Time.deltaTime * MapSpeed;
    }
}
