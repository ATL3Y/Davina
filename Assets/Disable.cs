using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {
	public Disable() { s_Instance = this; }
	public static Disable Instance { get { return s_Instance; } }
	private static Disable s_Instance;

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
