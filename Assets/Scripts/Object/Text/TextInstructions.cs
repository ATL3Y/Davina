using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class TextInstructions : MonoBehaviour 
{
	public TextInstructions() { s_Instance = this; }
	public static TextInstructions Instance { get { return s_Instance; } }
	private static TextInstructions s_Instance;

	private List < GameObject > _alphabet = new List<GameObject> (); //these are all the letters of the alphabet to spell with
	private List < GameObject > _letters = new List<GameObject> (); //letters in the given phrase

	private int size;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .025f;

	private List<string> instructions = new List<string>();

	private float lineLengthLimit = 15f;
	private float lineHeightLimit = .05f;
	private int numberOfLines = 1;

	private float timeLeft = 0f;
	private int currentInstruction = 0;

	private bool end = false;

	public void Start() 
	{
		instructions.Add ("xxx"); //0 -- default 
		instructions.Add ("POINT AND CLICK TRIGGER TO TELEPORT"); //1 -- at scene opening
		instructions.Add ("TOUCH THE LENS AND HOLD TRIGGER TO HOLD IT"); //2 -- teleport (tutorial), teleport to mom (characters)
		instructions.Add ("PLACE THE LENS IN THE FRAME WITH YOUUR CHOSEN SIDE FACING DAVINA"); //3 -- picked up collectable (tutorial), teleport to davina (characters)
        instructions.Add ("RELEASE THE TRIGGER TO RELEASE THE LENS"); //4 -- add function called from collectable in release range
        instructions.Add ("USE ONE HAND TO HOLD THE LENS AND THE OTHER HAND TO TELEPORT"); //5 -- picked up collectable (characters)

        origPos = transform.localPosition;
		origRot = transform.localRotation;
		origScale = new Vector3 (1f, 1f, 1f);

		tracking /= transform.localScale.x;

		string levelPath = "lettersGOsmall";
		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if(alphabet == null || alphabet.Length == 0)
        {
			//print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add (l); //add letter to the list
		}
	}

	public void Update() 
	{
		timeLeft -= Time.deltaTime;

		if (timeLeft < 0.0f)
        {
			MakeLines (instructions[0]); //" "
		}
	}

	public void OnEnable()
    {
		M_Event.logicEvents [(int)LogicEvents.Tutorial] += OnTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.TransportEnd] += OnTransportEnd;
    }

	public void OnDisable()
    {
		M_Event.logicEvents [(int)LogicEvents.Tutorial] -= OnTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.TransportEnd] -= OnTransportEnd;
    }

    public void OnTransportEnd(LogicArg arg)
    {
        Interactable teleportedTo;

        if ((Interactable)arg.GetMessage(Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT) != null)
        {
            teleportedTo = (Interactable)arg.GetMessage(Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
            if (teleportedTo.name.Contains("Davina"))
            {
                MakeLines(instructions[3]); //"PLACE THE LENS IN THE ROUND FRAME WITH THE SIDE YOU CHOOSE FACING DAVINA"
            }
            else if (teleportedTo.name.Contains("Mom"))
            {
                MakeLines(instructions[2]); //"TOUCH THE LENS AND HOLD TRIGGER TO HOLD IT"
            }
        }
    }

    // called from NiceCollectable
    public void OnReleaseRange()
    {
        MakeLines(instructions[4]); //"RELEASE THE TRIGGER TO RELEASE THE LENS"
    }

    void OnTutorial(LogicArg arg)
	{
		MakeLines(instructions[1]); //"POINT AND CLICK TRIGGER TO TELEPORT"
    }

	void OnCharacters(LogicArg arg)
	{
        MakeLines(instructions[1]); //"POINT AND CLICK TRIGGER TO TELEPORT"
    }

	public void PickedUpCollectable()
    {
        Debug.Log(instructions.Count);
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            MakeLines(instructions[5]); //"USE ONE HAND TO HOLD THE LENS AND THE OTHER HAND TO TELEPORT"
        }
        else
        {
            MakeLines(instructions[3]); //"PLACE THE LENS IN THE ROUND FRAME WITH THE SIDE YOU CHOOSE FACING DAVINA"
        }
	}

    public void MakeLines(string text)
    {
		Clear ();
		//InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);

		if (end)
			return;
		
		List<string> lines = new List<string>();

		//place all the words into an array
		string[] words = text.Split(' ', '\t', System.Environment.NewLine.ToCharArray()[0]);
		//intantiate a line
		System.Text.StringBuilder line = new System.Text.StringBuilder();

		for (int i = 0; i < words.Length; i++)
        {
			if (line.Length + words [i].Length + 1 > lineLengthLimit)
			{
				//add full line to the list of lines
				lines.Add (line.ToString());
				//clear the line
				line.Remove (0, line.Length);
			}
			//add the word that put the last line over the limit to the new line
			line.Append (words[ i ] + " " );
		}
		// if there is a word left, add the final word
		if (line.Length > 0)
        {
			lines.Add (line.ToString());
		}
			
		numberOfLines = lines.Count;
		//print (numberOfLines);
		for (int i = 0; i < lines.Count; i++)
        {
			MakeTextGO (lines [i], i);
			//print ("new line");
		}
	}

	public void MakeTextGO(string text, int height) //could have size, shader... //assuming all uppercase 
	{
		//center text using size of string and number of lines
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * (length / (2 * numberOfLines)) * transform.lossyScale.x;

		float lineHeight =  lineHeightLimit * (float) (numberOfLines - height - 1);

		for(int i=0; i < text.Length; i++)
        {
			int count = 1; 
			if(text [i] == ' ')
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
					newLetter.transform.localPosition = new Vector3(-tracking * count * i, lineHeight, 0f);
					newLetter.transform.localRotation = Quaternion.identity;
					_letters.Add(newLetter);
					count = 1;
				} 
			}
		}
		//leave instruction for a time
		Pause (7f);
	}

	public void Clear()
	{
		foreach(Transform child in transform)
        {
			GameObject.Destroy(child.gameObject);
		}

		_letters.Clear();
		transform.localPosition = origPos; //Vector3.zero; 
		transform.localRotation = origRot; //Quaternion.identity; 
		transform.localScale = origScale; //new Vector3(1f, 1f, 1f); 
	}

	void Pause(float delay)
    {
		timeLeft = delay;
	}
}