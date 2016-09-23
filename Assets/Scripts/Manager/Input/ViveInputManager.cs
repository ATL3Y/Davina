using UnityEngine;
using System.Collections;

public class ViveInputManager : InputManager {

	bool  lastLeftSignal = false;
	bool  lastRightSignal = false;
	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (ViveInputController.Instance.ReceivedLeftButtonDownSignal ()
		    || ViveInputController.Instance.ReceivedRightButtonDownSignal ())
			FireSelectObject ();

		if (ViveInputController.Instance.ReceivedLeftPadDownSignal ()
		    || ViveInputController.Instance.ReceivedRightPadDownSignal ())
			FireTransport ();

	}


	public override Ray GetCenterRayCast ()
	{
		if ( ViveInputController.Instance.ReceivedLeftButtonPressSignal())
			return new Ray (ViveInputController.Instance.leftController.transform.position,
				ViveInputController.Instance.leftController.transform.forward);
		
		return new Ray (ViveInputController.Instance.rightController.transform.position,
			ViveInputController.Instance.rightController.transform.forward);
	}
}
