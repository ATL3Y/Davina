using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidCam : Interactable
{
    private Color color;
    [SerializeField]
    MeshRenderer[] outlineRenders;
    [SerializeField]
    GameObject hoverObject;

    #region public functions

    #endregion

    #region private functions

    private void SetOutline ( bool value )
    {
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.enabled = value;
        }
    }

    private void SetHoverObject ( bool value )
    {
        if ( hoverObject != null )
        {
            hoverObject.SetActive ( value );
        }
    }

    #endregion

    #region inherited functions
    private void OnEnable ( )
    {
        m_finished = false;
        useable = true;
    }

    public override void Use ( Hand hand )
    {
        if ( m_finished )
            return;

        m_finished = true;

        // first play the final stuff
        Lens.instance.OnFinished ( );
    }

    public override void Update ( )
    {
        base.Update ( );

        // HOVER
        bool value = m_hoverTime > 0.0f;
        SetOutline ( value );
        SetHoverObject ( value );
    }

    public override void Start ( )
    {
        base.Start ( );
    }

    #endregion
}
