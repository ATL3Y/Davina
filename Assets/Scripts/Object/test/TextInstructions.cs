using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TextInstructions : MonoBehaviour 
{
	private List < GameObject > _alphabet = new List<GameObject> (); //these are all the letters of the alphabet to spell with
	private List < GameObject > _letters = new List<GameObject> (); //letters in the given phrase

	private int size;
	private float oldLength = 0f;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .03f;

	[SerializeField] List<string> instructions;

	public void Start () 
	{
		origPos = transform.localPosition;
		origRot = transform.localRotation;
		origScale = transform.localScale;

		tracking /= transform.localScale.x;

		string levelPath = "lettersGOsmall";
		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if (alphabet == null || alphabet.Length == 0) 
		{
			print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add (l); //add letter to the list // note: could just use the array...
		}

		MakeTextGO (instructions [0]);
			
	}

	public void Update () 
	{

	}

	public void OnEnable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] += OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] += OnTransport;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] += OnOutofFocus;
	}

	public void OnDisable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] -= OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] -= OnTransport;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] -= OnOutofFocus;
	}
	PasserBy focusPasserby;	
	public void OnFocusNew( InputArg arg ){
		print ("in on focus new in text");
		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		CollectableObj cobj = arg.focusObject.GetComponent<CollectableObj> ();
		if (cobj != null) {
			MakeTextGO (instructions [3]);
			print ("in on focus new in text c conditional");
		} else if (p != null && p != LogicManager.Instance.StayPasserBy) { //TODO: why can't i stop from focusing on current focus object?
			focusPasserby = p;
			MakeTextGO (instructions [1]);
			print ("in on focus new in text p conditional");
		} 
	}

	public void OnOutofFocus( InputArg arg )
	{
		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		if ( focusPasserby == p) {
			MakeTextGO (instructions [2]);
			focusPasserby = null;
		}
	}

	public void OnTransport(InputArg arg){
		print ("in on transport in text");

		if (transform.position.y < 10f) {
			//have gone to lower level
			Clear();
			transform.parent.GetComponent<Disable> ().DisableClidren ();
		}

		if (InputManager.Instance.FocusedObject != null && InputManager.Instance.FocusedObject is PasserBy) {
			MakeTextGO (instructions [2]);
			print ("in on transport in text transport conditional");
		}
	}

	public void MakeTextGO ( string text ) //could have size, shader... //assuming all uppercase 
	{
		Clear ();
		//Pause ();

		//center text
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * length / 2 * transform.localScale.x;

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

	public void Pause(){
		float time = 1.5f;

		while (time > 0f) {
			time -= Time.deltaTime;
		}
	}
}