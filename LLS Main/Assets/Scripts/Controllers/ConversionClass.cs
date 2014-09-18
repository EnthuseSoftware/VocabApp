using UnityEngine;

public class ConversionClass : MonoBehaviour{

    public Language language; //Language selected
	public Course course; //Course selected
	public Level level; //Level Selected
	public Unit unit; //Unit Selected
	public Lesson lesson; //Lesson Selected
	public Vocabulary Vocab;
	public SQLiteWrapper sql;

	/*
	private bool initialized = false;
	*/
	
	private string dbConnection;

	void Awake()
	{
		// When scene starts, Grab the database and check the state
		dbConnection = "config.db";
		sql = new SQLiteWrapper();
		sql.OpenDatabase( dbConnection );

		//Set this object as being persistant
		DontDestroyOnLoad( gameObject );
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) ) { Application.Quit(); }
		/*
		if(!initialized)
		{
			initialized = true;
			GetComponent<BaseSceneClass>().Type = BaseSceneClass.VocabType.Learn;
			GetComponent<BaseSceneClass>().SelectedType = true;
		}
		 */
	}

	void OnGUI()
	{
		//GUI.Box( new Rect( 0, 0, 50, 20 ), sql.CheckState().ToString() );
	}
}
