using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchIK : MonoBehaviour
{

    #region private data
    [SerializeField] Transform spine;

    // original Reverse IK nodes 
    [SerializeField] Transform head;
    [SerializeField] Transform lHand;
    [SerializeField] Transform rHand;

    // copy Reverse IK nodes 
    [SerializeField] Transform headCopy;
    [SerializeField] Transform lHandCopy;
    [SerializeField] Transform rHandCopy;

    // Bones to stretch
    private Transform[] spineTs;
    private Transform[] lArmTs;
    private Transform[] rArmTs;

    // Note: we have to do this with position in order to know we're crossing the correct distance 
    private float[] spineOrigLocYs;
    private float[] lArmOrigLocYs;
    private float[] rArmOrigLocYs;

    private bool debug = false;

    #endregion

    #region private functions


    #endregion

    #region inherited functions

    void Update ()
    {
        // Reset to original local Y
        for(int i = 0; i < spineTs.Length; i++ )
        {
            Vector3 pos = spineTs [ i ].localPosition;
            spineTs [ i ].localPosition = new Vector3 ( pos.x, spineOrigLocYs [ i ], pos.z );
        }

        for ( int i = 0; i < lArmTs.Length; i++ )
        {
            Vector3 pos = lArmTs [ i ].localPosition;
            lArmTs [ i ].localPosition = new Vector3 ( pos.x, lArmOrigLocYs [ i ], pos.z );
        }

        for ( int i = 0; i < rArmTs.Length; i++ )
        {
            Vector3 pos = rArmTs [ i ].localPosition;
            rArmTs [ i ].localPosition = new Vector3 ( pos.x, rArmOrigLocYs [ i ], pos.z );
        }

        // Check for the delta distance between the originals and copies
        float deltaHead = Vector3.Magnitude(headCopy.position - head.position);
        float deltaLHand = Vector3.Magnitude(lHandCopy.position - lHand.position); 
        float deltaRHand = Vector3.Magnitude(rHandCopy.position - rHand.position);

        // If there is a gap, bridge it
        if (deltaHead > 0.0f )
        {
            for ( int i = 0; i < spineTs.Length; i++ )
            {
                Vector3 pos = spineTs [ i ].localPosition;
                spineTs [ i ].localPosition = new Vector3 ( pos.x, deltaHead / spineTs.Length + spineOrigLocYs [ i ], pos.z );
            }
        }

        if ( deltaLHand > 0.0f )
        {
            for ( int i = 0; i < lArmTs.Length; i++ )
            {
                Vector3 pos = lArmTs [ i ].localPosition;
                lArmTs [ i ].localPosition = new Vector3 ( pos.x, deltaLHand / lArmTs.Length + lArmOrigLocYs [ i ], pos.z );
            }
        }

        if ( deltaRHand > 0.0f )
        {
            for ( int i = 0; i < rArmTs.Length; i++ )
            {
                Vector3 pos = rArmTs [ i ].localPosition;
                rArmTs [ i ].localPosition = new Vector3 ( pos.x, deltaRHand / rArmTs.Length + rArmOrigLocYs [ i ], pos.z );
            }
        }

        if ( debug )
        {
            for ( int i = 0; i < spineTs.Length; i++ )
            {
                AxKDebugLines.AddFancySphere ( spineTs [ i ].position, 0.1f, Color.yellow, 0 );
            }

            AxKDebugLines.AddFancySphere ( head.position, 0.2f, Color.red, 0 );
            AxKDebugLines.AddFancySphere ( headCopy.position, 0.2f, Color.black, 0 );

            AxKDebugLines.AddFancySphere ( lHand.position, 0.2f, Color.blue, 0 );
            AxKDebugLines.AddFancySphere ( lHandCopy.position, 0.2f, Color.green, 0 );
            Debug.Log ( deltaHead + ", " + deltaLHand + ", " + deltaRHand );
        }
    }

    void Start ( )
    {
        // Store the spine transforms we want to stretch
        spineTs = new Transform [ ] {
            spine, // Spine
            head.parent.parent.parent, // Spine1
            head.parent.parent, // Spine2
            head.parent // Neck
            };

        // Store the spine's original Y positions
        spineOrigLocYs = new float [ spineTs.Length ];
        for(int i = 0; i < spineOrigLocYs.Length; i++ )
        {
            spineOrigLocYs [ i ] = spineTs [ i ].localPosition.y;
        }

        // Store the left arm transforms we want to stretch
        lArmTs = new Transform [ ] {
            lHand.parent.parent, // LeftArm
            lHand.parent, // LeftForeArm
            lHand, // LeftHand
            };

        // Store the left arm's original Y positions
        lArmOrigLocYs = new float [ lArmTs.Length ];
        for ( int i = 0; i < lArmOrigLocYs.Length; i++ )
        {
            lArmOrigLocYs [ i ] = lArmTs [ i ].localPosition.y;
        }

        // Store the right arm transforms we want to stretch
        rArmTs = new Transform [ ] {
            rHand.parent.parent, // RightArm
            rHand.parent, // RightForeArm
            rHand, // RightHand
            };

        // Store the right arm's original Y positions
        rArmOrigLocYs = new float [ rArmTs.Length ];
        for ( int i = 0; i < rArmOrigLocYs.Length; i++ )
        {
            rArmOrigLocYs [ i ] = rArmTs [ i ].localPosition.y;
        }
    }

    #endregion
}
