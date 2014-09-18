using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SQLiteWrapper
{
	public Texture background;
	public Texture forground;
	private SqliteDatabase db = null; //The database variable

	#region Connection
	/// <summary>
	/// Function that initiates the connection to the database
	/// </summary>
	/// <param name="path"></param>
	public void OpenDatabase ( string p )
	{
		string filepath = p;
		/*
		if(!File.Exists(filepath))
 
		{
 
			// if it doesn't ->
 
			// open StreamingAssets directory and load the db ->
 
			WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + p);  // this is the path to your StreamingAssets in android
 
			while(!loadDB.isDone) {}  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
 
			// then save to Application.persistentDataPath
 
			File.WriteAllBytes(filepath, loadDB.bytes);
		}
		*/
		//open db connection

		db = new SqliteDatabase( filepath );
	}

	/// <summary>
	/// Returns true if open, false if close.
	/// <para></para>
	/// If the database is neither of those, 
	/// it will put the current state into the debug log as an error
	/// </summary>
	/// <returns></returns>
	public bool CheckState()
	{
		return db.IsConnectionOpen;
	}
	/*
	/// <summary>
	/// Close the database and null out the connection
	/// </summary>
	public void CloseDatabase()
	{
		db.Close();
		db = null;
	}
	 * */
	//Can't use Close use with SqliteDatabase
	#endregion

	#region Queries
	/// <summary>
	/// Queries the database for Languages and returns the list
	/// </summary>
	/// <returns></returns>
	public List<Language> QueryDatabaseForLanguages ()
	{
		//Create the query command and generate a reader for the output
		DataTable LangTable = db.ExecuteQuery( "SELECT ID, Name, Description FROM Language" );

		List<Language> values = new List<Language>();
		//reader.Read() sets the reader to read the next row of fields.
		//The first call sets for the first row
		foreach ( DataRow row in LangTable.Rows )
		{
			//reader can be referanced by the fields names.
			//by doing so you will have to cast the returned "object"
			values.Add( new Language()
						{
							ID = ( string ) row [ "Id" ],
							Name = ( string ) row [ "Name" ],
							Description = ( string ) row [ "Description" ]
						} );
		}

		return values;
	}

	/// <summary>
	/// Query Database for Courses using the Language ID and return a List<Course>
	/// </summary>
	/// <param name="LangID"></param>
	/// <returns></returns>
	public List<Course> QueryDatabaseForCourses ( string LangID )
	{
		//Create Reader for __to__ table and pull the associated ID's
		DataTable CourseTable =  db.ExecuteQuery( "SELECT * FROM Course WHERE LanguageID = '" + LangID + "'" );
		
		//List to hold the Levels the user can select
		List<Course> CourseList = new List<Course>();
		
		//Read the first (or next) row
		foreach ( DataRow row in CourseTable.Rows )
		{
			//Add this row to the level list
			CourseList.Add( new Course()
			{
				ID = ( string ) row [ "Id" ],
				Name = ( string ) row [ "Name" ],
				Description = ( string ) row [ "Description" ],
				UserID = ( string ) row [ "UserId" ],
				LanguageID = ( string ) row [ "LanguageId" ]
			} );
		}


		return CourseList;
	}

	/// <summary>
	/// Queries the database for the given language and returns the Levels associated
	/// </summary>
	/// <returns></returns>
	public List<Level> QueryDatabaseForLevels ( string CourseID )
	{
		//Create Reader for __to__ table and pull the associated ID's
		DataTable LevelTable = db.ExecuteQuery( "SELECT * FROM Level WHERE CourseId = '" + CourseID + "'" );

		//List to hold the Levels the user can select
		List<Level> levelList = new List<Level>();

		//Read the first (or next) row
		foreach ( DataRow row in LevelTable.Rows )
		{
			//Add this row to the level list
			levelList.Add( new Level()
			{
				ID = ( string ) row [ "Id" ],
				Name = ( string ) row [ "Name" ],
				Description = ( string ) row [ "Description" ],
				CourseID = ( string ) row [ "CourseId" ]
			} );
		}


		return levelList;
	}

	/// <summary>
	/// Queries the database for the given language and returns the Level associated
	/// </summary>
	/// <returns></returns>
	public List<Unit> QueryDatabaseForUnits ( string levID )
	{
		DataTable UnitTable = db.ExecuteQuery( "SELECT * FROM Unit WHERE LevelID = '" + levID + "'" );

		//List to hold the Units the user can select
		List<Unit> unitlist = new List<Unit>();

		//Read the first (or next) row
		foreach ( DataRow row in UnitTable.Rows )
		{
			//Add this row to the unitlist
			unitlist.Add( new Unit()
			{
				ID = ( string ) row [ "Id" ],
				Name = ( string ) row [ "Name" ],
				Description = ( string ) row [ "Description" ],
				LevelID = ( string ) row [ "LevelID" ]
			} );
		}


		return unitlist;
	}

	/// <summary>
	/// Queries the database for the given Unit and returns the Lessons associated
	/// </summary>
	/// <returns></returns>
	public List<Lesson> QueryDatabaseForLessons ( string unitID )
	{
		//Create query for __to__ table and pull the associated ID's
		DataTable LessonTable = db.ExecuteQuery( "SELECT * FROM Lesson WHERE UnitID = '" + unitID + "'" );

		//List to hold the Lessons the user can select
		List<Lesson> lessonlist = new List<Lesson>();

		//Read the first (or next) row
		foreach ( DataRow row in LessonTable.Rows )
		{
			//Add this row to the lessonlist
			lessonlist.Add( new Lesson()
			{
				ID = ( string ) row [ "Id" ],
				Name = ( string ) row [ "Name" ],
				Description = ( string ) row [ "Description" ],
				UnitID = ( string ) row [ "UnitID" ]
			} );
		}


		return lessonlist;
	}

	/// <summary>
	/// Query the database for the Vocab we've selected, and fill it's contents
	/// </summary>
	/// <param name="lesson"></param>
	/// <returns></returns>
	public List<Vocabulary> QueryDatabaseForVocabs ( string lessonID )
	{
		//Create query for __to__ table and pull the associated ID's
		DataTable VocabTable = db.ExecuteQuery( "SELECT * FROM Activity WHERE LessonID = '" + lessonID + "'" );

		//List to hold the Lessons the user can select
		List<Vocabulary> VocabularyList = new List<Vocabulary>();

		//Read the first (or next) row
		foreach ( DataRow row in VocabTable.Rows )
		{
			//Add this row to the lessonlist
			VocabularyList.Add( new Vocabulary()
			{
				ID = ( string ) row [ "Id" ],
				Name = ( string ) row [ "Name" ],
				Description = ( string ) row [ "Description" ],
				LessonID = ( string ) row [ "LessonID" ]
			} );
		}

		return VocabularyList;
	}

	/// <summary>
	/// Query the database for the Vocab we've selected, and fill it's contents
	/// </summary>
	/// <param name="lesson"></param>
	/// <returns></returns>
	public Vocabulary QueryDatabaseForVocab ( Vocabulary Vocab )
	{
		//Fill the Vocabs words nad meanings
		Vocab.words = QueryDatabaseForWords( Vocab.ID );
		Vocab.wordMeanings = QueryDatabaseForMeanings( Vocab.ID );
		return Vocab;
	}

	/// <summary>
	/// Queries the database for Words and returns the list
	/// </summary>
	/// <returns></returns>
	public List<Word> QueryDatabaseForWords ( string ContainingID )
	{
		//Create query for __to__ table and pull the associated ID's
		DataTable WordIDTable = db.ExecuteQuery( "SELECT * FROM VocabularyItem WHERE ActivityID = '" + ContainingID + "'" );

		//List to hold the Associated ID's
		List<string> WordID = new List<string>();
		foreach ( DataRow row in WordIDTable.Rows )
		{
			//Gather the ID's of each Word from the associated Vocab
			WordID.Add( ( string ) row [ "WordId" ] );
		}

		//List to hold the Words
		List<Word> values = new List<Word>();

		//For each lessonID in the list, pull the associated Lesson and add it to the list
		foreach ( string ID in WordID )
		{
			//Create the query command and generate a reader for the output
			DataTable WordTable = db.ExecuteQuery( "SELECT * FROM Word WHERE ID = '" + ID + "'" );

			//reader.Read() sets the reader to read the next row of fields.
			//The first call sets for the first row
			foreach ( DataRow row in WordTable.Rows )
			{
				//Start by taking the name and blob from the database
				AudioClipData NewClip = new AudioClipData()
				{
					Name = ( string ) row [ "Name" ]
				};
				//Create the nessessary information
				NewClip.Compile( ( byte [] ) row [ "Sound" ] );

				//Create the sound clip from the given data
				AudioClip clip = AudioClip.Create(
					NewClip.Name,
					NewClip.length,
					NewClip.Channels,
					NewClip.Frequency,
					NewClip.Sound3D,
					NewClip.Stream
					);
				clip.SetData( NewClip.AudioSamples, 0 );

				//reader can be referanced by the fields names.
				//by doing so you will have to cast the returned "object"
				values.Add( new Word()
				{
					ID = ( string ) row [ "Id" ],
					name = ( string ) row [ "Name" ],
					description = ( string ) row [ "Description" ],
					sound = clip,
					soundVolume = 1f,
					original = ( byte [] ) row [ "Sound" ]
				} );
			}
		}


		return values;
	}

	/// <summary>
	/// Queries the database for Meanings and returns the list
	/// </summary>
	/// <returns></returns>
	public List<WordMeaning> QueryDatabaseForMeanings ( string ContainingID )
	{
		//Create query for __to__ table and pull the associated ID's
		DataTable MeaningIDTable = db.ExecuteQuery( "SELECT * FROM VocabularyItem WHERE ActivityID = '" + ContainingID + "'" );

		List<string> MeaningID = new List<string>();

		foreach ( DataRow row in MeaningIDTable.Rows )
		{
			//Gather the ID's of each Word from the associated Vocab
			MeaningID.Add( ( string ) row [ "MeaningId" ] );
		}

		List<WordMeaning> meanings = new List<WordMeaning>();

		foreach ( string ID in MeaningID )
		{
			//Create the query command and generate a reader for the output
			DataTable MeaningTable = db.ExecuteQuery( "SELECT * FROM Meaning WHERE ID = '" + ID + "'" );

			//reader.Read() sets the reader to read the next row of fields.
			//The first call sets for the first row
			foreach ( DataRow row in MeaningTable.Rows )
			{
				Texture2D newTex = new Texture2D( 0, 0 );
				newTex.LoadImage( ( byte [] ) row [ "Picture" ] );

				meanings.Add( new WordMeaning()
				{
					ID = ( string ) row [ "Id" ],
					name = ( string ) row [ "Name" ],
					description = ( string ) row [ "Description" ],
					picture = newTex,
					original = ( byte [] ) row [ "Picture" ]
				} );
			}
		}

		return meanings;
	}

	/*
	public void ConvertToNew(Vocabulary vocab, Language lang)
	{
		FillVocabItem(vocab);
		FillWords(vocab, lang);
		FillWtoM(vocab);
		FillMeaning(vocab);
	}

	public void FillVocabItem(Vocabulary vocab)
	{
		var dbwrite = db.CreateCommand();

		//for each word change the command to insert the next set of data
		for ( int i =0; i < vocab.words.Count; i++ )
		{
			dbwrite.CommandText = "INSERT INTO 'main'.'VocabularyItem' ('ActivityID','MeaningID','WordID') VALUES (@ACTID, @MID, @WID)";
			dbwrite.Parameters.AddWithValue( "@ACTID", vocab.ID);
			dbwrite.Parameters.AddWithValue( "@WID", vocab.words [ i ].ID );
			dbwrite.Parameters.AddWithValue( "@MID", vocab.wordMeanings[i].ID );

			dbwrite.ExecuteNonQuery();
		}
	}

	public void FillWords(Vocabulary vocab, Language lang)
	{
		var dbwrite = db.CreateCommand();

		//for each word change the command to insert the next set of data
		for ( int i =0; i < vocab.words.Count; i++ )
		{
			dbwrite.CommandText = "INSERT INTO 'main'.'Word' ('ID','LanguageID','Name','Description','Sound','SoundVol') VALUES (@ID, @Lang, @Name, @Desc, @Sound, @SoundVol)";

			dbwrite.Parameters.AddWithValue( "@ID", vocab.words [ i ].ID );
			dbwrite.Parameters.AddWithValue( "@Lang", lang.ID );
			dbwrite.Parameters.AddWithValue( "@Name", vocab.words [ i ].name );
			dbwrite.Parameters.AddWithValue( "@Desc", vocab.words [ i ].description );
			dbwrite.Parameters.AddWithValue( "@Sound", vocab.words [ i ].original );
			dbwrite.Parameters.AddWithValue( "@SoundVol", 1 );

			dbwrite.ExecuteNonQuery();
		}
	}

	public void FillMeaning ( Vocabulary vocab)
	{
		var dbwrite = db.CreateCommand();

		//for each word change the command to insert the next set of data
		for ( int i =0; i < vocab.wordMeanings.Count; i++ )
		{
			dbwrite.CommandText = "INSERT INTO 'main'.'Meaning' ('ID','Name','Description','Picture','PartOfSpeechID') VALUES (@ID, @Name, @Desc, @Photo, @Part)";

			dbwrite.Parameters.AddWithValue( "@ID", vocab.wordMeanings [ i ].ID );
			dbwrite.Parameters.AddWithValue( "@Name", vocab.wordMeanings [ i ].name );
			dbwrite.Parameters.AddWithValue( "@Desc", vocab.wordMeanings [ i ].description );
			dbwrite.Parameters.AddWithValue( "@Photo", vocab.wordMeanings [ i ].original );
			dbwrite.Parameters.AddWithValue( "@Part", "Noun" );

			dbwrite.ExecuteNonQuery();
		}
	}

	public void FillWtoM(Vocabulary vocab)
	{
		var dbwrite = db.CreateCommand();

		//for each word change the command to insert the next set of data
		for ( int i =0; i < vocab.words.Count; i++ )
		{
			dbwrite.CommandText = "INSERT INTO 'main'.'WordToMeaning' ('WordID','MeaningID','DoNotIncludeInExam') VALUES (@WID, @MID, @BOOL)";

			dbwrite.Parameters.AddWithValue( "@WID", vocab.words [ i ].ID );
			dbwrite.Parameters.AddWithValue( "@MID", vocab.wordMeanings[i].ID );
			dbwrite.Parameters.AddWithValue( "@BOOL", false );

			dbwrite.ExecuteNonQuery();
		}
	} */
	//Conversion to new DB

	/*
	/// <summary>
	/// Example for putting together the required rows in the joining tables.
	/// </summary>
	public void FillWordToMeaning ()
	{
		//Create query for grabbing the first set of IDs
		var dbcmd1 = db.CreateCommand();
		dbcmd1.CommandText = "SELECT * FROM Word";
		var reader1 = dbcmd1.ExecuteReader();

		//Create query for grabbing the second set of IDs
		var dbcmd2 = db.CreateCommand();
		dbcmd2.CommandText = "SELECT * FROM Meaning";
		var reader2 = dbcmd2.ExecuteReader();

		//List to hold the Associated ID's
		List<string> words = new List<string>();
		List<string> meanings = new List<string>();

		while ( reader1.Read() )
		{
			//Gather the ID's of each word
			words.Add( ( string ) reader1 [ "Id" ] );
		}

		while ( reader2.Read() )
		{
			//Gather the ID's of each meaning
			meanings.Add( ( string ) reader2 [ "Id" ] );
		}

		var dbwrite = db.CreateCommand();
		
		//for each word change the command to insert the next set of data
		for(int i =0; i < words.Count; i++)
		{
			dbwrite.CommandText = string.Format("INSERT INTO 'main'.'WordToMeaning' ('WordID','MeaningID') VALUES ('{0}', '{1}')", words[i], meanings[i]);
			dbwrite.ExecuteNonQuery();
		}
	}*/
	//Fill wordtomeaningtable 

	#endregion
}
