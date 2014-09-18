using UnityEngine;
using System.Collections;

public class BackgroundSizer : MonoBehaviour {
	
	public void Start()
	{
		//Resize the background to fit the screen resolution
		 gameObject.GetComponent<GUITexture>().pixelInset = new Rect( 0 - BaseSceneClass.ourScreen.width / 2, 0 - BaseSceneClass.ourScreen.height / 2, BaseSceneClass.ourScreen.width, BaseSceneClass.ourScreen.height );
	}
}
