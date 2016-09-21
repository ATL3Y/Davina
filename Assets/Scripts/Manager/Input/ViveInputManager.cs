using UnityEngine;
using System.Collections;

public class ViveInputManager : InputManager {

	bool  lastLeftSignal = false;
	bool  lastRightSignal = false;
	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (ViveInputController.instance.ReceivedLeftButtonDownSignal ()
		    || ViveInputController.instance.ReceivedRightButtonDownSignal ())
			FireSelectObject ();

		if (ViveInputController.instance.ReceivedLeftPadDownSignal ()
		    || ViveInputController.instance.ReceivedRightPadDownSignal ())
			FireTransport ();

	}


	public override Ray GetCenterRayCast ()
	{
		if ( ViveInputController.instance.ReceivedLeftButtonPressSignal())
			return new Ray (ViveInputController.instance.leftController.transform.position,
				ViveInputController.instance.leftController.transform.forward);
		
		return new Ray (ViveInputController.instance.rightController.transform.position,
			ViveInputController.instance.rightController.transform.forward);
	}
}
