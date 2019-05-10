using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class Hand : MonoBehaviour 
{
	public bool left = false;
    private Color selectColor = new Color(0.0f, 7.0f, 144.0f, 1.0f);

    // Use this for initialization
    void Start () 
	{
		if (name.Contains ("left"))
			left = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 position = transform.position;// ViveInputController.Instance.boundsLeftController;
        //Vector3 forward = Vector3.Normalize(1.5f * transform.forward - transform.up);
        Vector3 forward = transform.forward;
        //AxKDebugLines.AddLine(transform.position, transform.position + forward, selectColor, 0);
		Quaternion rotation = Quaternion.LookRotation (transform.forward, transform.up);
		Vector3 scale = new Vector3 (0.1f, 0.1f, 0.3f) * 0.2f;
		OBB obb = new OBB(position, rotation, scale);
		//AxKDebugLines.AddOBB (obb, Color.red);
		//AxKDebugLines.AddLine(transform.position, transform.position + transform.up * .1f, Color.green * .3f, 0);
		//AxKDebugLines.AddLine(transform.position, transform.position + transform.right * .1f, Color.red * .3f, 0);
		//AxKDebugLines.AddLine(transform.position, transform.position + transform.forward * .1f, Color.cyan * .3f, 0);

		//bool handFulls = false;
		List<Interactable> availableInteractions = new List<Interactable>();
        Interactable[ ] interactions = GameObject.FindObjectsOfType<Interactable>();

        for ( int i = 0; i < interactions.Length; i++ )
        {
            if ( interactions [ i ].GetOwner ( ) == this )
                return;
        }

        for ( int i = 0; i < interactions.Length; i++ )
        {
            Interactable interaction = interactions[i];
            // print ("int " + interaction.gameObject.name + " has owner " + interaction.HasOwner() +" is in range " + interaction.IsInInteractionRange (position, new Ray (position, forward), obb));
            if ( interaction.enabled && !interaction.HasOwner ( ) && interaction.useable && interaction.IsInInteractionRange ( position, new Ray ( position, forward ), obb ) )
            {
                // print (interaction.gameObject.name);
                availableInteractions.Add ( interaction );
            }
        }

        Interactable highestPriority = null;
        for (int i = 0; i < availableInteractions.Count; i++)
        {
            if (highestPriority == null || availableInteractions[i].priority > highestPriority.priority)
            {
                highestPriority = availableInteractions[i];
            }
        }

		bool triggerPressed = left && VRInputController.Instance.ReceivedLeftButtonDownSignal()
			|| !left && VRInputController.Instance.ReceivedRightButtonDownSignal();

        if (highestPriority != null)
        {
			//print ("highest priority is " + highestPriority);
            highestPriority.IsSelected();

            if (triggerPressed)
            {
                highestPriority.Use(this);
				//Debug.Log ("high priority " + highestPriority.name);
            }
        }
	}
}
