using UnityEngine;
using System.Collections;

/// <summary>
/// Basic behaviour for the project
/// </summary>
public class MBehavior : MonoBehaviour {

	void Awake()
	{
		MAwake ();
	}

	void Start()
	{
		MStart ();
	}

	/// <summary>
	/// TODO: add the game pause function
	/// </summary>
	void Update()
	{
		MUpdate ();
	}

	void OnEnable()
	{
		MOnEnable ();
	}

	void OnDisable()
	{
		MOnDisable ();
	}

	virtual protected void MAwake() {

	}

	// Use this for initialization
	virtual protected void MStart () {
		
	}
	
	// Update is called once per frame
	virtual protected void MUpdate () {
	
	}

	virtual protected void MOnEnable() {
	}

	virtual protected void MOnDisable() {
	}

}
