using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class Hands : MonoBehaviour {
	public bool left;

	// Use this for initialization
	void Start () {
		left = gameObject.name.ToLower ().Contains ("left");
	}
	
	// Update is called once per frame
	void Update ()
	{

		Vector3 position = transform.position;// ViveInputController.Instance.boundsLeftController;
		Vector3 forward = transform.forward;

		Quaternion rotation = Quaternion.LookRotation (transform.forward, transform.up);
		Vector3 scale = new Vector3 (0.1f, 0.1f, 0.3f) * 0.2f;
		OBB obb = new OBB( position, rotation, scale );
		//AxKDebugLines.AddOBB (obb, Color.red);

		bool handFulls = false;
		List<Interactable> availableInteractions = new List<Interactable>( );
       // if ( Input.GetMouseButtonDown( 0 ) )
        {
			Interactable[ ] interactions = GameObject.FindObjectsOfType<Interactable>( );

			for (int i = 0; i < interactions.Length; i++) {
				if (interactions [i].GetOwner () == this)
					return;
			}

            for ( int i = 0; i < interactions.Length; i++ )
            {
				Interactable interaction = interactions[ i ];
				//print ("int " + interaction.gameObject.name + " has owner " + interaction.HasOwner() +" is in range " + interaction.IsInInteractionRange (position, new Ray (position, forward), obb));
				if ( interaction.enabled && !interaction.HasOwner() && interaction.useable && interaction.IsInInteractionRange( position, new Ray( position, forward ), obb ) )
                {
					//print (interaction.gameObject.name);
                    availableInteractions.Add( interaction );
                }
            }
        }
        
		Interactable highestPriority = null;
        for ( int i = 0; i < availableInteractions.Count; i++ )
        {
            if ( highestPriority == null || availableInteractions[ i ].priority > highestPriority.priority )
            {
                highestPriority = availableInteractions[ i ];
            }
        }

		bool triggerPressed = (left && ViveInputController.Instance.ReceivedLeftButtonDownSignal ()) || (!left && ViveInputController.Instance.ReceivedRightButtonDownSignal ());
		if (triggerPressed)
			AxKDebugLines.AddSphere (transform.position, 0.08f, Color.cyan, 0.01f);

        if ( highestPriority != null )
        {
			//print ("highest priority is " + highestPriority);

            highestPriority.IsSelected( );
			if ( triggerPressed )
            {
                highestPriority.Use( this );
				//Debug.Log ("high priorit " + highestPriority.name);

            }
        }
	
	}
}
