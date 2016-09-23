using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum MInputType
{
	None,
	SelectObject,
	Transport,
	FocusNewObject,
}

public enum LogicEvents
{
	None,
	TransportStart,
	TransportEnd,
	SelectSuccessful,
}

public class M_Event : MonoBehaviour {

	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);

	public static event EventHandler StartApp;
	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }


	public delegate void LogicHandler( LogicArg arg );

	public static LogicHandler[] logicEvents = new LogicHandler[System.Enum.GetNames (typeof (LogicEvents)).Length];
	public static void FireLogicEvent( LogicArg arg )
	{
		if (arg.type != LogicEvents.None) {
			FireLogicEvent (arg.type, arg);
		}
	}
	public static void FireLogicEvent( LogicEvents type , LogicArg arg )
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

public class BasicArg : EventArgs
{
	public BasicArg(object _this){m_sender = _this;}
	object m_sender;
	public object sender{get{return m_sender;}}
}

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


public class InputArg : BasicArg
{
	public InputArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public MInputType type;

	public GameObject focusObject;
}

public class LogicArg : MsgArg
{
	public LogicArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public LogicEvents type;
}