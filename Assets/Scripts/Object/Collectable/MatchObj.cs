using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchObj : CollectableObj {

	[SerializeField] MCharacter parent;
	[SerializeField] List<string> matchTags; //tags of the item this will match to
	[SerializeField] LogicEvents onFillRaiseEvent;
	[SerializeField] LogicEvents onFillLowerEvent;


	public override bool Select (ClickType clickType)
	{
		base.Select (clickType);
		return true;
	}

	public override bool UnSelect ()
	{
		base.UnSelect ();
		return true;
	}

	public override void OnFill ( float delay )
	{
		base.OnFill ( delay );

        //Debug.Log( "in on Fill in Match Obj " + gameObject.name );
		if (storySoundSource != null && storySoundSource.isPlaying) {
			storySoundSource.Stop ();
		}

		if (storySoundSource != null )// && GetStoryTimer( ) == 0f )
        {
            //Debug.Log( " call coroutine in Match Obj " );
            //wait until the "hole" prompt has played
            StartCoroutine( DelaySoundClipPlay( storySoundSource, delay ) );
        } 
		else
        {
            //Debug.Log( " call next event in Match Obj " );
            CallNextEvent( );
        }
	}

    IEnumerator DelaySoundClipPlay( AudioSource audiosource, float delay )
    {
        //Debug.Log( Time.timeSinceLevelLoad + " before first delay in delay soundclipplay " );
        yield return new WaitForSeconds( delay );
        //Debug.Log( Time.timeSinceLevelLoad + " before second delay in delay soundclipplay " );
        audiosource.Play( );
        yield return new WaitForSeconds( audiosource.clip.length );
        //Debug.Log( Time.timeSinceLevelLoad + " before callnextevent in delay soundclipplay " );
        CallNextEvent( );
    }

    void CallNextEvent( )
    {
        //print ("in match on fill");
        // other option: one "on fill" event, bool on arg for up or down, check bool in MCharacter
        if ( gameObject.tag == "Raise" )
        {
            M_Event.FireLogicEvent( onFillRaiseEvent, new LogicArg( this ) );
            LogicArg logicArg = new LogicArg( this );
            M_Event.FireLogicEvent( LogicEvents.ExitStory, logicArg );
        }
        else if ( gameObject.tag == "Lower" )
        {
            M_Event.FireLogicEvent( onFillLowerEvent, new LogicArg( this ) );
            LogicArg logicArg = new LogicArg( this );
            M_Event.FireLogicEvent( LogicEvents.ExitStory, logicArg );
        }
        else if ( gameObject.tag == "Tutorial" )
        {
			MetricManagerScript.instance.AddToMatchList( Time.timeSinceLevelLoad + " in call exitstorytutorial "  + "/n");
            LogicArg logicArg = new LogicArg( this );
            M_Event.FireLogicEvent( LogicEvents.ExitStoryTutorial, logicArg );
        }

    }


}
