using UnityEngine;
using System.Collections;

public class MergeEffect : MBehavior {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3 (.1f * Mathf.Sin (Time.time), 0, .1f * Mathf.Cos (Time.time));
	}
}
