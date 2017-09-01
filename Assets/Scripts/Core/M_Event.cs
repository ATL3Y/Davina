using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// enum of the different input action
/// </summary>
public enum MInputType
{
	None,
	SelectObject,
	Transport, 
	FocusNewObject,
	OutOfFocusObject,
}

/// <summary>
/// Logic events.
/// </summary>
public enum LogicEvents
{
	None=0,
	TransportStart=1,
	TransportEnd=2,
	SelectObject=3,
	MatchObject=4,
	UnselectObject=5,
	EnterInnerWorld=6,
	ExitInnerWorld=7,
	EnterCharacterRange=8,
	ExitCharacterRange=9,

	IntoWork=10,
	CameraAttachPointChange=11,

	GetObject=12,

	RaiseFallingCharacter=13,
	LowerFallingCharacter=14,

	EnterStory=15,
	ExitStory=16,

	Characters=17,
	End=18, // maybe these should be separate scenes and scene enter functions
	Credits=19,

    Finale=22,
	Heard=23,

	//OpenShotOneEnter = 101,
	//OpenShotTwoEnter = 102,
	//OpenShotThreeEnter = 103,
	Tutorial = 104,

	//CharacterSceneEnter = 201,

}

public class M_Event : MonoBehaviour {

	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);

	public static event EventHandler StartApp;
	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }


	public delegate void LogicHandler( LogicArg arg );

//	public static LogicHandler[] logicEvents = new LogicHandler[System.Enum.GetNames (typeof (LogicEvents)).Length];
	public static LogicHandler[] logicEvents = new LogicHandler[999];
	public static void FireLogicEvent(LogicArg arg)
	{
		if (arg.type != LogicEvents.None)
        {
			FireLogicEvent (arg.type, arg);
		}
	}

	public static void FireLogicEvent(LogicEvents type, LogicArg arg )
	{
		if ( logicEvents[(int)type] != null )
		{
			arg.type = type;
			logicEvents [(int)type] ( arg );
		}

	}

	/// <summary>
	/// Input events
	/// </summary>
	public delegate void InputHandler( InputArg arg );

	public static InputHandler[] inputEvents = new InputHandler[System.Enum.GetNames (typeof (MInputType)).Length];
	public static void FireInput( InputArg arg )
	{
		if (arg.type != MInputType.None) {
			FireInput (arg.type, arg);
		}
	}
	public static void FireInput( MInputType type , InputArg arg )
	{
		if ( inputEvents[(int)type] != null )
		{
			arg.type = type;
			inputEvents [(int)type] ( arg );
		}
	
	}

}

/// <summary>
/// Basic argument.
/// save the sender of the event
/// </summary>
public class BasicArg : EventArgs
{
	public BasicArg(object _this){m_sender = _this;}
	object m_sender;
	public object sender{get{return m_sender;}}
}

/// <summary>
/// Message argument.
/// based on BasicArg
/// add message dictionary for allowing to add expandable parameters
/// </summary>
public class MsgArg : BasicArg
{
	public MsgArg(object _this):base(_this){}
	Dictionary<string,object> m_dict;
	Dictionary<string,object> dict
	{
		get {
			if ( m_dict == null )
				m_dict = new Dictionary<string, object>();
			return m_dict;
		}
	}
	public void AddMessage(string key, object val)
	{
		dict.Add(key, val);
	}
	public object GetMessage(string key)
	{
		object res;
		dict.TryGetValue(key , out res);
		return res;
	}
	public bool ContainMessage(string key)
	{
		return dict.ContainsKey(key);
	}
}

public enum ClickType
{
	Mouse, 
	LeftController,
	RightController,
}

public class InputArg : BasicArg
{
	public InputArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public MInputType type;
	public ClickType clickType = ClickType.Mouse;

	public GameObject focusObject;
}

/// <summary>
/// Logic argument.
/// based on MsgArg
/// save the type of the event
/// </summary>
public class LogicArg : MsgArg
{
	public LogicArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public LogicEvents type;
}