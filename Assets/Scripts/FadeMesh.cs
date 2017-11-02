using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeMesh : MonoBehaviour 
{
    private List <GameObject> m_childrenWithMeshes;
	private List <GameObject> m_childrenWithAlphas;

	private List <Mesh> m_childMeshes;
	private List <Mesh> m_childMeshesOriginal;

	public bool IsFadeToLine{ get { return m_FadeToLine ; } }
	static bool m_FadeToLine  = false;

	public float fadeAmount = 0.0f;
	public Transform outside;
	public Transform inside;

	[SerializeField] Transform center;
    private bool m_set = false;

	// Use this for initialization
	void Start ()
    {
		SetWireFrame (inside);
    }

    // Update is called once per frame
    void Update()
    {
        // fadeAmount: 1 is opaque. 0 is transparent.
        // score: 1 is light and -1 is dark 
        // the score goes from -2 to 2 in tutorial and -3 to 3 in characters 

        Lens currentLens = Lens.instance; //LogicManager.Instance.GetCurrentLens();
        if ( currentLens != null )
        {
            fadeAmount = ( currentLens.Dot + 1.0f ) / 2.0f;
            // Debug.Log ( "fadeAmount is " + fadeAmount );
        }
        // Otherwise look normal
        else
        {
            // Debug.Log ( "currentLens is null." );
            fadeAmount = 1.0f;
        }

        /*
        if (Score.Instance.GetScore() == 0)
        {
            fadeAmount = .5f;
        }
        else if(Score.Instance.GetScore() == 1)
        {
            fadeAmount = .7f;
        }
        else if (Score.Instance.GetScore() == 2)
        {
            fadeAmount = .8f;
        }
        else if (Score.Instance.GetScore() == 3)
        {
            fadeAmount = .9f;
        }
        else if (Score.Instance.GetScore() == -1)
        {
            fadeAmount = .3f;
        }
        else if (Score.Instance.GetScore() == -2)
        {
            fadeAmount = .2f;
        }
        else if (Score.Instance.GetScore() == -3)
        {
            fadeAmount = .1f;
        }
        */

        if (!GetComponent<MCharacter>().IsInInnerWorld)
        {
            SetAlpha(outside, fadeAmount);
            SetAlpha(inside, 1.0f - fadeAmount);
        }
    }

    void SetAlpha (Transform t, float value)
	{
		for (int i = 0; i < t.childCount; i++)
        {
			SetAlpha (t.GetChild (i), value);
		}
			
		Renderer renderer = t.GetComponent< Renderer > ();
		if (renderer != null)
        {
			if (renderer.material.HasProperty ("_Color"))
            {
				//print (renderer.gameObject.name);
				Color tempColor = renderer.material.color;
				tempColor.a = value;
				renderer.material.color = tempColor;// ("_Color", tempColor);
			}
		} 
	}

	void SetWireFrame (Transform t )
	{
		for (int i = 0; i < t.childCount; i++)
        {
			SetWireFrame (t.GetChild (i) );
		}

		SkinnedMeshRenderer filter = t.GetComponent< SkinnedMeshRenderer > ();
		if (filter != null)
        {
			//print (t.name);
			Mesh tempMesh = (Mesh)Instantiate( filter.sharedMesh );
			tempMesh.SetIndices (filter.sharedMesh.GetIndices (0), MeshTopology.Lines, 0);
			filter.sharedMesh = tempMesh;
		}
	}
	
    /*
	void FadeToLine(){
		//print ("fade to line");
		foreach (Transform child in outside.GetComponentsInChildren<Transform>()) {
			if (child.tag == "NonMesh") {
				child.gameObject.GetComponent<Renderer> ().enabled = false;
            }
		}
        foreach (Transform child in inside.GetComponentsInChildren<Transform>())
        {
            if (child.tag == "NonMesh")
            {
                child.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }

        for (int i = 0; i < m_childrenWithMeshes.Count; i++) {
			if (m_childMeshes [i].GetTopology (0) != MeshTopology.Lines) {
				m_childMeshes [i].SetIndices (m_childMeshes [i].triangles, MeshTopology.Lines, 0);
			}
		}
	}

	void ToOriginal(){
		//print ("to original");

		for (int i = 0; i < m_childrenWithMeshes.Count; i++) {
			m_childMeshes [i].vertices = m_childMeshesOriginal [i].vertices;
			m_childMeshes [i].triangles = m_childMeshesOriginal [i].triangles;
		}

        foreach (Transform child in inside.GetComponentsInChildren<Transform>())
        {
            if (child.tag == "NonMesh")
            {
                child.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }

        foreach (Transform child in outside.GetComponentsInChildren<Transform>()) {
			if (child.tag == "NonMesh") {
				child.gameObject.GetComponent<Renderer> ().enabled = true;
			}
		}
	}
    */
}
