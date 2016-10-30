using UnityEngine;
using System.Collections;

public class Global 
{

	static public Vector2 Vector2Infinity = new Vector2(Mathf.Infinity, Mathf.Infinity);
	static public Vector3 Vector3Infinity = new Vector3(99999f, 99999f, 99999f);

	static public int StandableMask = LayerMask.GetMask("Floor");

	static public Transform world
	{
		get{
			return GameObject.Find ("DWorld").transform;
		}
	}
		
	static public string EVENT_LOGIC_TRANSPORTTO_MOBJECT = "EVENT_LOGIC_TRANSPORTTO_MOBJECT";
	static public string EVENT_LOGIC_SELECT_COBJECT = "EVENT_LOGIC_SELECT_COBJECT";
	static public string EVENT_LOGIC_MATCH_COBJECT = "EVENT_LOGIC_MATCH_COBJECT";
	static public string EVENT_LOGIC_UNSELECT_COBJECT = "T_COBJECT";
	static public string EVENT_LOGIC_ENTERINNERWORLD_CLIP = "BGM";
	static public string EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER = "Character";
	static public string EVENT_LOGIC_EXITINNERWORLD_MCHARACTER = "Character";
	//static public string EVENT_LOGIC_ENTERSTORYOBJ = "EVENT_LOGIC_ENTERSTORYOBJ"; //need?
	//static public string EVENT_LOGIC_EXITSTORYOBJ = "EVENT_LOGIC_EXITSTORYOBJ";

}
