using UnityEngine;
using System.Collections;

public class ItemZone : MonoBehaviour 
{
	[SerializeField] GameObject root;

	// Use this for initialization
	void Start () 
	{
        if (gameObject.name.Contains("Mother"))
        {
            transform.rotation = Quaternion.AngleAxis(90f, root.transform.right) * root.transform.rotation;
        } else
        {
            transform.rotation = root.transform.rotation;
        }
        transform.position = root.transform.position;
    }
}
