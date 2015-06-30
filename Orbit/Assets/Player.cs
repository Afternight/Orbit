using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Vector2 y = new Vector2 (1,1);
	private Vector2 mousePosition;
	public float maxGravDist = 100f;
	public float maxGravity = 0.5f;
	private Vector3 initial;
	//public float gravitational=0.0000000000667f;
	GameObject[] planets;
	SpriteRenderer sr;
	public Sprite rocket;
	public Sprite power;
	private int fuel;
	
	
	// Use this for initialization
	void Start () {
		fuel = 200;
		initial = transform.position;
		sr = GetComponent<SpriteRenderer> ();
		planets = GameObject.FindGameObjectsWithTag("Planet");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)&&fuel>0) { //when mouse is held down
			//Debug.Log ("I am alive");
			fuel=fuel-1;
			sr.sprite = power;
			Rigidbody2D x = GetComponent<Rigidbody2D> ();
			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			x.AddRelativeForce (y, ForceMode2D.Impulse);
		} else {
			sr.sprite = rocket;
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll){ //NEED TO CHANGE THIS TO PLANET OBJECTS AND OTHERS
		Application.LoadLevel (Application.loadedLevel); //resets level 
		//GameObject.Destroy (gameObject);
		//GameObject.Instantiate (gameObject,initial,transform.rotation);
	}
	
	void FixedUpdate () {
		foreach(GameObject planet in planets) { //iterates through planets
			float dist = Vector3.Distance(planet.transform.position, transform.position);
			if (dist <= maxGravDist) {
				maxGravity= 0.1f;//need to modify
				maxGravity=maxGravity/dist*2;// distance gets smaller!!!!
				Vector3 v = planet.transform.position - transform.position;
				GetComponent<Rigidbody2D>().AddForce(v.normalized * (1.0f - dist / maxGravDist) * maxGravity,ForceMode2D.Impulse); //change max gravity based on distance from object
			}
			
		}
	}
}
