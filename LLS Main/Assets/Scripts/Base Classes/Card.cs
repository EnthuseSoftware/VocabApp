using UnityEngine;

public class Card : MonoBehaviour
{
	//Our card information
	public string wordName;
	public AudioClip sound;
	public float volume;
	public Texture picture;
	public GameObject backGround;

	private GameObject theCamera;

	//our bools
	public bool quiz;
	public bool change = false;
	private bool start = false;

	//Colors to use
	private Color target;
	private Color green = Color.green;
	private Color red = Color.red;
	private Color grey = Color.grey;
	private Color blue = Color.blue;
	public float lerptime = 5;
	private float elapsed = 0;

	private BaseSceneClass BSC;

	void Start ()
	{
		backGround = transform.FindChild( "Back" ).gameObject;// Grab the background of this card
		theCamera = GameObject.Find( "Main Camera" );// Grab the camera
		BSC = theCamera.GetComponent<BaseSceneClass>();// Grab the base scene class
		backGround.renderer.material.color = grey; // Set the background game object to Grey
	}

	void Update ()
	{
		//Solid state machine for which update to follow
		switch ( BSC.Type )
		{
			case BaseSceneClass.VocabType.Learn:
				UpdateLearn();
				break;

			case BaseSceneClass.VocabType.Practice:
				UpdatePractice();
				break;

			case BaseSceneClass.VocabType.Review:
				UpdateReview();
				break;

			case BaseSceneClass.VocabType.Quiz:
				UpdateQuiz();
				break;

		}

		//Make sure the picture is the picture
		if ( renderer.material.mainTexture != picture )
		{
			renderer.material.SetTexture( "_MainTex", picture );
		}
	}

	private void UpdateLearn ()
	{
		//While the object is playing its sound, change the color of the card associated with it
		//The time it changes is directly correlated with the audios length
		if ( theCamera.audio.isPlaying && theCamera.audio.clip == sound && !change )
		{
			lerptime = theCamera.audio.clip.length;
			change = true;
			target = green;
		}

		if ( change )
			elapsed += Time.deltaTime;

		//When the timer stops, change it back to Grey
		if ( change && elapsed >= lerptime + .1f || theCamera.audio.clip != sound )
		{
			backGround.renderer.material.color = grey;
			change = false;
			elapsed = 0;
		}

		if ( change )
			changeColor( target );
	}

	private void UpdatePractice ()
	{
		//once the object is selected, change the color of the card depending on if it was correct
		//The time it changes is directly correlated with the audios length
		if ( change && !start )
		{
			lerptime = theCamera.audio.clip.length;
			start = true;
			change = false;
		}

		if ( start )
			elapsed += Time.deltaTime;

		if ( start && elapsed >= lerptime + .1f )
		{
			backGround.renderer.material.color = grey;
			start = false;
			elapsed = 0;
		}
		if ( start )
			changeColor( target );
	}

	private void UpdateReview ()
	{
		//If the cameras audio is playing and its the same as the
		//selected card, turn it blue
		if ( theCamera.audio.isPlaying && BSC.selectedCard.sound == sound && !change )
		{
			lerptime = theCamera.audio.clip.length;
			change = true;
			target = blue;
		}

		if ( change )
			elapsed += Time.deltaTime;

		if ( change && elapsed > lerptime + 1 )
		{
			backGround.renderer.material.color = grey;
			change = false;
			elapsed = 0;
		}

		if ( change )
			changeColor( target );

		//The card will indicate correct or wrong using the BSC review update
	}

	private void UpdateQuiz ()
	{
		//change will be triggered through the update of quiz
		if ( change )
			elapsed += Time.deltaTime;

		if ( change && elapsed >= lerptime + .1f )
		{
			change = false;
			elapsed = 0;
		}

		if ( change )
		{
			if ( quiz )
				changeColor( green );
			else
				changeColor( red );
		}


	}

	void changeColor ( Color colorTo )
	{
		//lerp the background color of the cards to the indicated color
		backGround.renderer.material.color = Color.Lerp( backGround.renderer.material.color, colorTo, elapsed / lerptime );
	}

	public void Correct ()
	{
		change = true;
		target = green;
	}

	public void Wrong ()
	{
		change = true;
		target = red;
	}

	public void Grey ()
	{
		change = true;
		target = grey;
	}

	public bool done ()
	{
		return !change;
	}

	/// <summary>
	/// Check to see if this cards background color is Grey, if it is not, return false
	/// </summary>
	/// <returns></returns>
	public bool check ()
	{
		
		if ( backGround.renderer.material.color == grey )
			return true;
		else
			return false;
	}

	/// <summary>
	/// Make this cards background color Grey
	/// </summary>
	public void reset ()
	{
		change = false;
		backGround.renderer.material.color = grey;
	}
}
