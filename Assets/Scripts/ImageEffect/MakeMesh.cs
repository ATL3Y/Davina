using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeMesh : MonoBehaviour 
{
	private Color m_color;
	bool once = true;
	private List <GameObject> m_childrenWithMeshes;
	private List <Mesh> m_childMeshes;
	private List <Mesh> m_childOriginalMeshes;

	public bool IsLine{ get { return m_isLine ; } }
	static bool m_isLine  = false ;

	// Use this for initialization
	public void Start () 
	{
		m_color = new Color ();
		ColorUtility.TryParseHtmlString ("#FFBDF7FF", out m_color);

		m_childrenWithMeshes = new List<GameObject>();
		m_childMeshes = new List<Mesh> ();
		m_childOriginalMeshes = new List<Mesh> ();

		foreach (Transform child in GetComponentsInChildren<Transform>()) 
		{
			if (child.tag == "Mesh") 
			{
				m_childrenWithMeshes.Add (child.gameObject);
				m_childMeshes.Add (new Mesh ());
				m_childOriginalMeshes.Add (new Mesh ());
			}
		}

		for (int i = 0; i < m_childMeshes.Count; i++) 
		{
			if(m_childrenWithMeshes[i].GetComponent<MeshFilter> () != null) m_childMeshes[i] = m_childrenWithMeshes[i].GetComponent<MeshFilter> ().sharedMesh;
			if ( m_childrenWithMeshes[i].GetComponent< MeshFilter >() != null ) m_childMeshes[i] = m_childrenWithMeshes[i].GetComponent< MeshFilter >().mesh; //added else
			else if ( m_childrenWithMeshes[i].GetComponent< SkinnedMeshRenderer >() != null ) m_childMeshes[i] = m_childrenWithMeshes[i].GetComponent< SkinnedMeshRenderer >().sharedMesh;
		}

		for (int i = 0; i < m_childOriginalMeshes.Count; i++) 
		{
			m_childOriginalMeshes[i].vertices = m_childMeshes [i].vertices;
			m_childOriginalMeshes[i].triangles = m_childMeshes [i].triangles;

			m_childOriginalMeshes[i].uv = new Vector2[m_childMeshes[i].uv.Length];
			/*
			for(int j=0; j<m_childOriginalMeshes[i].uv.Length; j++)
			{
				m_childOriginalMeshes[i].uv [j] = m_childMeshes [i].uv [j];
			}
			*/
		}

		/* previous version for one mesh on this gameobject
		if (GetComponent<MeshFilter> () != null) m_mesh = GetComponent<MeshFilter> ().sharedMesh;
		if ( GetComponent< MeshFilter >() != null ) m_mesh = GetComponent< MeshFilter >().mesh ;
		else if ( GetComponent< SkinnedMeshRenderer >() != null ) m_mesh = GetComponent< SkinnedMeshRenderer >().sharedMesh;

		m_meshOriginal = new Mesh ();
		m_meshOriginal.vertices = m_mesh.vertices;
		m_meshOriginal.triangles = m_mesh.triangles;

		for (int i = 0; i < m_meshOriginal.uv.Length; i++) 
		{
			m_meshOriginal.uv [i] = m_mesh.uv [i];
		}
		*/
	}

	void Update ()
	{

	}

	/// <summary>
	/// A list to record the MakeMesh with the trigger in
	/// </summary>
	private static List<MakeMesh> MakeMeshWithTriggerIn = new List<MakeMesh>();

	void OnTriggerEnter( Collider col )
	{
		if ( col.gameObject.tag == "GameController" || col.gameObject.tag == "Player") 
		{
			foreach (MakeMesh mesh in MakeMeshWithTriggerIn) {
				mesh.ToOriginal ();
			}

			ToLines ();
			MakeMeshWithTriggerIn.Add (this);
		}
	}

	void OnTriggerExit( Collider col )
	{
		if (col.gameObject.tag == "GameController" || col.gameObject.tag == "Player") {
			if (MakeMeshWithTriggerIn.Contains (this)) {
				MakeMeshWithTriggerIn.Remove (this);
			}
			if (m_isLine) {
				ToOriginal ();
				if (MakeMeshWithTriggerIn.Count > 0)
					MakeMeshWithTriggerIn [0].ToLines ();
			}
		}

	}

	void ToLines()
	{
		print ("in to lines, child count = " + m_childrenWithMeshes.Count + "and child mesh count = " + m_childMeshes.Count);
		for (int i = 0; i < m_childrenWithMeshes.Count; i++) 
		{
			if (m_childMeshes[i].GetTopology (0) != MeshTopology.Lines)
				m_childMeshes[i].SetIndices (m_childMeshes[i].triangles, MeshTopology.Lines, 0);
		}
		m_isLine = true;
	}

	void ToOriginal()
	{
		for (int i = 0; i < m_childMeshes.Count; i++) 
		{
			m_childMeshes[i].vertices = m_childOriginalMeshes[i].vertices;
			m_childMeshes[i].triangles = m_childOriginalMeshes[i].triangles;

			/*
			for (int j = 0; j < m_childMeshes[i].uv.Length; j++) //from original
			{
				m_childMeshes[i].uv [j] = m_childOriginalMeshes[i].uv [j];
			}
			*/

		}
		m_isLine = false;
	}



	void OnDisable()
	{
		ToOriginal ();
	}
		
	
	/*
	public void LateUpdate (  ) 
	{
		if ( m_mesh.GetTopology(0) != MeshTopology.Lines )
			m_mesh.SetIndices (m_mesh.triangles, MeshTopology.Lines, 0);
	}
	*/
}
