using UnityEngine;
using System.Collections;

public class BoundsDetector : MonoBehaviour {

    public Bounds bounds;

	// Use this for initialization
	void Start () {

            bounds = new Bounds(Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f));
  
    }
	
	// Update is called once per frame
	void Update () {

        bounds.center = transform.localPosition;
	
	}
}
