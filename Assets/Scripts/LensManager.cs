using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensManager : MBehavior
{
    [SerializeField]
    List<GameObject> storyObjA;
    [SerializeField]
    List<GameObject> storyObjB;
    [SerializeField]
    List<GameObject> storyObjC;

    public Transform observePos;

    [SerializeField]
    Interactable videoCam;
    public static LensManager instance;

    private List<GameObject> currentStory = new List<GameObject>();

    private int count = -1;

    public void EnableCamera ( )
    {
        if (videoCam != null )
        {
            videoCam.gameObject.SetActive ( true );
        }
        else
        {
            Debug.Log ( "cam not there" );
        }
    }

    protected override void MOnEnable ( )
    {
        base.MOnEnable ( );
        M_Event.logicEvents [ ( int ) LogicEvents.Tutorial ] += OnTutorial;
        M_Event.logicEvents [ ( int ) LogicEvents.EnterStory ] += OnEnterStory;
        M_Event.logicEvents [ ( int ) LogicEvents.ExitStory ] += OnExitStory;
        M_Event.logicEvents [ ( int ) LogicEvents.Characters ] += OnCharacters;
    }

    protected override void MOnDisable ( )
    {
        base.MOnDisable ( );
        M_Event.logicEvents [ ( int ) LogicEvents.Tutorial ] -= OnTutorial;
        M_Event.logicEvents [ ( int ) LogicEvents.EnterStory ] -= OnEnterStory;
        M_Event.logicEvents [ ( int ) LogicEvents.ExitStory ] -= OnExitStory;
        M_Event.logicEvents [ ( int ) LogicEvents.Characters ] -= OnCharacters;
    }

    void OnEnterStory ( LogicArg arg )
    {
        M_Event.FireLogicEvent ( LogicEvents.ExitStory, new LogicArg ( this ) );
        count++;

        if( GetStory() == null )
        {
            // Run the finale after 3 sec
            if ( gameObject.scene.buildIndex == 2 )
            {
                timer = 3f;
                runTimer = true;
                return;
            }
        }

        currentStory = GetStory ( );
        for ( int i = 0; i < currentStory.Count; i++ )
        {
            currentStory [ i ].SetActive ( true );
        }
    }

    float timer = 0f;
    bool runTimer = false;
    protected override void MUpdate ( )
    {
        base.MUpdate ( );

        // Once we're all done, run a little timer to delay the finale for a sec
        if ( runTimer )
        {
            timer -= Time.deltaTime;
            if ( timer <= 0f )
            {
                runTimer = false;
                if ( gameObject.scene.buildIndex == 2 ) // characters scene
                {
                    M_Event.FireLogicEvent ( LogicEvents.Finale, new LogicArg ( this ) );
                }
            }
        }

        if ( Lens.instance != null )
        {
            if(currentStory.Count > 2 )
            {
                /*
                if ( Lens.instance.LightSide )
                {
                    if ( currentStory [ 2 ] != null && currentStory [ 3 ] != null )
                    {
                        currentStory [ 2 ].SetActive ( true );
                        currentStory [ 3 ].SetActive ( false );
                    }
                }
                else
                {
                    if ( currentStory [ 2 ] != null && currentStory [ 3 ] != null )
                    {
                        currentStory [ 2 ].SetActive ( false );
                        currentStory [ 3 ].SetActive ( true );
                    }
                }
                */
            }

        }
    }

    // Exit last story before entering new one 
    void OnExitStory ( LogicArg arg )
    {
        videoCam.gameObject.SetActive ( false );
        for ( int i = currentStory.Count - 1; i >= 0; i-- )
        {
            Lens lens = currentStory[i].GetComponent<Lens>();
            if ( lens != null && lens != ( Lens ) arg.sender )
            {
                Debug.Log ( "in OnExitStory deactivating" );
                currentStory [ i ].SetActive ( false );
            }
        }

        for ( int i = 0; i < currentStory.Count; i++ )
        {
            for ( int j = 0; j < currentStory [ i ].transform.childCount; j++ )
            {
                currentStory [ i ].transform.GetChild ( j ).gameObject.SetActive ( false );
            }
            currentStory [ i ].SetActive ( false );
        }
    }

    // Returns the next batch of story obj
    List<GameObject> GetStory ( )
    {
        switch ( count )
        {
            case 0:
                if ( storyObjA.Count > 0 )
                {
                    return storyObjA;
                }
                else
                {
                    return null;
                }
            case 1:
                if ( storyObjB.Count > 0 )
                {
                    return storyObjB;
                }
                else
                {
                    return null;
                }
            case 2:
                if ( storyObjC.Count > 0 )
                {
                    return storyObjC;
                }
                else
                {
                    return null;
                }
            case 3:
                return null;
            default:
                return null;
        }
    }

    void OnTutorial ( LogicArg arg )
    {
        Init ( );
        M_Event.FireLogicEvent ( LogicEvents.EnterStory, new LogicArg ( this ) );
    }

    void OnCharacters ( LogicArg arg )
    {
        Init ( );
        M_Event.FireLogicEvent ( LogicEvents.EnterStory, new LogicArg ( this ) );
    }

    void Init ( )
    {
        instance = this;
        // set stories to false and level objects to true
        for ( int i = 0; i < storyObjA.Count; i++ )
        {
            storyObjA [ i ].SetActive ( false );
        }
        for ( int i = 0; i < storyObjB.Count; i++ )
        {
            storyObjB [ i ].SetActive ( false );
        }
        for ( int i = 0; i < storyObjC.Count; i++ )
        {
            storyObjC [ i ].SetActive ( false );
        }

        videoCam.gameObject.SetActive ( false );
    }
}
