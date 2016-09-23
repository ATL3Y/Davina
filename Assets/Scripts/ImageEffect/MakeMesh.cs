using UnityEngine;
using System.Collections;

public class MakeMesh : MonoBehaviour 
{
	private Color _color;
	bool once = true;
	Mesh m_mesh;

	// Use this for initialization
	public void Start () 
	{
		_color = new Color ();
		ColorUtility.TryParseHtmlString ("#FFBDF7FF", out _color);
		if (GetComponent<MeshFilter> () != null) m_mesh = GetComponent<MeshFilter> ().sharedMesh;
		if ( GetComponent< MeshFilter >() != null ) m_mesh = GetComponent< MeshFilter >().mesh ;
		else if ( GetComponent< SkinnedMeshRenderer >() != null ) m_mesh = GetComponent< SkinnedMeshRenderer >().sharedMesh;
	}

	public void Update ()
	{
		
	}
	
	// Update is called once per frame
	public void LateUpdate (  ) 
	{
		//if (once) {
		if ( m_mesh.GetTopology(0) != MeshTopology.Lines )
			m_mesh.SetIndices (m_mesh.triangles, MeshTopology.Lines, 0);
			once = false;
		//}

		//AxKDebugLines.AddMesh (transform, GetComponent<MeshFilter>().mesh, _color, 0);
	}
}
