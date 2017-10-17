using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    [SerializeField]
    GameObject targetGameObject;
    Vector3 target;

    private void Start()
    {
        target = targetGameObject.transform.position - transform.position;
        target *= 100f; // make it unreachable
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 direction = Vector3.Normalize(target - transform.position);
        Vector3 toTarget = Vector3.Lerp(transform.position, transform.position + direction, Time.deltaTime * .03f);
        transform.position = new Vector3(toTarget.x, .23f, toTarget.z); //1.13f? 

        /*
        // If passed the target, maybe, have target turn? 
        if (Vector3.Dot(direction, transform.forward) < 0.0f)
        {

        }
        */
    }
}
