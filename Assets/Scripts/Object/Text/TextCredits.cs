using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TextCredits : MonoBehaviour 
{
	public TextCredits() { s_Instance = this; }
	public static TextCredits Instance { get { return s_Instance; } }
	private static TextCredits s_Instance;

	private List<GameObject> _alphabet = new List<GameObject>(); //these are all the letters of the alphabet to spell with
	private List<GameObject> _letters = new List<GameObject>(); //letters in the given phrase

	private int size;
	//private float oldLength = 0f;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .5f;

	private List<string> instructions = new List<string>();

	private float lineLengthLimit = 20f;
	private float lineHeightLimit = 1.2f;
	private int numberOfLines = 1;

	private float timeLeft=0f;
	private int currentInstruction = 0;

	public void Awake()
    {
        instructions.Add("");
        instructions.Add("IN HONOR OF          THE STRUGGLE FOR     SELF AND LOVE");
		instructions.Add("                          DAVINA");
		instructions.Add("MUSIC AND            SOUND DESIGN         RICKIE LEE KROELL"); 
		instructions.Add("STORY                ISOBEL SHASHA"); 
		instructions.Add("PROGRAMMER           ATWOOD DENG");
        instructions.Add("VOICE ACTOR MOTHER   YIWEN DAI");
        instructions.Add("VOICE ACTOR DAVINA   JUNG HO SOHN");
        instructions.Add("GAME DESIGN          PROGRAMMER           ATLEY LOUGHRIDGE"); 
		instructions.Add("SPECIAL THANKS       RICHARD LEMARCHAND   RUSSELL HONOR"); 
		instructions.Add("USC                  INTERACTIVE MEDIA    AND GAMES");
        instructions.Add("IN HONOR OF          THE STRUGGLE FOR     SELF AND LOVE");
    }

	public void Start() 
	{
		origPos = transform.localPosition;
		origRot = transform.localRotation;
		origScale = new Vector3 (1f, 1f, 1f);

		tracking /= transform.localScale.x;

		string levelPath = "lettersGO";
		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if (alphabet == null || alphabet.Length == 0)
        {
			//print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add(l); //add letter to the list // note: could just use the array...
		}
	}

	public void Update()
    {

		if(timeLeft > 0f)
        {
			timeLeft -= Time.deltaTime;
		}
        else
        {
			if(instructions [currentInstruction] != null && currentInstruction < instructions.Count)
            {
				MakeLines(instructions[currentInstruction]);
				timeLeft = 3f;
				currentInstruction++;
			}
		}
	}

	public void OnEnable()
    {
		
	}

	public void OnDisable()
    {

	}

	public void MakeLines(string text)
    {

		if (timeLeft > 0f)
        {
			return;
		}

		Clear();
		//InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);

		List<string> lines = new List<string>();

		//place all the words into an array
		string[] words = text.Split (' ', '\t', System.Environment.NewLine.ToCharArray()[0]);
		//intantiate a line
		System.Text.StringBuilder line = new System.Text.StringBuilder();

		for(int i = 0; i < words.Length; i++)
        {
			if(line.Length + words [i].Length + 1 > lineLengthLimit)
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
		if(line.Length > 0)
        {
			lines.Add (line.ToString ());
		}

		numberOfLines = lines.Count;
		//print (numberOfLines);
		for (int i = 0; i < lines.Count; i++)
        {
			MakeTextGO(lines [i], i);
			//print ("new line");
		}
	}

	public void MakeTextGO(string text, int height) //could have size, shader... //assuming all uppercase 
	{
		/*
		//center text using size of string
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * length / 2 * transform.lossyScale.x;
		*/

		float lineHeight =  lineHeightLimit * (float) (numberOfLines - height - 1);

		for(int i=0; i < text.Length; i++)
        {
			int count = 1; 
			if (text [i] == ' ')
            {
				count *= 2; //make a space 
				continue; //jump to the next letter 
			}

			for(int j = 0; j < _alphabet.Count; j++)
            {
				if (text [i].ToString() == _alphabet [j].name)
                { 
					GameObject newLetter = Instantiate(_alphabet [j]);
					newLetter.transform.SetParent (transform);
					newLetter.transform.localPosition = new Vector3(-tracking * count * i, lineHeight, 0f);
					newLetter.transform.localRotation = Quaternion.identity;
					_letters.Add (newLetter);
					count = 1;
				} 
			}
		}
		//leave instruction for at least 1 second
		Pause(4f);
	}

	public void Clear()
	{
		foreach (Transform child in transform)
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