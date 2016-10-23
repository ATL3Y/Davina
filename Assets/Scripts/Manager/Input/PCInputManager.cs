using UnityEngine;
using System.Collections;

public class PCInputManager : InputManager {


	protected override void MUpdate ()
	{
		base.MUpdate ();

		// when left click, select the object and transport
		if (Input.GetMouseButtonDown (0) || Input.GetKeyDown(KeyCode.Space) ) {
			FireSelectObject ( ClickType.Mouse );
			FireTransport ();
		}

//		if (Input.GetMouseButtonDown (1)) {
//			FireTransport ();
//		}

//		Debug.DrawLine (GetCenterRayCast ().origin, GetCenterRayCast ().direction * 1000f);
	}

	public override Ray[] GetCenterRayCast ()
	{
		Ray[] centers = new Ray[2];

		if (Camera.main != null) {
			centers[0] = Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));
			centers[1] = Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));
			return centers;
		}

		centers [0] = new Ray (Vector3.zero, Vector3.forward);
		return centers;
	}
		
}
