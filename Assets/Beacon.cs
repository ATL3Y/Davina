using UnityEngine;
using System.Collections;

public class Beacon : MonoBehaviour 
{
	Interactable interactable;
	Color color;

	// Use this for initialization
	void Start () 
	{
		interactable = GetComponentInParent<Interactable> ();
		ColorUtility.TryParseHtmlString ("#FFA59550", out color);
		if (GetComponent<LineRenderer> () != null) {
			GetComponent<LineRenderer> ().SetColors (color, color);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (color.a <= 0f) {
			print ("done");
			return;
		}

		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.Euler(Vector3.up);

		if (interactable != null && interactable.Finished) {
			if (GetComponent<LineRenderer> () != null) {
				color.a -= .0005f;
				GetComponent<LineRenderer> ().SetColors (color, color);
			}
		} 


	}
		
}
