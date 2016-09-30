using UnityEngine;
using System.Collections;

public class MakeMesh : MonoBehaviour 
{
	private Color _color;
	bool once = true;
	Mesh m_mesh;
	Mesh m_meshOriginal;

	// Use this for initialization
	public void Start () 
	{

		_color = new Color ();
		ColorUtility.TryParseHtmlString ("#FFBDF7FF", out _color);
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
	}

	void Update ()
	{
		
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			if (m_mesh.GetTopology (0) != MeshTopology.Lines)
				m_mesh.SetIndices (m_mesh.triangles, MeshTopology.Lines, 0);
		} 
		else if (Input.GetKeyUp (KeyCode.Space)) 
		{
			m_mesh.vertices = m_meshOriginal.vertices;
			m_mesh.triangles = m_meshOriginal.triangles;

			for (int i = 0; i < m_meshOriginal.uv.Length; i++) 
			{
				m_mesh.uv [i] = m_meshOriginal.uv [i];
			}
		}

		
	}
		
	
	/*
	 
	public void LateUpdate (  ) 
	{
		if ( m_mesh.GetTopology(0) != MeshTopology.Lines )
			m_mesh.SetIndices (m_mesh.triangles, MeshTopology.Lines, 0);
	}
	*/

}
