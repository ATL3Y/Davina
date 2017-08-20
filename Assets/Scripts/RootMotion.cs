using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    [SerializeField]
    GameObject targetGameObject;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 direction = Vector3.Normalize(targetGameObject.transform.position - transform.position);

        // If passed the target, just go forwards. 
        if (Vector3.Dot(direction, transform.forward) < 0.0f)
        {
            direction = transform.forward;
        }

        Vector3 target = Vector3.Lerp(transform.position, transform.position + direction, Time.deltaTime * .05f);
        transform.rotation = Quaternion.AngleAxis(27.0f, transform.right) * Quaternion.LookRotation(direction, transform.up);
        transform.position = new Vector3(target.x, 1.13f, target.z);
        
	}
}
