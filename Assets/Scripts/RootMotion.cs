using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MBehavior
{
    [SerializeField]
    GameObject targetGameObject;
    Vector3 target;
    float speed;
    private float upLimZ;
    private float lowLimZ;
    private float distance;

    protected override void MOnEnable ( )
    {
        base.MOnEnable ( );
        M_Event.logicEvents [ ( int ) LogicEvents.EnterStory ] += OnEnterStory;
    }

    protected override void MOnDisable ( )
    {
        base.MOnDisable ( );
        M_Event.logicEvents [ ( int ) LogicEvents.EnterStory ] -= OnEnterStory;
    }

    void OnEnterStory ( LogicArg arg )
    {
        // increase potential closeness each round 
        lowLimZ -= distance / 3.0f;
        Debug.Log ( "lowLimZ " + lowLimZ );
    }
    private void Start()
    {
        // Limit the mom's path. Give some padding. 
        upLimZ = transform.position.z + 1.0f;
        lowLimZ = targetGameObject.transform.position.z;
        Debug.Log ( "lowLimZ " + lowLimZ );
        distance = upLimZ - lowLimZ;
        // no motion at start
        lowLimZ = upLimZ - distance / 3.0f;
        Debug.Log ( "lowLimZ " + lowLimZ );
        target = targetGameObject.transform.position - transform.position;
        target *= 100f; // make it unreachable
        speed = 0.1f;
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 direction = Vector3.Normalize(target - transform.position);

        // If we're the dark side, go away
        if ( Lens.instance != null )
        {
            speed = Mathf.Abs ( Lens.instance.Dot ) / 2.0f;
            if( !Lens.instance.LightSide )
            {
                direction = -direction;
            } 
        }

        Vector3 toTarget = Vector3.Lerp(transform.position, transform.position + direction, speed * Time.deltaTime);
        toTarget = new Vector3 ( toTarget.x, .23f, toTarget.z );
        if ( toTarget.z > lowLimZ && toTarget.z < upLimZ )
        {
            transform.position = toTarget;
        }
    }
}
