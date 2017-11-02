using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXOutlinePulse : MBehavior
{
    public GameObject myOutline;

    float pulseStartTime;
    GameObject pulseObject;

    float originalWidth;

    Color originalColor;

    [SerializeField]
    bool multipleMeshes = false;
    protected SkinnedMeshRenderer[] outlineSkinnedMeshRenders;

    public void SpawnPulse()
    {
        if( multipleMeshes )
        {
            originalWidth = 20.0f;
            originalColor = Color.blue;
        }
        else
        {
            originalWidth = myOutline.GetComponent<Renderer> ( ).material.GetFloat ( "_Outline" );
            originalColor = myOutline.GetComponent<Renderer> ( ).material.GetColor ( "_OutlineColor" );
        }
        

        pulseObject = GameObject.Instantiate(myOutline);
        pulseObject.transform.SetParent(transform.root); // parent to scene root to it'll disappear
        pulseObject.transform.localPosition = transform.position;
        pulseObject.transform.localRotation = transform.rotation;
        pulseObject.transform.localScale = new Vector3(1f, 1f, 1f);
        pulseStartTime = Time.time;
    }

    // Use this for initialization
    void Start ()
    {
        SpawnPulse();
	}

	// Update is called once per frame
	void Update ()
    {
        if ( pulseObject == null )
            return;

        pulseObject.transform.localPosition = transform.position;
        pulseObject.transform.localRotation = transform.rotation;

        float timeSinceStart = Time.time - pulseStartTime;
        float scale = 1 + timeSinceStart;

        if ( multipleMeshes )
        {
            scale *= 3.0f;
            foreach ( SkinnedMeshRenderer r in pulseObject.GetComponentsInChildren<SkinnedMeshRenderer> ( ) )
            {
                r.enabled = true;
                r.material.SetFloat ( "_Outline", originalWidth * ( 1 - timeSinceStart ) );
                r.material.color = originalColor * ( 1 - 2.0f * timeSinceStart );
            }
        }
        else
        {
            pulseObject.GetComponent<Renderer> ( ).material.SetFloat ( "_Outline", originalWidth * ( 1 - timeSinceStart ) );
            myOutline.GetComponent<Renderer> ( ).material.color = originalColor * ( 1 - 2.0f * timeSinceStart );
        }

        pulseObject.transform.localScale = new Vector3 ( scale, scale, scale );

        if (timeSinceStart > 1)
        {
            GameObject.Destroy(pulseObject);
            SpawnPulse ( );
            if ( !multipleMeshes )
            {
                
            }
        }
	}

    // Clean up after transitions 
    protected override void MOnEnable()
    {
        base.MOnEnable();
        M_Event.logicEvents[(int)LogicEvents.ExitStory] += OnExitStory;
        M_Event.logicEvents[(int)LogicEvents.Characters] += OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
    }

    protected override void MOnDisable()
    {
        base.MOnDisable();
        M_Event.logicEvents[(int)LogicEvents.ExitStory] -= OnExitStory;
        M_Event.logicEvents[(int)LogicEvents.Characters] -= OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
    }

    void OnExitStory(LogicArg arg)
    {
        GameObject.Destroy(pulseObject);
    }

    void OnCharacters(LogicArg arg)
    {
        GameObject.Destroy(pulseObject);
    }

    void OnEnd(LogicArg arg)
    {
        GameObject.Destroy(pulseObject);
    }
}
