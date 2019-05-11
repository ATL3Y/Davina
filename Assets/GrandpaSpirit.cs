using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandpaSpirit : MonoBehaviour
{
    public static GrandpaSpirit instance;
    [SerializeField]
    FXOutlinePulse[] fxPulses;

	// Use this for initialization
	void Start ()
    {
        instance = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SpawnPulse ( )
    {
        foreach(FXOutlinePulse fx in fxPulses )
        {
            fx.SpawnPulse ( );
        }
    }
}
