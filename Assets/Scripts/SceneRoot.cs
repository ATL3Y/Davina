using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    Transform startPos;

	// Use this for initialization
	void Start()
    {
        LogicManager.Instance.SetSceneRoots(gameObject.scene.buildIndex, gameObject);
        if (startPos != null)
        {
            Debug.Log("Running scene root pt fyi and b4 " + LogicManager.Instance.transform.position);
            LogicManager.Instance.transform.position = startPos.position;
            LogicManager.Instance.transform.rotation = startPos.rotation;
            Debug.Log("Running scene root pt fyi and after " + LogicManager.Instance.transform.position);
        }    
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}
}
