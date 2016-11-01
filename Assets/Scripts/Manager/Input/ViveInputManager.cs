using UnityEngine;
using System.Collections;

public class ViveInputManager : InputManager {

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (ViveInputController.Instance.ReceivedLeftButtonDownSignal()){
			FireSelectObject ( ClickType.LeftController ); // the left controller
		} else if(ViveInputController.Instance.ReceivedRightButtonDownSignal()){
			FireSelectObject ( ClickType.RightController ); //not the left controller
		}

		if (ViveInputController.Instance.ReceivedLeftPadDownSignal()){
			FireTransport ( );
		} else if(ViveInputController.Instance.ReceivedRightPadDownSignal()){
			FireTransport ();
		}


	}

	public override Ray[] GetCenterRayCast ()
	{
		//if ( ViveInputController.Instance.ReceivedLeftButtonPressSignal())
		Ray[] centers = new Ray[2];

		centers[0] = new Ray (ViveInputController.Instance.leftController.transform.position,
			ViveInputController.Instance.leftController.transform.forward);
		centers[1] = new Ray (ViveInputController.Instance.rightController.transform.position,
			ViveInputController.Instance.rightController.transform.forward);
		
		return centers;
	}

	public override void VibrateController (int index)
	{
		Debug.Log ("Vibrate vive input manager called index = " + index);
		ViveInputController.Instance.VibrateController (index);
	}

}
