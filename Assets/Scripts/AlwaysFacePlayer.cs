using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFacePlayer : MonoBehaviour
{
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 to = LogicManager.Instance.GetPlayerHeadTransform().position - transform.position;
        transform.rotation = Quaternion.LookRotation(to, Vector3.up);
	}
}
