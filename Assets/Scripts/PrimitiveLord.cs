using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrimitiveLord : MonoBehaviour 
{
	private List<GameObject> _primitives;
	private bool grow = false;
	public GameObject prefab;
	private float _delay = 1.0f;
	public int size = 20;

	void Start () 
	{
		_primitives = new List<GameObject> ();

		for (int i = 0; i < size; i++) 
		{
			Create (i);
		}
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			grow = true;
		}

		if (grow) 
		{
			_primitives [0].transform.localScale *= 1.1f; 
		}

		_delay -= Time.deltaTime;
		if (_delay <= 1.0f) 
		{
			//Create ();
			_delay = 1.0f;
		}
	}

	void Create()
	{
		GameObject myGameObject = Instantiate(prefab);
		Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 5.0f;
		myGameObject.transform.position = position;
		_primitives.Add (myGameObject);
	}

	void Create( int i )
	{
		GameObject myGameObject = Instantiate(prefab);
		myGameObject.transform.position = new Vector3 (i, i, i);
		_primitives.Add (myGameObject);
	}
		
}
