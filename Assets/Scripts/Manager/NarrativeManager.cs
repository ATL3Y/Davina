using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NarrativeManager : MBehavior {

	public NarrativeManager() { s_Instance = this; }
	public static NarrativeManager Instance { get { return s_Instance; } }
	private static NarrativeManager s_Instance;

	List<string> loadedScene = new List<string>();

	[System.Serializable]
	public class NarrativeLoadSceneEvent
	{
		public LogicEvents logicEvent;
		public string loadScene;
		public bool refreshScene = true;
	}
	[SerializeField] List<NarrativeLoadSceneEvent> narrativeEvents;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

		for (int i = 0; i < M_Event.logicEvents.Length; ++i) {
			M_Event.logicEvents [i] += OnLogicEvent;
		}
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		for (int i = 0; i < M_Event.logicEvents.Length; ++i) {
			M_Event.logicEvents [i] -= OnLogicEvent;
		}

	}

	void OnLogicEvent( LogicArg arg )
	{
		foreach (NarrativeLoadSceneEvent e in narrativeEvents) {
			if (e.logicEvent == arg.type) {
				DoEvent (arg, e);
			}
		}
	}


	void DoEvent( LogicArg arg,  NarrativeLoadSceneEvent e )
	{
		//Debug.Log ("Load Scene " + e.loadScene);
		SceneManager.LoadSceneAsync (e.loadScene , LoadSceneMode.Additive);

		if (e.refreshScene) {
			foreach( string scene in loadedScene )
			{
				SceneManager.UnloadScene (scene);
			}
			loadedScene.Clear ();
		}

		loadedScene.Add (e.loadScene);
	}

	IEnumerator UnloadSceneDelay( float time , string addScene)
	{
		yield return new WaitForSeconds (time);
		foreach( string scene in loadedScene )
		{
			SceneManager.UnloadScene (scene);
		}
		loadedScene.Clear ();
		loadedScene.Add (addScene);
	}
}
