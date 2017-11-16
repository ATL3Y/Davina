using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : Interactable
{
    private Color color;
    [SerializeField]
    MeshRenderer[] outlineRenders;
    [SerializeField]
    GameObject hoverObject;
    [SerializeField]
    Transform nextStartPos;

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

    public override void Use ( Hand hand )
    {
        if ( m_finished )
        {
            Debug.Log ( "finished" );
            return;
        }

        m_finished = true;
        if ( gameObject.name.Contains ( "Quit" ) )
        {
            TransportManager.Instance.StationaryEffect ( nextStartPos.position, false );

            if ( Application.isEditor )
            {
                //UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Application.Quit ( );
            }
        }
        else
        {
            // If we're in the tutorial, go to characters
            if(SceneManager.GetActiveScene().buildIndex == 1 )
            {
                TransportManager.Instance.StationaryEffect ( nextStartPos.position, true );
            }
            // If we're in the "white" ending, play the VO
            else if ( SceneManager.GetActiveScene ( ).buildIndex == 3 )
            {
                if ( VOEventAudio.instance != null )
                {
                    VOEventAudio.instance.PlayOnWhiteEnding ( );
                }
            }
        }
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
