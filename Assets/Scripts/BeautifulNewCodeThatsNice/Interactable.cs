using UnityEngine;
using System.Collections;

public class Interactable: MonoBehaviour
{
    public bool insideBounds;
    public bool debug;
    public int priority;
    public string displayMessage;
    public bool useable = true;
    public Vector3 boundsMult = Vector3.one;
    private Bounds m_bounds;

	public Hand GetOwner( ) {
		return owner;
	}

	protected float m_hoverTime = 0.0f;

    private bool m_insideBoundsLastFrame;
	private Vector3 originalRot;

    float GetRadius( Bounds bounds )
    {
        return ( bounds.extents.x + bounds.extents.y + bounds.extents.z ) / 3.0f;
    }

	public bool IsInInteractionRange( Vector3 inputPosition, Ray inputLookDirection, OBB handObb )
    {
       
        bool returnValue = false;
        //might have to add obb suport
        if ( insideBounds )
        {
			bool flag = false;
			CheckOBB( ref flag, transform, handObb );
			returnValue = flag;
        }
        else
        {
            bool flag = false;
            Check( ref flag, transform, inputPosition, inputLookDirection );

            returnValue = flag;
        }

        if ( debug )
        {
           // if ( sphereInteractionBounds ) AxKDebugLines.AddFancySphere( m_bounds.center, GetRadius( m_bounds ), returnValue ? Color.green : Color.red );
           // else AxKDebugLines.AddBounds( m_bounds, returnValue ? Color.green : Color.red );
        }

        if ( returnValue )
        {
            //if ( displayMessage != "" ) RM.AH.SetActionText( displayMessage );
            InUseRange( );
        }
        m_insideBoundsLastFrame = returnValue;
        return returnValue;
    }

    public void IsSelected()
    {
		if ( displayMessage != "" ) Debug.Log( displayMessage );

        OnHover( );
    }

	void CheckOBB( ref bool flag, Transform t, OBB handObb )
	{
		for ( int i = 0; i < t.childCount; i++ )
		{
			CheckOBB( ref flag, t.GetChild( i ), handObb );
		}

		if ( t.GetComponent<MeshFilter>( ) != null )
		{
			Bounds b = t.GetComponent<MeshFilter>( ).mesh.bounds;
			Matrix4x4 mat = new Matrix4x4( );
			Vector3 tR = Vector3.right * b.extents.x * boundsMult.x;
			Vector3 tU = Vector3.up * b.extents.y * boundsMult.y;
			Vector3 tF = Vector3.forward * b.extents.z * boundsMult.z;

			mat.SetColumn( 0, new Vector4( tR.x, tR.y, tR.z, 0.0f ) );
			mat.SetColumn( 1, new Vector4( tU.x, tU.y, tU.z, 0.0f ) );
			mat.SetColumn( 2, new Vector4( tF.x, tF.y, tF.z, 0.0f ) );
			mat.SetColumn( 3, new Vector4( b.center.x, b.center.y, b.center.z, 1.0f ) );

			OBB obb = new OBB( t.localToWorldMatrix * mat );
			//if ( debug ) AxKDebugLines.AddOBB( obb, Color.white );

			flag = OBB.TestOBBOBB( obb, handObb );
		}
	}

