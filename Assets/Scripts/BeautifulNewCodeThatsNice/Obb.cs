using UnityEngine;

public class OBB
{
    public Vector3 point;
    public Vector3 up;
    public Vector3 right;
    public Vector3 forward;
    public Vector3 scale;
	public Vector3 e
	{
		get { return scale; }
	}
	public Vector3[] u
	{
		get { return new Vector3[ 3 ]{ right, up, forward }; }
	}

    public OBB( Vector3 point_INPUT, Quaternion quaternion, Vector3 scale_INPUT )
    {
        point = point_INPUT;
        right = quaternion * Vector3.right;
        up = quaternion * Vector3.up;
        forward = quaternion * Vector3.forward;
        scale = scale_INPUT;


    }

    public OBB( Matrix4x4 input )
    {
        point = new Vector3( input[ 0, 3 ], input[ 1, 3 ], input[ 2, 3 ] );
        right = new Vector3( input[ 0, 0 ], input[ 1, 0 ], input[ 2, 0 ] );
        up = new Vector3( input[ 0, 1 ], input[ 1, 1 ], input[ 2, 1 ] );
        forward = new Vector3( input[ 0, 2 ], input[ 1, 2 ], input[ 2, 2 ] );
        scale = new Vector3( right.magnitude, up.magnitude, forward.magnitude );
        right = right.normalized;
        up = up.normalized;
        forward = forward.normalized;
    }

    public Quaternion GetQuaternion( )
    {
        return Quaternion.LookRotation( up, forward );
    }


    public Matrix4x4 GetMatrix()
    {
        Matrix4x4 mat = new Matrix4x4( );
        Vector3 tR = right * scale.x;
        Vector3 tU = up * scale.y;
        Vector3 tF = forward * scale.z;

        mat.SetColumn( 0, new Vector4( tR.x, tR.y, tR.z, 0.0f ) );
        mat.SetColumn( 1, new Vector4( tU.x, tU.y, tU.z, 0.0f ) );
        mat.SetColumn( 2, new Vector4( tF.x, tF.y, tF.z, 0.0f ) );
        mat.SetColumn( 3, new Vector4( point.x, point.y, point.z, 1.0f ) );

        return mat;
    }

    public Vector3[] GetEdgePoints()
    {
        Vector3[ ] points = new Vector3[ 8 ];
        points[ 0 ] = point + right * scale.x + up * scale.y + forward * scale.z;
        points[ 1 ] = point + right * scale.x + up * scale.y + forward * -scale.z;
        points[ 2 ] = point + right * scale.x + up * -scale.y + forward * scale.z;
        points[ 3 ] = point + right * scale.x + up * -scale.y + forward * -scale.z;
        points[ 4 ] = point + right * -scale.x + up * scale.y + forward * scale.z;
        points[ 5 ] = point + right * -scale.x + up * scale.y + forward * -scale.z;
        points[ 6 ] = point + right * -scale.x + up * -scale.y + forward * scale.z;
        points[ 7 ] = point + right * -scale.x + up * -scale.y + forward * -scale.z;

        return points;
    }

    public Vector3[] GetTriangles()
    {
        Vector3[ ] tris = new Vector3[ 12 ];
        tris[ 0 ] = new Vector3( 0, 2, 3 );
        tris[ 1 ] = new Vector3( 0, 3, 1 );
        tris[ 2 ] = new Vector3( 2, 6, 7 );
        tris[ 3 ] = new Vector3( 2, 7, 3 );
        tris[ 4 ] = new Vector3( 4, 0, 1 );
        tris[ 5 ] = new Vector3( 4, 1, 5 );
        tris[ 6 ] = new Vector3( 1, 3, 7 );
        tris[ 7 ] = new Vector3( 1, 7, 5 );
        tris[ 8 ] = new Vector3( 4, 6, 2 );
        tris[ 9 ] = new Vector3( 4, 2, 0 );
        tris[ 10 ] = new Vector3( 5, 7, 6 );
        tris[ 11 ] = new Vector3( 5, 6, 4 );

        return tris;
    }

