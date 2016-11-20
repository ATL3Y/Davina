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
		AxKDebugLines.AddOBB (obb, Color.red);

		List<Interactable> availableInteractions = new List<Interactable>( );
       // if ( Input.GetMouseButtonDown( 0 ) )
        {
			Interactable[ ] interactions = GameObject.FindObjectsOfType<Interactable>( );
            for ( int i = 0; i < interactions.Length; i++ )
            {
				Interactable interaction = interactions[ i ];
				if ( !interaction.HasOwner() && interaction.useable && interaction.IsInInteractionRange( position, new Ray( position, forward ), obb ) )
                {
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

        if ( highestPriority != null )
        {
            highestPriority.IsSelected( );
			if ( ( left && ViveInputController.Instance.ReceivedLeftButtonDownSignal() ) || ( !left && ViveInputController.Instance.ReceivedRightButtonDownSignal()))
            {
                highestPriority.Use( this );

            }
        }
	
	}
}
