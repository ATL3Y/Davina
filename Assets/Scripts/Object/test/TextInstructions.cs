using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TextInstructions : MonoBehaviour 
{
	public TextInstructions() { s_Instance = this; }
	public static TextInstructions Instance { get { return s_Instance; } }
	private static TextInstructions s_Instance;

	private List < GameObject > _alphabet = new List<GameObject> (); //these are all the letters of the alphabet to spell with
	private List < GameObject > _letters = new List<GameObject> (); //letters in the given phrase

	private int size;
	private float oldLength = 0f;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .025f;

	private List<string> instructions = new List<string>();

	private bool selected = false;
	private bool matched = false;

	public void Start () 
	{
		instructions.Add ("HEY POINT AT THE PERSON"); //0
		instructions.Add ("PRESS PAD"); //1
		instructions.Add ("REACH INSIDE CHEST AND TAP BRIGHT OBJECT"); //2
		instructions.Add ("TAP THE BRIGHT OBJECT"); //3
		instructions.Add ("PULL TRIGGER"); //4
		instructions.Add ("POINT AT THE NEXT PERSON"); //5

		instructions.Add ("THAT IS YOU"); //6
		instructions.Add ("THATS YOU AND YOU HAVE A HOLE IN YOUR HEART TO FILL"); //7
		instructions.Add ("FILL IT"); //8

		instructions.Add ("KEEP GOING LOOK DOWN"); //9

		origPos = transform.localPosition;
		origRot = transform.localRotation;
		origScale = transform.localScale;

		tracking /= transform.localScale.x;

		string levelPath = "lettersGOsmall";
		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if (alphabet == null || alphabet.Length == 0) 
		{
			//print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add (l); //add letter to the list // note: could just use the array...
		}

		MakeTextGO (instructions [0]); // point at the person
			
	}

	public void Update () 
	{

	}

	public void OnEnable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] += OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] += OnTransport;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] += OnOutofFocus;
		M_Event.logicEvents [(int)LogicEvents.MatchObject] += OnMatchObject;
	}

	public void OnDisable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] -= OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] -= OnTransport;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] -= OnOutofFocus;
		M_Event.logicEvents [(int)LogicEvents.MatchObject] -= OnMatchObject;
	}
	PasserBy focusPasserby;	
	public void OnFocusNew( InputArg arg ){

		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		CollectableObj cobj = arg.focusObject.GetComponent<CollectableObj> ();
		if (SelectObjectManager.Instance.SelectObj != null) {
			selected = true;
		}
		if (cobj != null && cobj.matched) {
			MakeTextGO (instructions [4]); // pull trigger
		} else if (cobj != null ) {
			MakeTextGO (instructions [4]); // pull trigger

		} else if (p != null && p != LogicManager.Instance.StayPasserBy) { 
			focusPasserby = p;
			MakeTextGO (instructions [1]); // press pad
		
		} 
	}

	public void OnOutofFocus( InputArg arg )
	{
		/*
		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		if ( focusPasserby == p) {
			MakeTextGO (instructions [0]);
			focusPasserby = null;
		} 
		*/

		//MakeTextGO (instructions [4]);

		if (transform.position.z > -30f && transform.position.z < -12f && !matched && !selected) {
			MakeTextGO (instructions [2]); // reach in chest 

			//StartCoroutine (Delay (3f));
			//MakeTextGO (instructions [3]); // tap bright obj

			//StartCoroutine (Delay (3f));
		}else if (transform.position.z > -12f && transform.position.z < 5f && !matched) {
			//MakeTextGO (instructions [6]); // that's you

			//StartCoroutine (Delay (3f));
			MakeTextGO (instructions [7]);

			//StartCoroutine (Delay (3f));
			//MakeTextGO (instructions [8]);
			//StartCoroutine (Delay (3f));
		}
	}

	public void OnTransport(InputArg arg){
		//print ("in on transport in text");

		if (transform.position.y < 10f) {
			//have gone to lower level
			Clear();
			transform.parent.GetComponent<Disable> ().DisableClidren ();
		}
			
		if (InputManager.Instance.FocusedObject != null && InputManager.Instance.FocusedObject is PasserBy) {
			MakeTextGO (instructions [2]); // reach in chest 
			//Pause(4f);
			//StartCoroutine (Delay (3f));
			//MakeTextGO (instructions [3]); // tap bright obj
		}
	}

	public void OnMatchObject(LogicArg arg ){
		matched = true;
		MakeTextGO (instructions[9]);
	}

	public void MakeTextGO ( string text ) //could have size, shader... //assuming all uppercase 
	{
		Clear ();
		//Pause (.5f);
		//StartCoroutine (Delay (.5f));

		InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);

		//center text
		/*
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * length / 2 * transform.localScale.x;
		*/

		for (int i=0; i < text.Length; i++) 
		{
			int count = 1; 
			if (text [i] == ' ') 
			{
				count *= 2; //make a space 
				continue; //jump to the next letter 
			}

			for (int j = 0; j < _alphabet.Count; j++) 
			{
				if (text [i].ToString() == _alphabet [j].name) 
				{ 
					GameObject newLetter = Instantiate (_alphabet [j]);
					newLetter.transform.SetParent (transform);
					newLetter.transform.localPosition = new Vector3 (-tracking * count * i, 0f, 0f);
					//newLetter.transform.localRotation = transform.rotation;
					newLetter.transform.localRotation = Quaternion.identity;
					_letters.Add (newLetter);
					count = 1;
				} 
			}
		}
	}

	public void SwapCharGO ( char c, int item ) //could have size, shader... //assuming all uppercase 
	{
		GameObject newLetter = new GameObject ();

		//assuming no match will yield an empty space
		for (int j = 0; j < _alphabet.Count; j++) 
		{
			if (c.ToString() == _alphabet [j].name) 
			{ 
				newLetter = Instantiate (_alphabet [j]);
			} 
		}

		newLetter.transform.position = _letters[item].transform.position;
		newLetter.transform.parent = transform;

		GameObject.Destroy (_letters [item]); //need?
		_letters [item] = newLetter;
	}

	public void Clear ()
	{
		foreach (Transform child in transform) 
		{
			GameObject.Destroy(child.gameObject);
		}

		_letters.Clear ();
		//could have light fade down and back 

		transform.localPosition = origPos; //Vector3.zero; 
		transform.localRotation = origRot; //Quaternion.identity; 
		transform.localScale = origScale; //new Vector3(1f, 1f, 1f); 

	}

	public void Pause( float time){

		while (time > 0f) {
			time -= Time.deltaTime;
		}
	}

	IEnumerator Delay( float delay )
	{
		yield return new WaitForSeconds (delay);
	}
}