    void Check( ref bool flag, Transform t, Vector3 inputPosition, Ray inputLookDirection )
    {
        for ( int i = 0; i < t.childCount; i++ )
        {
           Check( ref flag, t.GetChild( i ), inputPosition, inputLookDirection );
        }

        if ( t.GetComponent<MeshFilter>( ) != null )
        {
            Bounds b = t.GetComponent<MeshFilter>( ).mesh.bounds;
            Matrix4x4 mat = new Matrix4x4( );
            Vector3 tR = Vector3.right * b.extents.x * boundsMult.x;
            Vector3 tU = Vector3.up * b.extents.y * boundsMult.y;
            Vector3 tF = Vector3.forward * b.extents.z * boundsMult.z;

            mat.SetColumn( 0, new Vector4( tR.x, tR.y, tR.z, 0.0f ) );
            mat.SetColumn( 1, new Vector4( tU.x, tU.y, tU.z, 0.0f ) );
            mat.SetColumn( 2, new Vector4( tF.x, tF.y, tF.z, 0.0f ) );
            mat.SetColumn( 3, new Vector4( b.center.x, b.center.y, b.center.z, 1.0f ) );

            OBB obb = new OBB( t.localToWorldMatrix * mat );
            if ( debug ) AxKDebugLines.AddOBB( obb, Color.white );

            Vector3[ ] points = obb.GetEdgePoints( );
            Vector3[ ] tris = obb.GetTriangles( );
            for ( int i = 0; i < tris.Length; i++ )
            {
                Vector3 centroid = ( points[ ( int )tris[ i ].x ] + points[ ( int )tris[ i ].y ] + points[ ( int )tris[ i ].z ] ) / 3.0f;

                float range = 1000.25f;
                if ( Vector3.SqrMagnitude( centroid - inputLookDirection.origin ) > range && 
                    Vector3.SqrMagnitude( points[ ( int )tris[ i ].x ] - inputLookDirection.origin ) > range &&
                    Vector3.SqrMagnitude( points[ ( int )tris[ i ].y ] - inputLookDirection.origin ) > range &&
                    Vector3.SqrMagnitude( points[ ( int )tris[ i ].z ] - inputLookDirection.origin ) > range ) continue;

                bool d = Intersect( points[ ( int )tris[ i ].x ], points[ ( int )tris[ i ].y ], points[ ( int )tris[ i ].z ], inputLookDirection );
                if ( d )
                {
                    flag = true;
                }
                if ( debug )
                {
                    AxKDebugLines.AddLine( points[ ( int )tris[ i ].x ], points[ ( int )tris[ i ].y ], d ? Color.blue : Color.green );
                    AxKDebugLines.AddLine( points[ ( int )tris[ i ].y ], points[ ( int )tris[ i ].z ], d ? Color.blue : Color.green );
                    AxKDebugLines.AddLine( points[ ( int )tris[ i ].x ], points[ ( int )tris[ i ].z ], d ? Color.blue : Color.green );
                }
            }
        }
    }

    public static bool Intersect( Vector3 p1, Vector3 p2, Vector3 p3, Ray ray )
    {
        // Vectors from p1 to p2/p3 (edges)
        Vector3 e1, e2;

        Vector3 p, q, t;
        float det, invDet, u, v;


        //Find vectors for two edges sharing vertex/point p1
        e1 = p2 - p1;
        e2 = p3 - p1;

        // calculating determinant 
        p = Vector3.Cross( ray.direction, e2 );

        //Calculate determinat
        det = Vector3.Dot( e1, p );

        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if ( det > -Mathf.Epsilon && det < Mathf.Epsilon ) { return false; }
        invDet = 1.0f / det;

        //calculate distance from p1 to ray origin
        t = ray.origin - p1;

        //Calculate u parameter
        u = Vector3.Dot( t, p ) * invDet;

        //Check for ray hit
        if ( u < 0 || u > 1 ) { return false; }

        //Prepare to test v parameter
        q = Vector3.Cross( t, e1 );

        //Calculate v parameter
        v = Vector3.Dot( ray.direction, q ) * invDet;

        //Check for ray hit
        if ( v < 0 || u + v > 1 ) { return false; }

        if ( ( Vector3.Dot( e2, q ) * invDet ) > Mathf.Epsilon )
        {
            //ray does intersect
            return true;
        }

        // No hit at all
        return false;
    }

	protected Hand owner;

	public bool HasOwner() {
		return owner != null;
	}

    public virtual void Use( Hand hand )
    {
    }

    public virtual void InUseRange( )
    {
		m_hoverTime = Mathf.Clamp01 (m_hoverTime + Time.deltaTime * 4.0f);
    }

    public virtual void OnHover()
    {

    }

	public virtual void Update()
	{

		m_hoverTime = Mathf.Clamp01( m_hoverTime - Time.deltaTime * 2.0f );
	}

	public virtual void Start()
	{

	}

}