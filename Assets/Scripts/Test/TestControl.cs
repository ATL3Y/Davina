using UnityEngine;
using System.Collections;

public class TestControl : MonoBehaviour {

	[SerializeField] float speed;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Camera cam = Camera.main;
		Vector3 speedV3 = Time.deltaTime * speed * ( Input.GetAxis ("Horizontal") * cam.transform.right + Input.GetAxis ("Vertical") * cam.transform.forward ) ;
		speedV3.y = 0;
		transform.position += speedV3;

	}

	void OnGUI(){
		GUILayout.Label (Input.GetAxis ("Horizontal") + " " + Input.GetAxis ("Vertical"));
	}
}
