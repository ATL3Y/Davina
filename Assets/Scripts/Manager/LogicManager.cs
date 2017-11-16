using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 1. set up the input manager according to the VREnable
/// 2. set up the state machine for different scenes
/// </summary>
public class LogicManager : MBehavior
{
	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

    [SerializeField]
    Transform nextStartPos;

    public bool VREnable = true;
    public bool VolumetricLights;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;
    public GameObject VRLeftHand;
    public GameObject VRRightHand;
    [SerializeField] Transform PCHand;
	[SerializeField] Transform playerHead;
    [SerializeField] Transform playerPerson;
    [SerializeField] GameObject Rain;
	[SerializeField] TextStatic text;

    public GameObject TutorialRoot { get; set; }

	private NiceTeleporter m_stayTeleporter;
	public NiceTeleporter StayTeleporter
	{
		get { return m_stayTeleporter; }
	}

	private MCharacter m_stayCharacter;
	public MCharacter StayCharacter
    {
		get { return m_stayCharacter; }
	}

    public GameObject GetCurrentSceneRoot ( )
    {
        if( SceneManager.GetActiveScene().buildIndex == 1 )
        {
            return TutorialRoot;
        }
        else if( SceneManager.GetActiveScene ( ).buildIndex == 2 )
        {
            return m_sceneRoots [ 0 ].gameObject;
        }

        return null;
    }

    public enum State
    {
		Init, // 0
		Tutorial, // 1
		CharacterScene, // 2
        End, // 3
	}

	private AStateMachine<State,LogicEvents> m_stateMachine = new AStateMachine<State, LogicEvents>();

    private bool m_allScenesLoaded = false;
    private AsyncOperation[] m_async;
    public GameObject[] m_sceneRoots;

    protected override void MAwake()
    {
		base.MAwake();
        //AudioListener.volume = 0f;
        if (VREnable)
        {
			gameObject.AddComponent<ViveInputManager>();
		}
        else
        {
			gameObject.AddComponent<PCInputManager>();
		}

		if (PC != null)
        {
			PC.SetActive (!VREnable);
		}

		if (VR != null)
        {
			VR.SetActive (VREnable);
		}
		//Rain.transform.SetParent ( VREnable ? VR.transform : PC.transform);
		//Rain.transform.localPosition = Vector3.up * 5f;
		DontDestroyOnLoad (gameObject);
		Cursor.visible = false;

        InitScenes();

        if (VolumetricLights)
        {
            gameObject.GetComponentInChildren<HxVolumetricCamera>().enabled = true;
        }
        else
        {
            gameObject.GetComponentInChildren<HxVolumetricCamera>().enabled = false;
        }
    }

    private void InitScenes()
    { 
        int numAsyncScenes = SceneManager.sceneCountInBuildSettings - 2; // Init and Tutorial aren't async
        m_async = new AsyncOperation[numAsyncScenes];

        // Start in the Tutorial
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);

        TransportManager.Instance.StationaryEffect( nextStartPos.position, false );

        // Load other scenes async
        for (int i = 0; i < numAsyncScenes; i++)
        {
            m_async[i] = SceneManager.LoadSceneAsync(i + 2, LoadSceneMode.Additive);
            m_async[i].allowSceneActivation = false; 
        }

