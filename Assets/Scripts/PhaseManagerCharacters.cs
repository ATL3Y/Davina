using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManagerCharacters : MonoBehaviour
{
    private bool sawMom = false;
    private bool sawDavina = false;
    private bool sawBigD = false;
    private int travelledTo = 0;
    [SerializeField]
    GameObject phase2;

	// Use this for initialization
	void Start ()
    {
        phase2.SetActive ( false );
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void RegisterTravel ( string name )
    {
        if ( !sawMom && name == "Mom" )
        {
            sawMom = true;
            travelledTo++;
            if ( travelledTo == 2 )
            {
                // Enable big Davina
                phase2.SetActive ( true );
            }
        }

        if ( !sawDavina && name == "Davina" )
        {
            sawDavina = true;
            travelledTo++;
            if(travelledTo == 2 )
            {
                // Enable big Davina
                phase2.SetActive ( true );
            }
        }

        if ( !sawBigD && name == "BigD" )
        {
            sawBigD = true;
            travelledTo++;
            GetComponent<StoryObjManagerCharacters> ( ).enabled = true;
        }
    }
}
