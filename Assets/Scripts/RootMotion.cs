using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    [SerializeField]
    GameObject targetGameObject;
    Vector3 target;
    float speed;
    private float upLimZ;
    private float lowLimZ;

    private void Start()
    {
        upLimZ = transform.position.z + 5.0f;
        lowLimZ = targetGameObject.transform.position.z - 5.0f;
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
            speed = Mathf.Abs ( Lens.instance.Dot );
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
