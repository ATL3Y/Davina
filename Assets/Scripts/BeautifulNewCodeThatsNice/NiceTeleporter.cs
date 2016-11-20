using UnityEngine;
using System.Collections;

public class NiceTeleporter : Interactable {
	public PasserBy passerBy;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// if this is in the focus object list 
		/*
		if (ViveInputController.Instance.ReceivedLeftPadDownSignal || ViveInputController.Instance.ReceivedRightPadDownSignal) {
			for (int i = 0; i < InputManager.Instance.FocusedObject.Count; i++) 
			{
				PasserBy teleport = InputManager.Instance.FocusedObject [i];
				if (teleport != null && teleport.gameObject == this.gameObject) {
					Use ();
					break;
				}
			}
		}*/

	
	}

	public override void Use (Hands hand)
	{
		print ("haha");
		TransportManager.Instance.SetPasserBy (passerBy);
		InputManager.Instance.FocusedObject = passerBy; //@HACK0
		InputManager.Instance.FireTransport ();
	}
}
