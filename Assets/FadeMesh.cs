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

	// Use this for initialization
	void Start () {
		SetWireFrame (inside);
	}

	void SetAlpha (Transform t, float value)
	{
		for (int i = 0; i < t.childCount; i++) {
			SetAlpha (t.GetChild (i), value);
		}
			
		Renderer renderer = t.GetComponent< Renderer > ();
		if (renderer != null) {
			if (!renderer.material.HasProperty ("_Color")) {
			} else {
				//print (renderer.gameObject.name);
				Color tempColor = renderer.material.color;
				tempColor.a = value;
				renderer.material.color = tempColor;// ("_Color", tempColor);
			}
		} else {

		}
	}

	void SetWireFrame (Transform t )
	{
		for (int i = 0; i < t.childCount; i++) {
			SetWireFrame (t.GetChild (i) );
		}

		SkinnedMeshRenderer filter = t.GetComponent< SkinnedMeshRenderer > ();
		if (filter != null) {
			//print (t.name);
			Mesh tempMesh = (Mesh)Instantiate( filter.sharedMesh );

			//Mesh tempMesh = new Mesh ();// filter.sharedMesh;
			//tempMesh.vertices = filter.sharedMesh.vertices;
			//tempMesh.normals = filter.sharedMesh.normals;
			//tempMesh.uv = filter.sharedMesh.uv;

			/*
			Mesh tempMesh = new Mesh ();
			filter.BakeMesh (tempMesh);

			Vector3[] verts = tempMesh.vertices;
			for (int i = 0; i < tempMesh.vertexCount; i++) {
				//verts [i] = t.rotation * verts [i];
				//verts [i] += t.position;
			}
			tempMesh.vertices =verts;*/

			tempMesh.SetIndices (filter.sharedMesh.GetIndices (0), MeshTopology.Lines, 0);

			filter.sharedMesh = tempMesh;
		}
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if (Input.GetKeyDown (KeyCode.UpArrow))
			fadeAmount += 0.1f;
		else if (Input.GetKeyDown (KeyCode.DownArrow))
			fadeAmount -= 0.1f;

		float d = Vector3.Distance (center.position, LogicManager.Instance.GetPlayerHeadTransform ().position);
		d += .05f; //offset
		fadeAmount = Mathf.Clamp01(d * d * d);
        */
        fadeAmount = (Score.Instance.GetScore() + 3f) / 5.2f;
        //print("score " + fadeAmount);

		if (!transform.root.gameObject.GetComponent<MCharacter> ().IsInInnerWorld) {
			SetAlpha (outside, fadeAmount);
			SetAlpha (inside, 1.0f - fadeAmount);
		}
	}

	void FadeToLine(){
		//print ("fade to line");
		foreach (Transform child in GetComponentsInChildren<Transform>()) {
			if (child.tag == "NonMesh") {
				child.gameObject.GetComponent<Renderer> ().enabled = false;
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

		foreach (Transform child in GetComponentsInChildren<Transform>()) {
			if (child.tag == "NonMesh") {
				child.gameObject.GetComponent<Renderer> ().enabled = true;
			}
		}
	}
}
