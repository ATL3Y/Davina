using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleContainer : MonoBehaviour
{

    //[SerializeField] Transform parent;
    [SerializeField] GameObject prefab;
    public NiceHole hole { get; set; } // cap later

    public Color color;

    public AudioClip hoverSound;
    public AudioClip storySound;

    [SerializeField]
    GameObject anchor;

    // Use this for initialization
    void OnEnable ()
    {
        //Debug.Log("onenable in holecon");
        //transform.SetParent(parent);

        GameObject temp = GameObject.Instantiate(prefab);
        if(temp.GetComponent<NiceHole>() != null)
        {
            hole = temp.GetComponent<NiceHole>();
        }
        else
        {
            Debug.Log("hole is null");
        }
        hole.transform.SetParent(this.transform);
        hole.transform.localPosition = Vector3.zero;
        hole.transform.localRotation = Quaternion.identity;
        //hole.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        hole.hoverSound = hoverSound;
        hole.storySound = storySound;

        foreach(MeshRenderer r in hole.outlineRenders)
        {
            r.material.SetVector("_OutlineColor", color);
        }

        hole.Color = color;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(anchor != null)
        {
            transform.position = anchor.transform.position + anchor.transform.forward * 1f - anchor.transform.up * .25f;
        }
	}
}
