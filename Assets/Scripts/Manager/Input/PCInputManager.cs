using UnityEngine;
using System.Collections;

public class PCInputManager : InputManager {

	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (Input.GetMouseButtonDown (0)) {
			FireSelectObject ();
		}
		if (Input.GetMouseButtonDown (1)) {
			FireTransport ();
		}
	}

	public override Ray GetCenterRayCast ()
	{
		return base.GetCenterRayCast ();
	}


}
