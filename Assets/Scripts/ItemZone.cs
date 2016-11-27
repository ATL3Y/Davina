using UnityEngine;
using System.Collections;

public class ItemZone : MonoBehaviour 
{
	[SerializeField] GameObject root;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = root.transform.position;
		transform.rotation = root.transform.rotation * Quaternion.AngleAxis(90f, Vector3.up);
	}
}
