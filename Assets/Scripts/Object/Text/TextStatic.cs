using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TextStatic : MonoBehaviour 
{
	//make sure this doesn't run after death.. ?
	private List < GameObject > _alphabet = new List<GameObject>(); //these are all the letters of the alphabet to spell with
	private List < GameObject > _letters = new List<GameObject>(); //letters in the given phrase

	private int size;
	private float oldLength = 0f;
	private Vector3 origPos;
	private Quaternion origRot;
	private Vector3 origScale;
	private float tracking = .5f;

	[SerializeField] string firstPhrase;

	public void Start() 
	{
		origPos = transform.position;
		origRot = transform.rotation;
		origScale = transform.localScale;

		tracking /= transform.localScale.x;

        string levelPath;
        if (name.Contains("Title"))
        {
            levelPath = "lettersGOlarge";
            tracking = 2.5f;
        }else
        {
            levelPath = "lettersGO";
            tracking = .5f;
        }
        
		Object[] alphabet = Resources.LoadAll(levelPath, typeof(GameObject));

		if(alphabet == null || alphabet.Length == 0) 
		{
			//print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			l.layer = 20; //add to Bridge layer so it's visible by near camera
			_alphabet.Add(l); //add letter to the list // note: could just use the array...
		}

		MakeTextGO(firstPhrase);
	}
		
	public void MakeTextGO(string text) //could have size, shader... //assuming all uppercase 
	{
		Clear ();

		//center text
		size = text.Length;
		float length = size * tracking;
		transform.position += transform.right * length / 2 * transform.localScale.x;

		for(int i=0; i < text.Length; i++) 
		{
			int count = 1; 
			if(text [i] == ' ') 
			{
				count *= 2; //make a space 
				continue; //jump to the next letter 
			}

			for(int j = 0; j < _alphabet.Count; j++) 
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

	public void SwapCharGO(char c, int item) //could have size, shader... //assuming all uppercase 
	{
		GameObject newLetter = new GameObject();

		//assuming no match will yield an empty space
		for(int j = 0; j < _alphabet.Count; j++) 
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

	public void Clear()
	{
		foreach (Transform child in transform) 
		{
			GameObject.Destroy(child.gameObject);
		}

		_letters.Clear();
		//could have light fade down and back 

		transform.position = origPos;
		transform.rotation = origRot;
		transform.localScale = origScale;
	}
}
