using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Popup
{
	//Narayana
	static float timer = 0f;
	static int popupListHash = "PopupList".GetHashCode();

	public static bool List ( Rect position, ref bool entrySelected, ref bool showList, ref bool usable, ref int listEntry, GUIContent buttonContent, GUIContent [] listContent, GUIStyle listStyle )
	{
		return List( position, ref entrySelected, ref showList, ref usable, ref listEntry, buttonContent, listContent, "button", "box", listStyle );
	}

	public static bool List ( Rect position, ref bool entrySelected, ref bool showList, ref bool usable, ref int listEntry, GUIContent buttonContent, GUIContent [] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle )
	{
		// font sizing
		buttonStyle.fontSize = 18;
		buttonStyle.font = listStyle.font;
		//Stored mouse position for GUI selection detection
		Vector3 mousePos = new Vector3( Input.mousePosition.x, Input.mousePosition.y );
		bool done = false;

		//Redundancies just in case there is error in code which is calling pop up.
		if ( usable == false )
		{
			GUI.enabled = false;
			GUI.Label( position, buttonContent, buttonStyle );
			return done;
		}
		if ( listContent.Length == 0 )
		{

			GUI.Label( position, "N/A", buttonStyle );
			return done;
		}
		//Stored rectangle for selection detection
		Rect listRect = new Rect( position.x, position.y, position.width, position.height );

		/*Detects if the left mouse button has been depressed or the
		 * first touches phase has begun over the first layer of pop up GUI thus
		 * opening the selection grid for a selection to be made
		 * and then uses up the current event to disallow further accidental
		 *  GUI selection from the action. */
		if ( Event.current.type == EventType.mouseDown && Event.current.button == 0 && showList == false )
		{
			if ( guiContains( mousePos, position ) )
			{
				Debug.Log( "making it" );
				showList = true;
				Event.current.Use();
			}
		}
		/*Detects if the left mouse button is depressed  or the first
		 * touches phase has begun outside of the currently active
		 * selection grid and then closes said selection grid. */
		if ( Event.current.type == EventType.mouseDown && Event.current.button == 0 && showList == true )
		{
			if ( ( !guiContains( mousePos, listRect ) ) && timer <= 0 )
			{
				done = true;
			}
		}
		/*Detects if the left mouse button is depressed or
		 * the first touches phase has begun inside the selection grid area
		 * a timer is started and begins to run once the mouse is lifted 
		 * causing a selection to occur on the grid and then it promptly to close. */
		if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 && showList == true )
		{
			if ( guiContains( mousePos, listRect ) )
			{
				timer = .5f;

			}
		}
		//Timer code
		if ( timer > 0f && showList == true && !Input.GetMouseButton(0))
		{
			timer -= Time.deltaTime;
		}
		/*Conditional which causes the closing of the selection grid
		 * on the end of the timer */
		if ( timer < 0f && showList == true )
		{
			done = true;
			timer = 0f;
			
		}
		/*Shows a label describing what the selection is for 
		 * if list entry has not been changed.*/
		if ( showList == false && listEntry == -1 )
		{

			GUI.Label( position, buttonContent, buttonStyle );
		}
		/*Shows a label with the currently selected item in the list
		 * if there is one.*/
		else if ( showList == false )
		{

			GUI.Label( position, listContent [ listEntry ], buttonStyle );
		}
		//Displays selection grid if showlist is currently true
		if ( showList )
		{

			GUI.Box( listRect, "", boxStyle );
			listEntry = GUI.SelectionGrid( listRect, listEntry, listContent, 1, listStyle );
			entrySelected = false;
		}
		/* If list entry is changed and done is true the 
		 * selection grid is closed and the entry selected bool is changed.*/
		if ( done && listEntry != -1 )
		{
			showList = false;
			entrySelected = true;
		}
		/*if list entry remains unchanged and done is true the 
		 * selection grid is closed and entry selected is changed to false.*/
		else if ( done )
		{
			showList = false;
			entrySelected = false;
		}
		//uses up current event to prevent GUI bleed through.
		if ( Event.current.type == EventType.mouseDown && Event.current.button == 0 && done == true)
		{
			Event.current.Use();
		}

		return done;

	}

	/*function used for proper GUI to mouse detection since the 
	 * mouse input is anchored in the bottom left of the screen
	 * and the GUI is anchored in the top left of the screen.*/
	public static bool guiContains ( Vector3 mouse, Rect guiItem )
	{
		float X = mouse.x;
		float Y = Screen.height - mouse.y;

		if ( ( X < guiItem.xMin || X > guiItem.xMax ) || ( Y < guiItem.yMin || Y > guiItem.yMax ) )
		{
			return false;
		}
		else
		{
			return true;
		}

	}
}