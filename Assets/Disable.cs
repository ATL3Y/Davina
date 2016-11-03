using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisableClidren(){
		foreach (Transform child in transform) {
			child.gameObject.SetActive (false);
		}
	}
}
