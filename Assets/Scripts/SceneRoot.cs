using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        LogicManager.Instance.SetSceneRoots(gameObject.scene.buildIndex, gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
