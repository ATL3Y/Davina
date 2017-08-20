using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXOutlinePulse : MonoBehaviour
{

    public GameObject myOutline;

    float pulseStartTime;
    GameObject pulseObject;

    float originalWidth;

    void SpawnPulse()
    {
        originalWidth = myOutline.GetComponent<Renderer>().material.GetFloat("_Outline");

        pulseObject = GameObject.Instantiate(myOutline);
        pulseObject.transform.SetParent(transform);
        pulseObject.transform.localPosition = Vector3.zero;
        pulseObject.transform.localRotation = Quaternion.identity;
        pulseObject.transform.localScale = new Vector3(1f, 1f, 1f);
        pulseStartTime = Time.time;
    }

    // Use this for initialization
    void Start ()
    {
        SpawnPulse();
	}

    
	// Update is called once per frame
	void Update ()
    {
        float timeSinceStart = Time.time - pulseStartTime;
        float scale = 1 + timeSinceStart;
        pulseObject.transform.localScale = new Vector3(scale, scale, scale);

        pulseObject.GetComponent<Renderer>().material.SetFloat("_Outline", originalWidth * (1 - timeSinceStart));

        if (timeSinceStart > 1)
        {
            GameObject.Destroy(pulseObject);
            SpawnPulse();
        }
	}
}
