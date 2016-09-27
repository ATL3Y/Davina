using UnityEngine;
using System.Collections;

public class Global {

	static public Vector2 Vector2Infinity = new Vector2(Mathf.Infinity,Mathf.Infinity);
	static public Vector3 Vector3Infinity = new Vector3(99999f , 99999f , 99999f );

	static public int StandableMask = LayerMask.GetMask("Floor" );

	static public Transform world{
		get{
			return GameObject.Find ("DWorld").transform;
		}
	}


	static public string EVENT_LOGIC_TRANSPORTTO_MOBJECT = "MOBJECT";
	static public string EVENT_LOGIC_SELECT_COBJECT = "S_COBJECT";
	static public string EVENT_LOGIC_MATCH_COBJECT = "M_COBJECT";
	static public string EVENT_LOGIC_UNSELECT_COBJECT = "T_COBJECT";
	static public string EVENT_LOGIC_ENTERINNERWORLD_CLIP = "BGM";
	static public string EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER = "Character";
	static public string EVENT_LOGIC_EXITINNERWORLD_MCHARACTER = "Character";
}