        m_sceneRoots = new GameObject[numAsyncScenes];
    }

    public void SetSceneRoots(int buildIndex, GameObject sceneRoot)
    {
        m_sceneRoots[buildIndex - 2] = sceneRoot;
        m_sceneRoots[buildIndex - 2].SetActive(false);
    }

    public void IterateState()
    {
        // build indexes: init: 0, tutorial: 1, characters: 2, endWhite: 3, endBlack: 4
        // async/root indexes: characters: 0, endWhite: 1, endBlack: 2
        int index = (int)m_stateMachine.State;

        if (index == 0)
        {
            Debug.Log("IterateState() 0");
            // no need to disable the Init scene
            // enable the Tutorial 
            TutorialRoot.SetActive(true);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
        else if (index == 1)
        {
            Debug.Log("IterateState() 1");
            // disable the Tutorial 
            TutorialRoot.SetActive(false);
            // enable the first async scene
            m_async[0].allowSceneActivation = true;
            m_sceneRoots[0].SetActive(true);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
        }
        else if (index == 2 && Score.Instance.GetScore() >= 0) // white ending
        {
            Debug.Log("IterateState() " + index + " white end.  score = " + Score.Instance.GetScore());
            // disable the last async scene
            m_async[0].allowSceneActivation = false;
            m_sceneRoots[0].SetActive(false);

            // enable white ending
            m_async[1].allowSceneActivation = true;
            m_sceneRoots[1].SetActive(true);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(3));
        }
        else if (index == 2 && Score.Instance.GetScore() < 0) // black ending
        {
            Debug.Log("IterateState() " + index + " black end.  score = " + Score.Instance.GetScore());
            // disable character scene
            m_async[0].allowSceneActivation = false;
            m_sceneRoots[0].SetActive(false);

            // enable the black ending
            m_async[2].allowSceneActivation = true;
            m_sceneRoots[2].SetActive(true);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(4));
        }
        else if(index == 3 )
        {
            // Disable endings
            m_async [ 1 ].allowSceneActivation = false;
            m_sceneRoots [ 1 ].SetActive ( false );
            m_async [ 2 ].allowSceneActivation = false;
            m_sceneRoots [ 2 ].SetActive ( false );

            Debug.Log ( "Going back to the beginning." );
            TutorialRoot.SetActive ( true );
            SceneManager.SetActiveScene ( SceneManager.GetSceneByBuildIndex ( 1 ) );
            m_stateMachine.State = State.Init; // set to init because it will immediately iterate.
        }

        // iterate state 
        m_stateMachine.State++;
    }

    protected override void MStart()
    {
        InitStateMachine();
	}

    void InitStateMachine()
    {
        m_stateMachine.AddEnter(State.Tutorial, delegate () { M_Event.FireLogicEvent(LogicEvents.Tutorial, new LogicArg(this)); });
        m_stateMachine.AddEnter(State.CharacterScene, delegate () { M_Event.FireLogicEvent(LogicEvents.Characters, new LogicArg(this)); });
        m_stateMachine.AddEnter(State.End, delegate () { M_Event.FireLogicEvent(LogicEvents.End, new LogicArg(this)); });
        //m_stateMachine.BlindTimeStateChange (State.OpenShotOne, State.MotherScene, 6f);
        m_stateMachine.State = State.Init;
	}
		
	public Transform GetPlayerTransform()
    {
		return VREnable ? VR.transform : PC.transform;
	}

    public Transform GetPlayerPersonTransform()
    {
        return VREnable ? playerPerson : PC.transform;
    }

    public Transform GetPlayerHeadTransform()
    {
		return VREnable ? playerHead : PC.transform;
	}

	public Transform GetHandTransform(ClickType clickType)
    {
		switch (clickType) 
		{
		case ClickType.LeftController:
			return ViveInputController.Instance.leftController.transform;
		case ClickType.RightController:
			return ViveInputController.Instance.rightController.transform;
		default:
			return PCHand;
		}
	}
		
	protected override void MOnEnable()
    {
		base.MOnEnable();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] += OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] += OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] += OnExitInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.CameraAttachPointChange] += OnNewAttachPoint;
		
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] += OnCredits;

		for (int i = 0; i < M_Event.logicEvents.Length; ++i)
        {
			M_Event.logicEvents [i] += OnLogicEvent;
		}
	}

	protected override void MOnDisable()
    {
		base.MOnDisable();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] -= OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] -= OnExitInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.CameraAttachPointChange] -= OnNewAttachPoint;
		
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;
		
		for (int i = 0; i < M_Event.logicEvents.Length; ++i)
        {
			M_Event.logicEvents [i] -= OnLogicEvent;
		}
	}

    protected override void MUpdate()
    {
        base.MUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = new Vector3(9.46f, 0.17f, 2.48f);
            TransportManager.Instance.StationaryEffect ( pos, true );
        }

        m_stateMachine.Update();

        // Trigger Tutorial
        if (m_allScenesLoaded)
            return;

        m_allScenesLoaded = true;
        for(int i = 0; i < m_async.Length; i++)
        {
            if(m_async[i] == null || !m_async[i].isDone)
            {
                Debug.Log("br");
                m_allScenesLoaded = false;
                break;
            }
        }

        if (m_allScenesLoaded)
        {
            Debug.Log("calling IterateState()");
            IterateState();
        }    
    }

    void OnNewAttachPoint(LogicArg arg)
    {
		CameraAttachPoint point = (CameraAttachPoint)arg.sender;
		if (point != null)
        {
            Debug.Log ( "OnNewAttachPoint: " + point.transform.position + " local pos " + point.transform.localPosition + " in scene " + SceneManager.GetActiveScene().name );
            VR.transform.position = point.transform.position;
			VR.transform.rotation = point.transform.rotation;
		}
	}

	void OnLogicEvent(LogicArg arg)
    {
		m_stateMachine.OnEvent (arg.type);
	}

	void OnTransportToNewObject(LogicArg arg)
    {
		var obj = arg.GetMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
		if (obj is NiceTeleporter)
        {
			m_stayTeleporter = (NiceTeleporter)obj;
		}
	}

	void OnEnterInnerWorld(LogicArg arg)
    {
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER);
		if (character != null)
			m_stayCharacter = character;
	}

	void OnExitInnerWorld(LogicArg arg)
    {
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_EXITINNERWORLD_MCHARACTER);
		if (m_stayCharacter == character)
			m_stayCharacter = null;
	}

	void RaiseTheBody()
    {
		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("isUp", true);
		M_Event.FireLogicEvent (LogicEvents.RaiseFallingCharacter, arg);
	}

	void LowerTheBody()
    {
		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("down", true);
		M_Event.FireLogicEvent (LogicEvents.LowerFallingCharacter, arg);
	}

	public void OnCharacters (LogicArg arg )
    {
        // set  active scene
	}

	void OnEnd(LogicArg arg)
    {
        // set active scene
	}

	void OnCredits(LogicArg arg)
    {
		// set active scene
	}
}
