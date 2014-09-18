using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class MainMenuUI : MonoBehaviour
{
	//Android
	#region Members

	//Style control for GUI items
	public Texture2D sample1 = null;
	public Texture2D sample2 = null;
	public GUIStyle theListStyle;
	public Texture loading;
	public Texture VocabMenuTex;
	//Bools for hiding GUI items
	public bool hideMenuGUI = false;
	public bool hideActivityMenuGui = true;

	//Needed for activity loading
	public BaseSceneClass theScene;
	//Needed for database access
	private ConversionClass dataBase;

	/* From here to the next comment the data set here is being used for
	 * the pop up calls and queries.*/
	private List<Language> languageList;
	private List<Course> courseList;
	private List<Level> levelList;
	private List<Unit> unitList;
	private List<Lesson> lessonList;
	private List<Vocabulary> activityList;
	public Vocabulary selectedVocab;

	private bool languageQueried = false;
	private bool levelQueried = false;
	private bool courseQueried = false;
	private bool unitQueried = false;
	private bool lessonQueried = false;
	private bool activityQueried = false;
	private bool initialized = false;

	private bool languageUsable = false;
	private bool courseUsable = false;
	private bool levelUsable = false;
	private bool unitUsable = false;
	private bool lessonUsable = false;
	private bool activityUsable = false;

	private List<GUIContent> languageContentForPopup = new List<GUIContent>();
	private List<GUIContent> courseContentForPopup = new List<GUIContent>();
	private List<GUIContent> levelContentForPopup = new List<GUIContent>();
	private List<GUIContent> unitContentForPopup = new List<GUIContent>();
	private List<GUIContent> lessonContentForPopup = new List<GUIContent>();
	private List<GUIContent> activityContentForPopup = new List<GUIContent>();

	private bool showListLanguage;
	private bool showListCourse;
	private bool showListLevel;
	private bool showListUnit;
	private bool showListLesson;
	private bool showListActivity;

	private bool showGuiCourse;
	private bool showGuiLevel;
	private bool showGuiUnit;
	private bool showGuiLesson;
	private bool showGuiActivity;

	private int languageSelected = -1;
	private int courseSelected = -1;
	private int levelSelected = -1;
	private int unitSelected = -1;
	private int lessonSelected = -1;
	public int activitySelected = -1;

	private bool languageSelectedBool = false;
	private bool courseSelectedBool = false;
	private bool levelSelectedBool = false;
	private bool unitSelectedBool = false;
	private bool lessonSelectedBool = false;
	private bool activitySelectedBool = false;

	private int lastLanguage;
	private int lastCourse;
	private int lastLevel;
	private int lastUnit;
	private int lastLesson;
	// This data is being used for the loading of the activity
	private int lastActivity = -1;
	private int loader;
	private bool selectedVocabType = false;
	private bool load;
	private string level;
	private BaseSceneClass.VocabType type;
	public float delay;
	#endregion

	void Start ()
	{
		// Initialization of the main menu
		showListLanguage = false;
		hideMenuGUI = false;
		theScene = gameObject.GetComponent<BaseSceneClass>();
		dataBase = gameObject.GetComponent<ConversionClass>();

		// The setting of the GUI style for Pop up calls
		theListStyle.fontSize = ( BaseSceneClass.ourScreen.width / 100f < 16f ) ? 16 : ( int ) BaseSceneClass.ourScreen.width / 100;
		theListStyle.normal.textColor = Color.white;
		theListStyle.padding.left =
			theListStyle.padding.right =
				theListStyle.padding.top =
					theListStyle.padding.bottom = 4;

		if ( sample1 == null )
			theListStyle.onHover.background =
				theListStyle.hover.background = new Texture2D( 2, 2 );
		else
		{
			theListStyle.normal.background = sample1;
			theListStyle.onHover.background =
				theListStyle.hover.background = sample2;

		}
			
	}

	void Update ()
	{   //Delay for the activity menu
		if(delay >= 0)
		{
			delay -= Time.deltaTime;
		}
		//Database access
		#region QueryHandler
		/* This section is for language selection the database is queried and
		 * a list is filled with the returned language query it then takes all
		 * language names and stores them in a GUI content list to be used by
		 * pop up followed by bools being set to allow languages selection*/
		if ( languageSelectedBool == false && languageQueried == false )
		{
			languageList = dataBase.sql.QueryDatabaseForLanguages();
			foreach ( Language lang in languageList )
			{
				languageContentForPopup.Add( new GUIContent( lang.Name ) );
			}
			languageUsable = true;
			languageQueried = true;
		}
		/* This section is for course selection the database is queried and
		 * a list is filled with the returned course query it then takes all
		 * course names and stores them in a GUI content list to be used by
		 * pop up followed by bools being set to allow courses selection 
		 * and added code to handle a change in languages selection for those below
		 * it in the hierarchy*/
		if ( languageSelectedBool == true && courseQueried == false )
		{
			courseList = dataBase.sql.QueryDatabaseForCourses( languageList [ languageSelected ].ID );
			courseQueried = true;
			lastLanguage = languageSelected;

			foreach ( Course cour in courseList )
			{
				courseContentForPopup.Add( new GUIContent( cour.Name ) );
			}

			//listPopulatedCourse = true;
			if ( courseList.Count == 1 )
			{
				courseSelected = 0;
				courseSelectedBool = true;
			}

			courseUsable = true;
		}
		// Code section for handling change in selection
		if ( lastLanguage != languageSelected && courseQueried )
		{

			courseQueried =
				levelQueried =
					unitQueried =
						lessonQueried =
							activityQueried =
								courseSelectedBool =
									levelSelectedBool =
										unitSelectedBool =
											lessonSelectedBool =
												activitySelectedBool = false;

			courseSelected =
				levelSelected =
					unitSelected =
						lessonSelected =
							activitySelected = -1;
			courseUsable =
				levelUsable =
					unitUsable =
						lessonUsable =
							activityUsable = false;

			/*listPopulatedCourse=
				listPopulatedLevel =
					listPopulatedUnit = 
						listPopulatedLesson = 
							listPopulatedActivity = false;*/

			courseList.Clear();
			courseContentForPopup.Clear();

			if ( levelList != null )
			{
				levelList.Clear();
				levelContentForPopup.Clear();
			}
			if ( unitList != null )
			{
				unitList.Clear();
				unitContentForPopup.Clear();
			}
			if ( lessonList != null )
			{
				lessonList.Clear();
				lessonContentForPopup.Clear();
			}
			if ( activityList != null )
			{
				activityList.Clear();
				activityContentForPopup.Clear();
			}

			courseList = dataBase.sql.QueryDatabaseForCourses( languageList [ languageSelected ].ID );

			lastLanguage = languageSelected;
			//listPopulatedCourse = true;
			courseQueried = true;

			foreach ( Course cour in courseList )
			{
				courseContentForPopup.Add( new GUIContent( cour.Name ) );
			}
			if ( courseList.Count == 1 )
			{
				courseSelected = 0;
				courseSelectedBool = true;
			}
			courseUsable = true;
		}
		/* This section is for level selection the database is queried and
		 * a list is filled with the returned level query it then takes all
		 * level names and stores them in a GUI content list to be used by
		 * pop up followed by bools being set to allow levels selection 
		 * and added code to handle a change in course selection for those below
		 * it in the hierarchy*/
		if ( courseSelectedBool == true && levelQueried == false )
		{
			levelList = dataBase.sql.QueryDatabaseForLevels( courseList [ courseSelected ].ID );
			levelQueried = true;
			lastCourse = courseSelected;

			foreach ( Level lev in levelList )
			{
				levelContentForPopup.Add( new GUIContent( lev.Name ) );
			}

			//listPopulatedLevel = true;
			if ( levelList.Count == 1 )
			{
				levelSelected = 0;
				levelSelectedBool = true;
			}
			levelUsable = true;
		}
		//Code section for handling changes in selection
		if ( lastCourse != courseSelected && levelQueried )
		{
			levelQueried =
				unitQueried =
					lessonQueried =
						activityQueried =
							levelSelectedBool =
								unitSelectedBool =
									lessonSelectedBool =
										activitySelectedBool = false;

			levelSelected =
				unitSelected =
					lessonSelected =
						activitySelected = -1;
			levelUsable =
				unitUsable =
					lessonUsable =
						activityUsable = false;

			/* listPopulatedLevel =
				listPopulatedUnit = 
					listPopulatedLesson = 
						listPopulatedActivity = false;*/

			levelList.Clear();
			levelContentForPopup.Clear();



			if ( unitList != null )
			{
				unitList.Clear();
				unitContentForPopup.Clear();
			}
			if ( lessonList != null )
			{
				lessonList.Clear();
				lessonContentForPopup.Clear();
			}
			if ( activityList != null )
			{
				activityList.Clear();
				activityContentForPopup.Clear();
			}

			levelList = dataBase.sql.QueryDatabaseForLevels( courseList [ courseSelected ].ID );

			lastCourse = languageSelected;
			//listPopulatedLevel = true;
			levelQueried = true;

			foreach ( Level lev in levelList )
			{
				levelContentForPopup.Add( new GUIContent( lev.Name ) );
			}
			if ( levelList.Count == 1 )
			{
				levelSelected = 0;
				levelSelectedBool = true;
			}
			levelUsable = true;
		}
		/* This section is for unit selection the database is queried and
		 * a list is filled with the returned unit query it then takes all
		 * unit names and stores them in a GUI content list to be used by
		 * pop up followed by bools being set to allow units selection 
		 * and added code to handle a change in levels selection for those below
		 * it in the hierarchy*/
		if ( levelSelectedBool && unitQueried == false )
		{
			unitList = dataBase.sql.QueryDatabaseForUnits( levelList [ levelSelected ].ID );
			unitQueried = true;
			lastLevel = levelSelected;

			foreach ( Unit un in unitList )
			{
				unitContentForPopup.Add( new GUIContent( un.Name ) );
			}
			if ( unitList.Count == 1 )
			{
				unitSelected = 0;
				unitSelectedBool = true;
			}
			unitUsable = true;
			//listPopulatedUnit = true;
		}
		//Code section for handling changes in selection
		if ( lastLevel != levelSelected && unitQueried )
		{
			unitQueried =
				lessonQueried =
					activityQueried =
							unitSelectedBool =
								lessonSelectedBool =
									activitySelectedBool = false;
			unitSelected =
				lessonSelected =
					activitySelected = -1;
			unitUsable =
				lessonUsable =
					activityUsable = false;

			/*listPopulatedUnit = 
				listPopulatedLesson = 
					listPopulatedActivity = false;*/

			unitList.Clear();
			unitContentForPopup.Clear();
			if ( lessonList != null )
			{
				lessonList.Clear();
				lessonContentForPopup.Clear();
			}
			if ( activityList != null )
			{
				activityList.Clear();
				activityContentForPopup.Clear();
			}

			unitList = dataBase.sql.QueryDatabaseForUnits( levelList [ levelSelected ].ID );
			unitQueried = true;
			lastLevel = levelSelected;

			foreach ( Unit un in unitList )
			{
				unitContentForPopup.Add( new GUIContent( un.Name ) );
			}
			if ( unitList.Count == 1 )
			{
				unitSelected = 0;
				unitSelectedBool = true;
			}
			unitUsable = true;
			//listPopulatedUnit = true;

		}
		/* This section is for lesson selection the database is queried and
		 * a list is filled with the returned lesson query it then takes all
		 * lesson names and stores them in a GUI content list to be used by
		 * pop up followed by bools being set to allow lessons selection 
		 * and added code to handle a change in units selection for those below
		 * it in the hierarchy*/
		if ( unitSelectedBool && lessonQueried == false )
		{
			lessonList = dataBase.sql.QueryDatabaseForLessons( unitList [ unitSelected ].ID );
			lessonQueried = true;
			lastUnit = unitSelected;

			foreach ( Lesson less in lessonList )
			{
				lessonContentForPopup.Add( new GUIContent( less.Name ) );
			}
			if ( lessonList.Count == 1 )
			{
				lessonSelected = 0;
				lessonSelectedBool = true;
			}
			lessonUsable = true;
			//listPopulatedLesson = true;
		}
		//Code section for handling changes in selection
		if ( lastUnit != unitSelected && lessonQueried )
		{
			lessonQueried =
				activityQueried =
						lessonSelectedBool =
							activitySelectedBool = false;
			lessonSelected =
				activitySelected = -1;

			lessonUsable =
				activityUsable = false;

			/*listPopulatedLesson = 
				listPopulatedActivity = false;*/

			lessonList.Clear();
			if ( activityList != null )
			{
				activityList.Clear();
				activityContentForPopup.Clear();
			}

			lessonList = dataBase.sql.QueryDatabaseForLessons( unitList [ unitSelected ].ID );
			//listPopulatedLesson = true;
			lessonQueried = true;

			foreach ( Lesson less in lessonList )
			{
				lessonContentForPopup.Add( new GUIContent( less.Name ) );
			}
			lastUnit = unitSelected;
			if ( lessonList.Count == 1 )
			{
				lessonSelected = 0;
				lessonSelectedBool = true;
			}
			lessonUsable = true;
		}
		/* This section is for activity selection the database is partially 
		 * queried for names and placed in a list to be used by
		 * pop up followed by bools being set to allow activity's selection 
		 * and added code to handle a change in lessons selection */
		if ( lessonSelectedBool && activityQueried == false )
		{
			activityList = dataBase.sql.QueryDatabaseForVocabs( lessonList [ lessonSelected ].ID );
			activityQueried = true;
			lastLesson = lessonSelected;

			foreach ( Vocabulary act in activityList )
			{
				activityContentForPopup.Add( new GUIContent( act.Name ) );
			}
			if ( activityList.Count == 1 )
			{
				activitySelected = 0;
				activitySelectedBool = true;
			}
			activityUsable = true;
			//listPopulatedActivity =true;
		}
		//Code section for handling changes in selection
		if ( lastLesson != lessonSelected && activityQueried )
		{
			activityQueried = false;
			//activitySelectedBool = false;
			activitySelected = -1;
			activityUsable = false;
			//listPopulatedActivity = false;

			activityList.Clear();
			activityContentForPopup.Clear();

			activityList = dataBase.sql.QueryDatabaseForVocabs( lessonList [ lessonSelected ].ID );
			activityQueried = true;
			lastLesson = lessonSelected;

			foreach ( Vocabulary act in activityList )
			{
				activityContentForPopup.Add( new GUIContent( act.Name ) );
			}
			if ( activityList.Count == 1 )
			{
				activitySelected = 0;
				activitySelectedBool = true;
			}
			activityUsable = true;
			//listPopulatedActivity = true;

		}
		#endregion
		//Resets bools to allow moving from activity to activity
		if ( initialized )
		{
			initialized = false;
			theScene.SelectedType = true;
		}
	}

	void OnGUI ()
	{

		#region MainMenu
		//Calls for Pop up script
		if ( hideMenuGUI == false )
		{
			//Language
			Popup.List( // 1
				new Rect(
					0,
					0,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref languageSelectedBool, ref showListLanguage, ref languageUsable, ref languageSelected,
				new GUIContent( "Language" ),
				languageContentForPopup.ToArray(),
				theListStyle
				);
			//Course
			Popup.List( // 2
				new Rect(
					0,
					BaseSceneClass.ourScreen.height / 3,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref courseSelectedBool, ref showListCourse, ref courseUsable, ref courseSelected,
				new GUIContent( "Course" ),
				courseContentForPopup.ToArray(),
				theListStyle
				);
			//Level
			Popup.List( // 3
				new Rect(
					0,
					BaseSceneClass.ourScreen.height - BaseSceneClass.ourScreen.height / 3,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref levelSelectedBool, ref showListLevel, ref levelUsable, ref levelSelected,
				new GUIContent( "Level" ),
				levelContentForPopup.ToArray(),
				theListStyle
				);
			//Unit
			Popup.List( // 4
				new Rect(
					BaseSceneClass.ourScreen.width / 3,
					0,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref unitSelectedBool, ref showListUnit, ref unitUsable, ref unitSelected,
				new GUIContent( "Unit" ),
				unitContentForPopup.ToArray(),
				theListStyle
				);
			//Lesson
			Popup.List( // 5
				new Rect(
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref lessonSelectedBool, ref showListLesson, ref lessonUsable, ref lessonSelected,
				new GUIContent( "Lesson" ),
				lessonContentForPopup.ToArray(),
				theListStyle
				);
			//Activity
			Popup.List( // 6
				new Rect(
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height - BaseSceneClass.ourScreen.height / 3,
					BaseSceneClass.ourScreen.width / 3,
					BaseSceneClass.ourScreen.height / 3
					),
				ref activitySelectedBool, ref showListActivity, ref activityUsable, ref activitySelected,
				new GUIContent( "Activity" ),
				activityContentForPopup.ToArray(),
				theListStyle
				);
			/* Load Activity Button will need to be adjusted for use with
			 * other types of activities currently good for use with Vocabulary*/
			if ( activitySelected != -1 )
			{
				if ( GUI.Button(
					new Rect(
						BaseSceneClass.ourScreen.width - BaseSceneClass.ourScreen.width / 3,
						BaseSceneClass.ourScreen.height / 3,
						BaseSceneClass.ourScreen.width / 3,
						BaseSceneClass.ourScreen.height / 3
						),
					"Load Activity" ) )
				{
					Event.current.Use();
					theScene.initialized = false;
					hideMenuGUI = true;
					hideActivityMenuGui = false;
				}
			}
		}
		#endregion

		#region ActivityMenu
		// Activity Menu, will need adjusting to work with more then just vocabulary
		if ( hideActivityMenuGui == false )
		{

			//Used to draw the activity menu
			GUI.DrawTexture( new Rect( 0, 0, Screen.width, Screen.height ), VocabMenuTex );

			// Using the guiContains function, make invisible buttons over the textures prompting for each Vocab type and well as a back button
			if ( Input.GetMouseButtonUp( 0 ) && delay <= 0)
			{
				if ( Popup.guiContains( Input.mousePosition, new Rect( 0, 0, BaseSceneClass.ourScreen.width * 0.43f, BaseSceneClass.ourScreen.height / 3 ) ) )
				{
					Fill( "learn", BaseSceneClass.VocabType.Learn );
				}

				if ( Popup.guiContains( Input.mousePosition, new Rect( BaseSceneClass.ourScreen.width - BaseSceneClass.ourScreen.width * 0.43f, 0, BaseSceneClass.ourScreen.width * 0.43f, BaseSceneClass.ourScreen.height / 3 ) ) )
				{
					Fill( "Practice", BaseSceneClass.VocabType.Practice );
				}

				if ( Popup.guiContains( Input.mousePosition, new Rect( 0, BaseSceneClass.ourScreen.height - BaseSceneClass.ourScreen.height / 3, BaseSceneClass.ourScreen.width * 0.43f, BaseSceneClass.ourScreen.height / 3 ) ) )
				{
					Fill( "Review", BaseSceneClass.VocabType.Review );
				}

				if ( Popup.guiContains( Input.mousePosition, new Rect( BaseSceneClass.ourScreen.width - BaseSceneClass.ourScreen.width * 0.43f, BaseSceneClass.ourScreen.height - BaseSceneClass.ourScreen.height / 3, BaseSceneClass.ourScreen.width * 0.43f, BaseSceneClass.ourScreen.height / 3 ) ) )
				{
					Fill( "Quiz", BaseSceneClass.VocabType.Quiz );
				}

				if ( Popup.guiContains( Input.mousePosition, new Rect( BaseSceneClass.ourScreen.width / 3, BaseSceneClass.ourScreen.height / 3, BaseSceneClass.ourScreen.width / 3, BaseSceneClass.ourScreen.height / 3 ) ) )
				{
					Back();
				}
			}
		}
		#endregion

		#region Finalize
		//Loading screen code section
		//This is here since loading on the same GUI update would not render the loading dialog
		if ( selectedVocabType )
		{
			GUI.DrawTexture( new Rect(
						BaseSceneClass.ourScreen.width / 3, //starting point
						BaseSceneClass.ourScreen.height / 3,//Starting point
						BaseSceneClass.ourScreen.width / 3, //Width
						BaseSceneClass.ourScreen.height / 3 //Height
						), loading );

			if ( loader > 0 )
				loader--;

			if ( loader == 0 )
			{
				selectedVocabType = false;
				load = true;
			}
		}
		#endregion

		#region Load
		//Activity Loading code section
		if ( load )
		{
			if ( lastActivity != activitySelected )
			{
				//Only pull the words and meaning from the database if we haven't already. 
				//This will always run the first time and whenever the selected vocab changes
				selectedVocab = dataBase.sql.QueryDatabaseForVocab( activityList [ activitySelected ] );
				lastActivity = activitySelected;
			}

			load = false;
			Application.LoadLevel( level );
			theScene.Type = type;
			initialized = true;
		}

		#endregion
	}

	private void Back ()
	{
		hideActivityMenuGui = true;
		hideMenuGUI = false;
	}

	//sets all the variables for loading the vocab type
	private void Fill ( string slevel, BaseSceneClass.VocabType stype )
	{

		hideActivityMenuGui = true;
		selectedVocabType = true;
		level = slevel;
		type = stype;
		loader = 15;
	}

}

/*
 * Useless touch code, sadly not used anymore
#if UNITY_ANDROID || UNITY_IPHONE
			if ( Input.touchCount > 0 )
			{
				foreach ( Touch touch in Input.touches )
				{
					if ( touch.phase == TouchPhase.Began && Popup.guiContains( touch.position, new Rect( 0, 0, Screen.width * 0.43f, Screen.height / 3 ) ) )
					{
						Fill( "learn", BaseSceneClass.VocabType.Learn );
					}

					if ( touch.phase == TouchPhase.Began && Popup.guiContains( touch.position, new Rect( Screen.width - Screen.width * 0.43f, 0, Screen.width * 0.43f, Screen.height / 3 ) ) )
					{
						Fill( "Practice", BaseSceneClass.VocabType.Practice );
					}

					if ( touch.phase == TouchPhase.Began && Popup.guiContains( touch.position, new Rect( 0, Screen.height - Screen.height / 3, Screen.width * 0.43f, Screen.height / 3 ) ) )
					{
						Fill( "Review", BaseSceneClass.VocabType.Review );
					}

					if ( touch.phase == TouchPhase.Began && Popup.guiContains( touch.position, new Rect( Screen.width - Screen.width * 0.43f, Screen.height - Screen.height / 3, Screen.width * 0.43f, Screen.height / 3 ) ) )
					{
						Fill( "Quiz", BaseSceneClass.VocabType.Quiz );
					}

					if ( touch.phase == TouchPhase.Began && Popup.guiContains( touch.position, new Rect( Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3 ) ) )
					{
						Back();
					}
				}
			}
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
*/
//Useless touch code