	static public bool TestOBBOBB(OBB a, OBB b)
	{
		float ra, rb;
		float[ , ] R = new float[4, 4];
		float[ , ] AbsR = new float[ 4, 4];
		// Compute rotation matrix expressing b in a’s coordinate frame
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				R[i,j] = Vector3.Dot(a.u[i], b.u[j]);
			}
		}

		// Compute translation vector t
		Vector3 t = b.point - a.point;
		// Bring translation into a’s coordinate frame
		t = new Vector3(Vector3.Dot(t, a.u[0]), Vector3.Dot(t, a.u[1]), Vector3.Dot(t, a.u[2]));
		// Compute common subexpressions. Add in an epsilon term to
		// counteract arithmetic errors when two edges are parallel and
		// their cross product is (near) null (see text for details)
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				AbsR[i, j] = Mathf.Abs(R[i, j]) + Mathf.Epsilon;
			}
		}
		// Test axes L = A0, L = A1, L = A2
		for (int i = 0; i < 3; i++) {
			ra = a.e[i];
			rb = b.e[0] * AbsR[i, 0] + b.e[1] * AbsR[i, 1] + b.e[2] * AbsR[i, 2];
			if (Mathf.Abs(t[i]) > ra + rb) return false;
		}
		// Test axes L = B0, L = B1, L = B2
		for (int i = 0; i < 3; i++) {
			ra = a.e[0] * AbsR[0, i] + a.e[1] * AbsR[1, i] + a.e[2] * AbsR[2, i];
			rb = b.e[i];
			if (Mathf.Abs(t[0] * R[0, i] + t[1] * R[1, i] + t[2] * R[2, i]) > ra + rb) return false;
		}
		// Test axis L = A0 x B0
		ra = a.e[1] * AbsR[2, 0] + a.e[2] * AbsR[1, 0];
		rb = b.e[1] * AbsR[0, 2] + b.e[2] * AbsR[0, 1];
		if (Mathf.Abs(t[2] * R[1, 0] - t[1] * R[2, 0]) > ra + rb) return false;
		// Test axis L = A0 x B1
		ra = a.e[1] * AbsR[2, 1] + a.e[2] * AbsR[1, 1];
		rb = b.e[0] * AbsR[0, 2] + b.e[2] * AbsR[0, 0];
		if (Mathf.Abs(t[2] * R[1, 1] - t[1] * R[2, 1]) > ra + rb) return false;
		// Test axis L = A0 x B2
		ra = a.e[1] * AbsR[2, 2] + a.e[2] * AbsR[1, 2];
		rb = b.e[0] * AbsR[0, 1] + b.e[1] * AbsR[0, 0];
		if (Mathf.Abs(t[2] * R[1, 2] - t[1] * R[2, 2]) > ra + rb) return false;
		// Test axis L = A1 x B0
		ra = a.e[0] * AbsR[2, 0] + a.e[2] * AbsR[0, 0];
		rb = b.e[1] * AbsR[1, 2] + b.e[2] * AbsR[1, 1];

		if (Mathf.Abs(t[0] * R[2, 0] - t[2] * R[0, 0]) > ra + rb) return false;
		// Test axis L = A1 x B1
		ra = a.e[0] * AbsR[2, 1] + a.e[2] * AbsR[0, 1];
		rb = b.e[0] * AbsR[1, 2] + b.e[2] * AbsR[1, 0];
		if (Mathf.Abs(t[0] * R[2, 1] - t[2] * R[0, 1]) > ra + rb) return false;
		// Test axis L = A1 x B2
		ra = a.e[0] * AbsR[2, 2] + a.e[2] * AbsR[0, 2];
		rb = b.e[0] * AbsR[1, 1] + b.e[1] * AbsR[1, 0];
		if (Mathf.Abs(t[0] * R[2, 2] - t[2] * R[0, 2]) > ra + rb) return false;
		// Test axis L = A2 x B0
		ra = a.e[0] * AbsR[1, 0] + a.e[1] * AbsR[0, 0];
		rb = b.e[1] * AbsR[2, 2] + b.e[2] * AbsR[2, 1];
		if (Mathf.Abs(t[1] * R[0, 0] - t[0] * R[1, 0]) > ra + rb) return false;
		// Test axis L = A2 x B1
		ra = a.e[0] * AbsR[1, 1] + a.e[1] * AbsR[0, 1];
		rb = b.e[0] * AbsR[2, 2] + b.e[2] * AbsR[2, 0];
		if (Mathf.Abs(t[1] * R[0, 1] - t[0] * R[1, 1]) > ra + rb) return false;
		// Test axis L = A2 x B2
		ra = a.e[0] * AbsR[1, 2] + a.e[1] * AbsR[0, 2];
		rb = b.e[0] * AbsR[2, 1] + b.e[1] * AbsR[2, 0];
		if (Mathf.Abs(t[1] * R[0, 2] - t[0] * R[1, 2]) > ra + rb) return false;
		// Since no separating axis is found, the OBBs must be intersecting
		return true;
	}
}