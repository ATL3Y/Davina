using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class MetricManagerScript : MonoBehaviour {

	public static MetricManagerScript instance;

	[SerializeField]
	private List<string> _matchObjects = new List<string> ();

	string createText = "";

	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}


	void Start () {}
	void Update () {}
	
	//When the game quits we'll actually write the file.
	void OnApplicationQuit(){
		GenerateMetricsString ();

		/* can't get this to work
		string time = System.DateTime.UtcNow.ToString ();
		string dateTime = System.DateTime.Now.ToString (); //Get the time to tack on to the file name
		time = time.Replace ("/", "-"); //Replace slashes with dashes, because Unity thinks they are directories..
		*/

		//string reportFile = "GameName_Metrics_" + time + ".txt"; 
		string day = System.DateTime.Now.Day.ToString();
		string month = System.DateTime.Now.Month.ToString();
		string year = System.DateTime.Now.Year.ToString();

		string reportFile = "GameName_Metrics_"+ month + "-" + day + "-" + year + ".txt";
		File.WriteAllText (reportFile, createText);
		//In Editor, this will show up in the project folder root (with Library, Assets, etc.)
		//In Standalone, this will show up in the same directory as your executable
	}

	void GenerateMetricsString(){
		foreach (string x in _matchObjects) {
			createText += x + '\n';
		}	
	}

	public void AddToMatchList(string matchObject)
	{
		_matchObjects.Add (matchObject);	
	}

}
