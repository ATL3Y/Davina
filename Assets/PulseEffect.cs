using UnityEngine;
using System.Collections;

public class PulseEffect : MonoBehaviour {

	private Color color;

	// Use this for initialization
	void Start () {
		color = GetComponent<Renderer> ().material.color;
		GetComponent<Renderer> ().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
	}
	
	// Update is called once per frame
	void Update () {
		//print (Mathf.Abs (Mathf.Sin (Time.timeSinceLevelLoad)));
		color *= Mathf.Abs(Mathf.Sin (Time.timeSinceLevelLoad));
		print (color.a);
		GetComponent<Renderer> ().material.SetColor("_EmissionColor", color);
	
	}
}
