using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class BaseSceneClass : MonoBehaviour
{
	// Narayana
	#region Members
	//Our screen variable, useful for portrait to landscape
	public static Rect ourScreen;
	public GUIStyle scoreStyle;
	private int Size;
	public List<Word> words;
	public List<WordMeaning> meanings;
	public GameObject[] cards;
	public GameObject controller;
	public bool touchScreen = false;
	public int cardPick;
	public int cardPick2;
	public List<int> alreadyUsed;
	public int rightSound;
	public int lastCard;
	public Card selectedCard;
	public Card secondCard;
	public int correctAnswers = 0;
	public int wrongAnswers = 0;
	public bool SelectedType = false;
	public bool initialized = false;
	public bool finished = false;
	public VocabType Type;
	public bool backed = false;

	//Gui style to give the quiz a different color and style to its buttons and text
	public GUIStyle Quizbox;

	private GameObject sceneObjects;
	private GameObject[] cameras;
	public float volume;
	private float delay;
	private bool playedSound = false;
	private MainMenuUI DB;

	bool check = false;

	//Used after completing a Quiz
	private Rect QuizScore = new Rect( Screen.width / 8, Screen.height / 8, Screen.width - Screen.width / 4, Screen.height - Screen.height / 8 - Screen.height / 3 - Screen.height / 10 );
	private Rect QuizRetry = new Rect( Screen.width / 8 - Screen.height / 20, Screen.height - Screen.height / 3, Screen.width / 2 - Screen.width / 8, Screen.height / 3 - Screen.height / 20 );
	private Rect QuizBack = new Rect( Screen.width / 2, Screen.height - Screen.height / 3, Screen.width / 2 - Screen.width / 8, Screen.height / 3 - Screen.height / 20 );

	//Enumerator to tell the BSC which activity type we're running
	public enum VocabType : int
	{
		Main,
		Learn,
		Practice,
		Quiz,
		Review
	}
	#endregion

	//Awake is called before Start, this is where you prep the components that may be used by other classes
	void Awake ()
	{

		if ( Screen.height > Screen.width )
		{
			ourScreen = new Rect( 0, 0, Screen.height, Screen.width );
			fillBox();// this is called touchScreen assist with the Android orientation
		}
		else
			ourScreen = new Rect( 0, 0, Screen.width, Screen.height );

	}

	//Called when the object is instantiated
	void Start ()
	{
		//Grab Main camera
		cameras = GameObject.FindGameObjectsWithTag( "MainCamera" );

		if ( cameras.Length > 1 )
		{
			for ( int i = 0; i < cameras.Length; i++ )
			{
				if ( !cameras [ i ].GetComponent<BaseSceneClass>().backed )
					DestroyImmediate( cameras [ i ] );// if there is more than one main camera, destroy the new one
				//The new one will not have the bool Backed = true
			}
		}

		//The main menu runs during main
		Type = VocabType.Main;
		DB = GameObject.Find( "Main Camera" ).GetComponent<MainMenuUI>();

#if UNITY_ANDROID || UNITY_IPHONE
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		//Orient the Phone/tablets to the landscape position
#endif
	}

	//Take the resized screen and fix the quiz Rects
	private void fillBox ()
	{
		QuizScore = new Rect( ourScreen.width / 8, ourScreen.height / 8, ourScreen.width - ourScreen.width / 4, ourScreen.height - ourScreen.height / 8 - ourScreen.height / 3 - ourScreen.height / 10 );
		QuizRetry = new Rect( ourScreen.width / 8 - ourScreen.height / 20, ourScreen.height - ourScreen.height / 3, ourScreen.width / 2 - ourScreen.width / 8, ourScreen.height / 3 - ourScreen.height / 20 );
		QuizBack = new Rect( ourScreen.width / 2, ourScreen.height - ourScreen.height / 3, ourScreen.width / 2 - ourScreen.width / 8, ourScreen.height / 3 - ourScreen.height / 20 );
	}

	void Update ()
	{
		//if no selected type, run the main menu
		if ( SelectedType )
		{
			//Solid state machine to run the vocabs
			//We will first artificially initialize them, then run the updates.
			switch ( Type )
			{
				case VocabType.Main:
					if ( !initialized )
						InitializeMain();
					else
						UpdateMain();
					break;

				case VocabType.Learn:
					if ( !initialized )
						InitializeLearn();
					else
						UpdateLearn();
					break;

				case VocabType.Practice:
					if ( !initialized )
						InitializePractice();
					else
						UpdatePractice();
					break;

				case VocabType.Quiz:
					if ( !initialized )
						InitializeQuiz();
					else
						UpdateQuiz();
					break;

				case VocabType.Review:
					if ( !initialized )
						InitializeReview();
					else
						UpdateReview();
					break;
			}
		}
	}

	#region Initialize
	/// <summary>
	/// The main initialize function that will be called before
	/// the initializing of other initialize functions
	/// </summary>
	void InitializeMain ()
	{
		//Grab the words and meanings
		words = DB.selectedVocab.words;
		meanings = DB.selectedVocab.wordMeanings;
		//Grab the card objects
		cards = GameObject.FindGameObjectsWithTag( "Card" );
		//Shuffle the cards into a random order
		cards.Shuffle();

		//For each card, take a word and meaning and apply their pictures and sounds to the cards
		Card currentCard;
		for ( int i = 0; i < cards.Length; i++ )
		{
			currentCard = cards [ i ].GetComponent<Card>();
			currentCard.wordName = words [ i ].name;
			currentCard.sound = words [ i ].sound;
			currentCard.volume = words [ i ].soundVolume;
			for ( int j = 0; j < meanings.Count; j++ )
			{
				if ( meanings [ j ].name == currentCard.wordName )
				{
					currentCard.picture = meanings [ j ].picture;
					break;
				}
			}
		}
		sceneObjects = GameObject.FindWithTag( "SceneObjects" );

		//Make sure the cards and buttons are in a good position for every camera and screen size
		Resize( sceneObjects );

		/*
		Debug.Log( Screen.width.ToString() + ( " " ) + Screen.height.ToString() );
		if ( Screen.width == 1920 && Screen.height == 1080 )
		{
			cardPrefab.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2 + Screen.width / 10, Screen.height / 2, 6 ) );
			selectables.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 7, Screen.height / 2, 6 ) );
		}
		else if ( Screen.width == 1024 && Screen.height == 768 )
		{
			cardPrefab.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2 + Screen.width / 10, Screen.height / 2, 7 ) );
			selectables.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 7, Screen.height / 2, 8 ) );
		}
		*/
		initialized = true;
	}

	//Just run initialize main
	void InitializeLearn ()
	{
		InitializeMain();

		initialized = true;
	}

	//Randomly take the right or wrong sound and select it first
	void InitializeReview ()
	{
		InitializeMain();

		cardPick = Random.Range( 0, cards.Length );
		selectedCard = cards [ cardPick ].GetComponent<Card>();

		rightSound = Random.Range( 0, 2 );
		if ( rightSound == 1 )
		{
			secondCard = selectedCard;
		}

		if ( rightSound == 0 )
		{
			do
			{

				cardPick2 = Random.Range( 0, cards.Length );

			} while ( cardPick2 == cardPick );

			secondCard = cards [ cardPick2 ].GetComponent<Card>();
		}

		initialized = true;
	}

	//Randomly select a card
	void InitializePractice ()
	{
		InitializeMain();

		cardPick = Random.Range( 0, cards.Length );
		selectedCard = cards [ cardPick ].GetComponent<Card>();

		initialized = true;
	}

	//Randomly select a card
	void InitializeQuiz ()
	{
		InitializeMain();

		cardPick = Random.Range( 0, cards.Length );
		selectedCard = cards [ cardPick ].GetComponent<Card>();
	}
	#endregion

	#region Updates
	void UpdateMain ()
	{
		//Don't do anything, its the main menu
	}

	//Take the individual input and play the sound accordingly
	void UpdateLearn ()
	{
		#region Input
		if ( Input.GetMouseButtonDown( 0 ) == true )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

			//If the user clicked a card, play its sound
			if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
			{
				Debug.Log( hit.collider.gameObject.GetComponent<Card>().wordName );
				selectedCard = hit.collider.gameObject.GetComponent<Card>();
				audio.clip = selectedCard.sound;
				audio.volume = selectedCard.volume;
				audio.Play();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
			{
				audio.Play();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
			{
				Back();
			}
		}
		#endregion
	}

	void UpdateReview ()
	{
		//When the sound is not playing and the colors are neutral
		//Select the net random sound to play and ask the user if that is the right card.
		//One random sound, and 50%  chance of it being correct
		if ( finished && greyed() )
		{
			finished = false;
			do
			{
				cardPick = Random.Range( 0, cards.Length );
			} while ( cardPick == lastCard );

			selectedCard = cards [ cardPick ].GetComponent<Card>();

			rightSound = Random.Range( 0, 2 );

			if ( rightSound == 1 )
			{
				secondCard = selectedCard;
				playedSound = false;
			}
			if ( rightSound == 0 )
			{
				do
				{
					cardPick2 = Random.Range( 0, cards.Length );

				} while ( cardPick2 == cardPick );

				secondCard = cards [ cardPick2 ].GetComponent<Card>();
				playedSound = false;
			}
		}

		//Once the cards are Grey and the answer has been chosen, play the sound
		if ( playedSound == false && cards [ lastCard ].GetComponent<Card>().done() && greyed() )
		{
			audio.clip = secondCard.sound;
			audio.volume = secondCard.volume;
			audio.Play();
			playedSound = true;

		}

		#region Input
		//The user can only select objects when the cards are not lit up
		if ( Input.GetMouseButtonDown( 0 ) == true && greyed() )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

			if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Yes" && secondCard == selectedCard )
			{
				selectedCard.Correct(); //Turn the card green
				lastCard = cardPick;
				finished = true;

			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Yes" && secondCard != selectedCard )
			{
				selectedCard.Wrong(); //Turn the selected card wrong
				secondCard.Correct(); //Turn the correct card green
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "No" && secondCard == selectedCard )
			{
				selectedCard.Wrong();// Turn the card red, this was the right one
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "No" && secondCard != selectedCard )
			{
				selectedCard.Correct(); // Turn the correct card green
				lastCard = cardPick;
				finished = true;
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
			{
				audio.Play();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
			{
				Back();
			}
		}
		#endregion

	}

	void UpdatePractice ()
	{
		if ( playedSound == false )
		{
			if ( !audio.isPlaying && greyed() )
			{
				audio.clip = selectedCard.sound;
				audio.volume = selectedCard.volume;
				audio.Play();
				playedSound = true;
			}
		}

		#region Input

		if ( Input.GetMouseButtonDown( 0 ) && greyed() )
		{
			playedSound = false;

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

			//If the ray hits the card that has been selected
			if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.GetComponent<Card>() == selectedCard )
			{
				//add to our used list
				alreadyUsed.Add( cardPick );
				Debug.Log( hit.collider.gameObject.GetComponent<Card>().wordName );

				// you are correct, card glows green
				hit.collider.gameObject.GetComponent<Card>().Correct();

				bool same = false;

				//if we've gone through all 12 cards, repeat
				if ( alreadyUsed.Count == cards.Length )
				{
					alreadyUsed.Clear();
				}

				do
				{
					//select a new card randomly
					same = false;
					cardPick = Random.Range( 0, cards.Length );
					foreach ( int element in alreadyUsed )
					{
						if ( element == cardPick )
						{
							same = true;
						}
					}

				} while ( same == true );

				selectedCard = cards [ cardPick ].GetComponent<Card>();
				playedSound = false;

			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
			{
				//light up the card red to show the sound was for a different card
				hit.collider.gameObject.GetComponent<Card>().Wrong();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
			{
				audio.Play();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
			{
				Back();
			}
		}

		#endregion
	}

	void UpdateQuiz ()
	{
		//Play the audio if it hasn't been played
		if ( playedSound == false )
		{
			audio.clip = selectedCard.sound;
			audio.volume = selectedCard.volume;
			audio.Play();
			playedSound = true;
		}

		#region Input

		if ( finished == false )
		{
			if ( Input.GetMouseButtonDown( 0 ) )
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

				//If the ray cast hit a correct card
				if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.GetComponent<Card>() == selectedCard )
				{
					//add it to our list
					alreadyUsed.Add( cardPick );

					//tell the car the user made a correct choice
					hit.collider.gameObject.GetComponent<Card>().quiz = true;
					correctAnswers++;

					bool same = false;

					//UI component informing the user they are finished with scores included.
					if ( alreadyUsed.Count == cards.Length )
					{
						finished = true;
						delay = 5f;
					}
					else
					{
						do
						{
							//find the next random card we haven't used
							same = false;
							cardPick = Random.Range( 0, cards.Length );
							foreach ( int element in alreadyUsed )
							{
								if ( element == cardPick )
								{
									same = true;
									break;
								}
							}

						} while ( same == true );

						//play the audio
						selectedCard = cards [ cardPick ].GetComponent<Card>();
						playedSound = false;
					}

				}
				//If the user selected the wrong card
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
				{
					//add the card that was selected to the list of used cards
					alreadyUsed.Add( cardPick );
					hit.collider.gameObject.GetComponent<Card>().quiz = false;
					wrongAnswers++;
					bool same = false;
					delay = 5f;

					//if the quiz is done, show the results
					if ( alreadyUsed.Count == cards.Length )
					{
						finished = true;
					}
					else
					{
						//Randomly select a new card that we haven't used
						do
						{
							same = false;
							cardPick = Random.Range( 0, cards.Length );
							foreach ( int element in alreadyUsed )
							{
								if ( element == cardPick )
								{
									same = true;
									break;
								}
							}

						} while ( same == true );

						//Set to play the sound next update
						selectedCard = cards [ cardPick ].GetComponent<Card>();
						playedSound = false;

					}

				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
				{
					audio.Play();
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
				{
					Back();
				}
			}
		}
		#endregion
		//When the quiz is over
		if ( finished == true )
		{
			//Change the Color for each card
			for ( int i = 0; i < cards.Length; i++ )
			{
				cards [ i ].GetComponent<Card>().change = true;
			}

			//count down the delay for the results menu
			if ( delay >= 0 )
			{
				delay -= Time.deltaTime;
			}
			//Display UI buttons
			//button press code for buttons and what they dude
		}

	}
	#endregion

	void OnGUI ()
	{

		#region Quiz
		//make sure this GUI only runs during Quiz
		if ( ( int ) Type == ( int ) VocabType.Quiz )
		{
			//Build a string with the current results
			string score = "Correct = " + correctAnswers.ToString() + "\nIncorrect = " + wrongAnswers.ToString();

			//Display the current results
			GUI.Box( new Rect( 0, 0, ourScreen.width / 6, scoreStyle.fontSize * 2 + 5 ), score, scoreStyle );

			if ( finished && delay <= 0 )
			{
				//These are the users total results
				GUI.Box( QuizScore, "Completed score totals are:\n\n" + score, Quizbox );

				//if they hit the retry button
				if ( GUI.Button( QuizRetry, "Retry", Quizbox ) )
				{
					for ( int i = 0; i < cards.Length; i++ )
					{
						//reset the cards
						cards [ i ].GetComponent<Card>().reset();
					}
					//clear the used list
					alreadyUsed.Clear();
					correctAnswers = wrongAnswers = 0;
					finished = false;

					cardPick = Random.Range( 0, cards.Length );
					selectedCard = cards [ cardPick ].GetComponent<Card>();

					playedSound = false;
					//always use the event
					Event.current.Use();
				}

				if ( GUI.Button( QuizBack, "Back", Quizbox ) )
				{
					Event.current.Use();
					Back();

				}

			}
		}
		#endregion

	}

	/// <summary>
	/// This function resets all the necessary variables for going back to the main menu
	/// </summary>
	void Back ()
	{
		DB.delay = .5f;
		audio.Stop();
		alreadyUsed.Clear();
		backed = true;
		DB.hideMenuGUI = true;
		DB.hideActivityMenuGui = false;
		Type = VocabType.Main;
		correctAnswers = 0;
		wrongAnswers = 0;
		SelectedType = false;
		initialized = false;
		finished = false;
		playedSound = false;
		Application.LoadLevel( "Main" );
	}
	/// <summary>
	/// this is used to check is the colors of the cards are set back to Grey
	/// </summary>
	/// <returns></returns>
	private bool greyed ()
	{
		check = false;

		foreach ( GameObject cardcolor in cards )
		{
			check = cardcolor.GetComponent<Card>().check();

			if ( !check )
				break;
		}

		return check;
	}
	/// <summary>
	/// Resize takes the active activity scene's sceneObjects prefab and 
	/// moves them to the center of the screen and a depth that has been
	/// determined to fit on screen, it also sets the scoreStyle for the quiz
	/// scene.
	/// </summary>
	/// <param name="sceneObjects"></param>
	void Resize ( GameObject sceneObjects )
	{

		sceneObjects.transform.position = Camera.main.ScreenToWorldPoint(
			new Vector3( ourScreen.width / 2 - ourScreen.width / 10, ourScreen.height / 2, 6 ) );

		scoreStyle.fontSize = ( int ) ourScreen.width / 100;
		if ( scoreStyle.fontSize < 16 )
		{
			scoreStyle.fontSize = 16;
		}
		scoreStyle.alignment = TextAnchor.LowerLeft;
		scoreStyle.clipping = TextClipping.Overflow;
		scoreStyle.normal.textColor = Color.white;
		scoreStyle.hover.textColor = Color.white;
	}

}

/// <summary>
/// Thread safe shuffle extension to the list item. This is found on StackOverflow, but edited to fit our program
/// </summary>
public static class MyExtensions
{
	//it automatically takes its self as a argument
	public static void Shuffle<T> ( this IList<T> list )
	{
		int n = list.Count;
		while ( n > 1 )
		{
			n--;
			int k = ThreadSafeRandom.ThisThreadsRandom.Next( n + 1 );
			//Use a temp to replace the object
			T value = list [ k ];
			list [ k ] = list [ n ];
			list [ n ] = value;
		}
	}

	public static class ThreadSafeRandom
	{
		[System.ThreadStatic]
		private static System.Random Local;

		public static System.Random ThisThreadsRandom
		{
			get { return Local ?? ( Local = new System.Random( unchecked( System.Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId ) ) ); }
		}
	}
}


/*
 * 
#region Touch
#if UNITY_ANDROID || UNITY_IPHONE

if ( Input.touchCount > 0 )
{

	foreach ( Touch touch in Input.touches )
	{

		RaycastHit hit;
		if ( touch.phase == TouchPhase.Began )
		{
			Ray ray = Camera.main.ScreenPointToRay( touch.position );
			if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
			{
				selectedCard = hit.collider.gameObject.GetComponent<Card>();
				audio.clip = selectedCard.sound;
				audio.volume = selectedCard.volume;
				audio.Play();

			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
			{
				audio.Play();
			}
			else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
			{
				Back();
			}
		}
	}
}
#endif
#endregion
*/
//Useless Learn touch code

/*
	#region Touch
#if UNITY_ANDROID || UNITY_IPHONE

	if ( Input.touchCount > 0 )
	{

		foreach ( Touch touch in Input.touches )
		{
			if ( touch.phase == TouchPhase.Began )
			{
				RaycastHit hit;

				Ray ray = Camera.main.ScreenPointToRay( touch.position );
				if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Yes" && secondCard == selectedCard )
				{
					selectedCard.Correct();

					lastCard = cardPick;
					// UI code telling user they are correct
					do
					{
						cardPick = Random.Range( 0, cards.Length );
					} while ( cardPick == lastCard );


					selectedCard = cards [ cardPick ].GetComponent<Card>();

					rightSound = Random.Range( 0, 2 );

					if ( rightSound == 1 )
					{
						secondCard = selectedCard;
						playedSound = false;
					}
					if ( rightSound == 0 )
					{
						do
						{

							cardPick2 = Random.Range( 0, cards.Length );

						} while ( cardPick2 == cardPick );

						secondCard = cards [ cardPick2 ].GetComponent<Card>();
						playedSound = false;
					}
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Yes" && secondCard != selectedCard )
				{
					selectedCard.Wrong();
					secondCard.Correct();
					//UI code telling the user they are wrong
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "No" && secondCard == selectedCard )
				{
					selectedCard.Wrong();
					//UI code telling the user they are wrong
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "No" && secondCard != selectedCard )
				{
					selectedCard.Correct();
					// UI code telling user they are correct
					lastCard = cardPick;
					do
					{
						cardPick = Random.Range( 0, cards.Length );
					} while ( cardPick == lastCard );

					selectedCard = cards [ cardPick ].GetComponent<Card>();

					rightSound = Random.Range( 0, 2 );

					if ( rightSound == 1 )
					{
						secondCard = selectedCard;
						playedSound = false;
					}
					if ( rightSound == 0 )
					{
						do
						{

							cardPick2 = Random.Range( 0, cards.Length );

						} while ( cardPick2 == cardPick );

						secondCard = cards [ cardPick2 ].GetComponent<Card>();
						playedSound = false;
					}
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
				{
					audio.Play();
				}
				else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
				{
					Back();
				}
			}
		}
	}
#endif
	 * */
//Useless Review touch code

/*
		#region Touch
#if UNITY_ANDROID || UNITY_IPHONE
		if ( Input.touchCount > 0 )
		{

			foreach ( Touch touch in Input.touches )
			{
				if ( touch.phase == TouchPhase.Began )
				{
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay( touch.position );

					if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.GetComponent<Card>() == selectedCard )
					{

						alreadyUsed.Add( cardPick );
						// UI element exclaiming you are correct

						bool same = false;

						if ( alreadyUsed.Count == cards.Length )
						{
							alreadyUsed.Clear();
						}

						do
						{
							same = false;
							cardPick = Random.Range( 0, cards.Length );
							foreach ( int element in alreadyUsed )
							{

								if ( element == cardPick )
								{
									same = true;
									break;
								}

							}
						} while ( same );

						selectedCard = cards [ cardPick ].GetComponent<Card>();
						playedSound = false;

					}
					else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
					{
						// UI element exclaiming you are wrong
						// Code for UI Element
					}
					else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
					{
						audio.Play();
					}
					else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
					{
						Back();
					}
				}
			}
		}
#endif
		#endregion
*/
//Useless Practice touch code

/*
		#region Touch
#if UNITY_ANDROID || UNITY_IPHONE
		if ( finished == false )
		{

			if ( Input.touchCount > 0 )
			{

				foreach ( Touch touch in Input.touches )
				{

					if ( touch.phase == TouchPhase.Began )
					{
						RaycastHit hit;
						Ray ray = Camera.main.ScreenPointToRay( touch.position );

						if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.GetComponent<Card>() == selectedCard )
						{
							alreadyUsed.Add( cardPick );
							hit.collider.gameObject.GetComponent<Card>().quiz = true;
							correctAnswers++;
							bool same = false;
							if ( alreadyUsed.Count == cards.Length )
							{
								finished = true;
								//UI component informing you are finished with scores included.
							}
							else
							{
								do
								{
									same = false;
									cardPick = Random.Range( 0, cards.Length );
									foreach ( int element in alreadyUsed )
									{
										if ( element == cardPick )
										{
											same = true;
											break;
										}
									}

								} while ( same == true );
								selectedCard = cards [ cardPick ].GetComponent<Card>();
								playedSound = false;

							}

						}
						else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Card" )
						{
							alreadyUsed.Add( cardPick );
							hit.collider.gameObject.GetComponent<Card>().quiz = false;
							wrongAnswers++;
							bool same = false;
							if ( alreadyUsed.Count == cards.Length )
							{
								finished = true;
								//UI component informing you are finished with scores included.
							}
							else
							{
								do
								{
									same = false;
									cardPick = Random.Range( 0, cards.Length );
									foreach ( int element in alreadyUsed )
									{
										if ( element == cardPick )
										{
											same = true;
											break;
										}
									}

								} while ( same == true );
								selectedCard = cards [ cardPick ].GetComponent<Card>();
								playedSound = false;
							}

						}
						else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Repeat" )
						{
							audio.Play();
						}
						else if ( Physics.Raycast( ray, out hit ) && hit.collider.gameObject.tag == "Back" )
						{
							Back();
						}
					}
				}
			}
		}
#endif
		#endregion
*/
//Useless Quiz Touch Code