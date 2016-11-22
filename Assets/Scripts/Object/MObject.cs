using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MObject : MBehavior {

	[SerializeField] protected AudioClip focusSound;
	private AudioSource focusSoundSource;
[SerializeField] bool lockScale = false;
	private Vector3 origianScale;
    private float focusTimer = 0f;
    private float storyTimer = 0f;
    protected int focusCount = 0;

    protected override void MAwake ()
	{
		base.MAwake ();
		if (focusSound != null) {
			focusSoundSource = gameObject.AddComponent<AudioSource> ();
			focusSoundSource.playOnAwake = false;
			focusSoundSource.loop = false;
			focusSoundSource.volume = 1f;
			focusSoundSource.spatialBlend = 1f;
			focusSoundSource.clip = focusSound;
		}
		origianScale = transform.lossyScale;
	}

	/// <summary>
	/// a varible to record whether the object is being focused
	/// </summary>
	private bool m_isFocus = false;
	public bool isFocused
	{
		get { return isFocused; }
	}


	/// <summary>
	/// Call when the input manager checked that the object is on focus
	/// </summary>
	virtual public void OnFocus()
	{
		m_isFocus = true;
		if (focusSoundSource != null && focusTimer == 0f )
        {
            focusSoundSource.Play( );
            focusCount++;
        }
			
	}

	/// <summary>
	/// Call when the input manager checked that the object is out of focus
	/// </summary>
	virtual public void OnOutofFocus()
	{
		m_isFocus = false;
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();
		if (lockScale) {
			Vector3 temScale = transform.lossyScale;
			if (temScale != origianScale) {
				Vector3 localScale = transform.localScale;
				localScale.x *= origianScale.x / temScale.x;
				localScale.y *= origianScale.y / temScale.y;
				localScale.z *= origianScale.z / temScale.z;
				transform.localScale = localScale;
			}
		}
        if(focusCount >= 2 )
        {
            focusTimer = 5f;
            focusCount = 0;
        }

        if(focusTimer > 0f )
        {
            focusTimer -= Time.deltaTime;
        }
        else
        {
            focusTimer = 0f;
        }

        if ( storyTimer > 0f )
        {
            storyTimer -= Time.deltaTime;
        }
        else
        {
            storyTimer = 0f;
        }

    }

    protected float GetFocusTimer( )
    {
        return focusTimer;
    }

    protected void SetFocusTimer( float time )
    {
       focusTimer = time;
    }

    protected float GetStoryTimer( )
    {
        return storyTimer;
    }

    protected void SetStoryTimer( float time )
    {
        storyTimer = time;
    }
}
