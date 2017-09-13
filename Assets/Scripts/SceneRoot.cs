using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    Transform startPos;
    [SerializeField]
    GameObject lights;
    [SerializeField]
    GameObject lightsVolumetric;

	// Use this for initialization
	void Start()
    {
        if(gameObject.scene.buildIndex == 1)
        {
            LogicManager.Instance.TutorialRoot = gameObject;
        }
        else
        {
            LogicManager.Instance.SetSceneRoots(gameObject.scene.buildIndex, gameObject);
        }
        

        if (startPos != null)
        {
            Debug.Log("Running scene root pt fyi and b4 " + LogicManager.Instance.transform.position);
            LogicManager.Instance.transform.position = startPos.position;
            LogicManager.Instance.transform.rotation = startPos.rotation;
            Debug.Log("Running scene root pt fyi and after " + LogicManager.Instance.transform.position);
        }
        if (lights != null && lightsVolumetric != null)
        {
            if (LogicManager.Instance.VolumetricLights)
            {
                lights.SetActive(false);
                lightsVolumetric.SetActive(true);
            }
            else
            {
                lights.SetActive(true);
                lightsVolumetric.SetActive(false);
            }
        }
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}
}
