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

	public bool VREnable = true;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;
	[SerializeField] Transform PCHand;
	[SerializeField] Transform playerHead;
    [SerializeField] Transform playerPerson;
    [SerializeField] GameObject Rain;
	[SerializeField] TextStatic text;

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
    private GameObject[] m_sceneRoots;

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

        int numScenes = SceneManager.sceneCountInBuildSettings;
        m_async = new AsyncOperation[numScenes-1];
        for (int i = 0; i < numScenes-1; i++)
        {
            m_async[i] = SceneManager.LoadSceneAsync(i+1, LoadSceneMode.Additive);
            m_async[i].allowSceneActivation = false;
        }

        m_sceneRoots = new GameObject[numScenes-1];
    }

    public void SetSceneRoots(int buildIndex, GameObject sceneRoot)
    {
        m_sceneRoots[buildIndex-1] = sceneRoot;
        m_sceneRoots[buildIndex - 1].SetActive(false);
    }

    public void IterateState()
    {
        int index = (int)m_stateMachine.State;
        int indexMin = (int)m_stateMachine.State - 1;

        // disable current scene
        if (indexMin > -1)
        {
            m_async[indexMin].allowSceneActivation = false;
            m_sceneRoots[indexMin].SetActive(false);
        }

        // enable the next scene
        m_async[indexMin + 1].allowSceneActivation = true;
        m_sceneRoots[indexMin + 1].SetActive(true);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index + 1));
        
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
            IterateState();
        }

        m_stateMachine.Update();

        // Trigger Tutorial
        if (m_allScenesLoaded)
            return;

        m_allScenesLoaded = true;
        for(int i = 0; i < m_async.Length; i++)
        {
            if(m_async[i]==null || !m_async[i].isDone)
            {
                m_allScenesLoaded = false;
                break;
            }
        }

        if (m_allScenesLoaded)
            IterateState();
    }

    void OnNewAttachPoint(LogicArg arg)
    {
		CameraAttachPoint point = (CameraAttachPoint)arg.sender;
		if (point != null)
        {
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

	public void OnCharacters (LogicArg arg ){
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
