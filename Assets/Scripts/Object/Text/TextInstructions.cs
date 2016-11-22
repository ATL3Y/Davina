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
	//private float oldLength = 0f;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .025f;

	private List<string> instructions = new List<string>();

	private float lineLengthLimit = 15f;
	private float lineHeightLimit = .05f;
	private int numberOfLines = 1;

	private float timeLeft=0f;
	private int currentInstruction = 0;
	//private bool callOnce = false;

	private bool characters = false;

	public void Start () 
	{
		instructions.Add (" "); //0
		//instructions.Add ("MAKE A CHOICE"); //1
		instructions.Add ("POINT AND HOLD TRIGGER"); //2
		//instructions.Add ("CARRY ITEM TO OUTLINE"); //3
		//instructions.Add ("RELEASE TRIGGER"); //4
		instructions.Add ("POINT AT GLOW"); //5
		instructions.Add ("PULL TRIGGER"); //6
		//instructions.Add ("CHOOSE"); //7
		instructions.Add ("POINT DOWN"); //8

		origPos = transform.localPosition;
		origRot = transform.localRotation;
		origScale = new Vector3 (1f, 1f, 1f);

		tracking /= transform.localScale.x;

		string levelPath = "lettersGOsmall";
		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if (alphabet == null || alphabet.Length == 0) {
			//print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add (l); //add letter to the list // note: could just use the array...
		}
		//MakeLines (instructions [0]); // LOOK AROUND AND STAND UP
		Pause(3f);
	}

	public void Update () 
	{
		timeLeft -= Time.deltaTime;

		if (timeLeft < 0.0f) {
			MakeLines (instructions [0]);
		}
	}

	public void OnEnable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] += OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] += OnTransport;
		M_Event.logicEvents [(int)LogicEvents.Heard] += OnHeard;
		M_Event.logicEvents [(int)LogicEvents.EnterStory] += OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] += OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
	}

	public void OnDisable(){
		M_Event.inputEvents [(int)MInputType.FocusNewObject] -= OnFocusNew;
		M_Event.inputEvents [(int)MInputType.Transport] -= OnTransport;
		M_Event.logicEvents [(int)LogicEvents.Heard] -= OnHeard;
		M_Event.logicEvents [(int)LogicEvents.EnterStory] -= OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] -= OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
	}

	void OnHeard(LogicArg arg)
	{
		MakeLines (instructions [2]); //POINT AND HOLD TRIGGER
	}

	void OnEnterStory(LogicArg arg)
	{
		MakeLines (instructions [2]); //POINT AND HOLD TRIGGER
	}

	void OnEnterStoryTutorial(LogicArg arg)
	{
		MakeLines (instructions [5]); //POINT AT GLOW
	}

	void OnCharacters(LogicArg arg)
	{
		MakeLines (instructions [8]); //POINT DOWN
	}
		
	NiceTeleporter teleportTo;	
	public void OnFocusNew( InputArg arg )
	{
		NiceTeleporter t = arg.focusObject.GetComponent<NiceTeleporter> ();

		if (t != null && t != LogicManager.Instance.StayTeleporter) { 
			teleportTo = t;
			MakeLines (instructions [6]); // PULL TRIGGER
		} 
	}

	public void OnOutofFocus( InputArg arg )
	{

	}

	public void OnTransport(InputArg arg)
	{
		/*
		if (teleportTo.transform.root.gameObject.name == "MotherMecanim") 
		{
			MakeLines (instructions [2]); //POINT AND HOLD TRIGGER
		} 
		else if (teleportTo.transform.root.gameObject.name == "FloatingDavina_v3") 
		{
			MakeLines (instructions [5]); //POINT AT GLOW
		}
		*/
	}


	public void MakeLines (string text){

		Clear ();
		//InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);

		List<string> lines = new List<string>();

		//place all the words into an array
		string[] words = text.Split (' ', '\t', System.Environment.NewLine.ToCharArray () [0]);
		//intantiate a line
		System.Text.StringBuilder line = new System.Text.StringBuilder ();

		for (int i = 0; i < words.Length; i++) {
			if (line.Length + words [i].Length + 1 > lineLengthLimit)
			{
				//add full line to the list of lines
				lines.Add (line.ToString ());
				//clear the line
				line.Remove (0, line.Length);
			}
			//add the word that put the last line over the limit to the new line
			line.Append (words[ i ] + " " );
		}
		// if there is a word left, add the final word
		if (line.Length > 0) {
			lines.Add (line.ToString ());
		}
			
		numberOfLines = lines.Count;
		//print (numberOfLines);
		for (int i = 0; i < lines.Count; i++) {
			MakeTextGO (lines [i], i);
			//print ("new line");
		}
	}

	public void MakeTextGO ( string text, int height ) //could have size, shader... //assuming all uppercase 
	{
		//center text using size of string
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * length / 2 * transform.lossyScale.x;

		float lineHeight =  lineHeightLimit * (float) (numberOfLines - height - 1);

		for (int i=0; i < text.Length; i++) {
			int count = 1; 
			if (text [i] == ' ') {
				count *= 2; //make a space 
				continue; //jump to the next letter 
			}

			for (int j = 0; j < _alphabet.Count; j++) {
				if (text [i].ToString() == _alphabet [j].name) { 
					GameObject newLetter = Instantiate (_alphabet [j]);
					newLetter.transform.SetParent (transform);
					newLetter.transform.localPosition = new Vector3 (-tracking * count * i, lineHeight, 0f);
					newLetter.transform.localRotation = Quaternion.identity;
					_letters.Add (newLetter);
					count = 1;
				} 
			}
		}
		//leave instruction for a time
		Pause (4f);
	}

	public void Clear ()
	{
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}

		_letters.Clear ();
		transform.localPosition = origPos; //Vector3.zero; 
		transform.localRotation = origRot; //Quaternion.identity; 
		transform.localScale = origScale; //new Vector3(1f, 1f, 1f); 
	}

	void Pause( float delay){
		timeLeft = delay;
	}
}