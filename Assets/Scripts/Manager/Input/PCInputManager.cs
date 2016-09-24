using UnityEngine;
using System.Collections;

public class PCInputManager : InputManager {


	protected override void MUpdate ()
	{
		base.MUpdate ();

		// when left click, select the object and transport
		if (Input.GetMouseButtonDown (0)) {
			FireSelectObject ();
			FireTransport ();
		}

//		if (Input.GetMouseButtonDown (1)) {
//			FireTransport ();
//		}

//		Debug.DrawLine (GetCenterRayCast ().origin, GetCenterRayCast ().direction * 1000f);
	}

	public override Ray GetCenterRayCast ()
	{
		return  Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));
	}
		
}
