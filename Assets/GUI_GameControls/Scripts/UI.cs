//Author: Bryan Leister
//Date: April, 2014
//
//Description: 
//Very basic GUI to get things started
//Uses some of the other Helper scripts created by me like 'Extensions_Audio' and 'Extensions_Texture' to handle fading in and out
//

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class UI : MonoBehaviour
{

		public GameObject		m_gameManager;					//Send our UI messages to this object or default to this.gameObject as the message receiver...
		public GameObject 		m_backgroundScreen;				//Image to show for registering the projection
		public GUISkin 			m_guiSkin;						//Style the buttons
		public AudioClip 		m_clickSound;					//play a click sound when clicking a button
		public bool				m_useFixedSizeGUI = true ;      //Editor option to use calculations for window sizes, if false we will calculate based on screen width
		public string 			m_message = "Start the Game!";	//A place for messages to be displayed, the GameManager script should set this as needed
		private string 			m_scoreMessage = "Player 1: 0" +
				"\nPlayer 2: 0";

		private float			m_space = 10;					//Spacer between GUI Elements
		private bool			m_showGUI = false;				//Show or Hide the GUI

		private AudioSource 	m_audioSource;					//A game audio source to show how to use boolean toggles and set preferences
		private bool			m_showMessagePanel = false;		//Do we want to see messages? Just demonstrating player preferences and gui stuff
		private bool			m_showButtonPanel = false;		//Do we want to see the Button panel? Just demonstrating player preferences and gui stuff
		private bool			m_showPreferences = true;


		private float 			m_smallWindowWidth = 200;							//hard code our GUI window dimensions
		private float 			m_largeWindowWidth = 400;
		private float 			m_windowHeight = 400;

		private Rect 			m_preferenceWindow = new Rect (50, 50, 200, 200);	//Default values for our GUI windows, starting with the Preference Window
		private Rect 			m_playerButtonWindow = new Rect (50, 50, 200, 200);	//The GUI Window where we put player buttons
		private Rect 			m_messageWindow = new Rect (50, 50, 200, 200);		//A GUI Window to display Messages from the GameManager like scores and whose turn it is


		void Awake ()
		{
				//First thing to do is turn off the background image and make it transparent so it can later fade in during the StartGame function
				if (m_backgroundScreen != null) {
						m_backgroundScreen.renderer.material.color = new Color (m_backgroundScreen.renderer.material.color.r, m_backgroundScreen.renderer.material.color.g,
                                          m_backgroundScreen.renderer.material.color.b, 0.0f);
						m_backgroundScreen.SetActive (false);
				}

				m_audioSource = this.GetComponent<AudioSource> () as AudioSource;

				if (m_gameManager == null) {
						Debug.Log ("We don't have a game manager for " + this.name + ", sending messages to myself instead...");
						m_gameManager = this.gameObject;
				}
		}


		void Start ()
		{
				ShowGUI ();
		}

		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Space)) {				//Show the GUI if the spacebar is pressed. Dismissing the GUI is done via a button in the GUI ("Press when Finished")
						
						m_showGUI = !m_showGUI; //toggle the gui from what it was to the opposite
				}
		
		}


		void OnGUI ()
		{
				if (m_guiSkin != null) 
						GUI.skin = m_guiSkin;													//Use a gui skin if it is specified

				m_showPreferences = GUILayout.Toggle (m_showPreferences, "Set Preferences");	//Persistent Toggle for the the preference panel

				if (!m_showGUI)																
						return;

				if (m_showPreferences)
						m_preferenceWindow = GUILayout.Window (0, m_preferenceWindow, GUIPreferenceWindow, "Preferences", GUILayout.Width (m_smallWindowWidth));


				if (m_showButtonPanel)
						m_playerButtonWindow = GUILayout.Window (1, m_playerButtonWindow, GUIPlayerChoices, "Player Choices", GUILayout.Width (m_smallWindowWidth));
		
				if (m_showMessagePanel)
						m_messageWindow = GUILayout.Window (2, m_messageWindow, GUIMessages, m_message, GUILayout.Width (m_largeWindowWidth));

		}


#region Custom functions start here

		void SetUpGUIDimensions ()
		{

				m_windowHeight = Screen.height * .5f;

				//A simple way to set up the Gui windows, most dimensions are hard coded as pixel Dimensions
				m_preferenceWindow = new Rect (20, 20, m_smallWindowWidth, m_windowHeight);
				m_messageWindow = new Rect ((Screen.width * .5f) - m_smallWindowWidth, 20, m_smallWindowWidth, m_windowHeight);
				m_playerButtonWindow = new Rect (Screen.width - (m_smallWindowWidth + 20), 20, m_smallWindowWidth, m_windowHeight);
				
				if (m_useFixedSizeGUI)
						return;

				//A more robust way to set up the windows, width is a percentage the screen width.
				//If your buttons are too wide, it might not work
				float sideMargin = Screen.width * .05f;
				float topMargin = sideMargin;
				if (topMargin > Screen.height * .1f)
						topMargin = Screen.height * .1f;

				m_smallWindowWidth = Screen.width * .2f;
				m_largeWindowWidth = Screen.width * .4f;
				

				m_preferenceWindow = new Rect (sideMargin, topMargin, m_smallWindowWidth, m_windowHeight);
				m_messageWindow = new Rect ((Screen.width * .5f) - (m_largeWindowWidth * .5f), topMargin, m_largeWindowWidth, m_windowHeight);
				m_playerButtonWindow = new Rect (Screen.width - (m_smallWindowWidth + sideMargin), topMargin, m_smallWindowWidth, m_windowHeight);
		}

		void CheckForPlayerPreferences ()
		{
				int check = PlayerPrefs.GetInt ("ShowMessagePanel");
		
				if (check == 1) {
						m_showMessagePanel = true;
				} else {
						m_showMessagePanel = false;
			
				}

				check = PlayerPrefs.GetInt ("ShowButtonPanel");
				if (check == 1) {
						m_showButtonPanel = true;
				} else {
						m_showButtonPanel = false;
				}

		}

		void SetPlayerPreferences ()
		{
				if (!m_showMessagePanel) {
						PlayerPrefs.SetInt ("ShowMessagePanel", 0);
				} else {
						PlayerPrefs.SetInt ("ShowMessagePanel", 1);
				}
		
				if (!m_showButtonPanel) {
						PlayerPrefs.SetInt ("ShowButtonPanel", 0);
				} else {
						PlayerPrefs.SetInt ("ShowButtonPanel", 1);
				}


		}

		void ShowGUI ()
		{
				SetUpGUIDimensions ();
				CheckForPlayerPreferences ();
		
				if (m_backgroundScreen == null) {
						Debug.LogWarning ("No background image! I can't show a background image if it doesn't exist!");
						m_showGUI = true;
						return;
				}
		
				m_backgroundScreen.SetActive (true);
		
				//Fade the background image alpha from black '0f' (transparent) to white '1f' (opaque). 
				//Material must support transparency for this to work! Once the fade is complete, toggle the GUI
				StartCoroutine (m_backgroundScreen.transform.renderer.material.FadeMaterialAlpha (0f, 1.0f, 1f, 2.0f, () =>
				{
						m_showGUI = true;
				}
				));
		}

		void MessageIs (string message)												//This catches the message from whomever sends it here and posts it to the message area
		{
				m_message = message;
		}

		void ScoreIs (Vector2 scores)												//This catches the score from whomever sends it here, and posts it
		{
				string message = "Player 1 = " + scores.x + "\nPlayer 2 = " + scores.y;
				m_scoreMessage = message;
		}


		void TakeTurn (GamePiece play)												//If no Game Manager is set up in the Editor and linked to this object, we send messages about the game state to ourself.
		{
				Debug.Log (this.name + " talking here. No game manager object specified, so not keeping score. But, just so you know -\n"
						+ play.player.ToString () + " chose the " + play.playerChoice.ToString ());
		}

#endregion

#region Custom GUI Window functions start here, they are run from the OnGUI Built-in function up above
		void GUIPreferenceWindow (int windowID)
		{

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				m_showMessagePanel = GUILayout.Toggle (m_showMessagePanel, " Show Message Panel?");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);
		
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				m_showButtonPanel = GUILayout.Toggle (m_showButtonPanel, " Show Button Panel?");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);
		
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Save Preferences")) {
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
						m_showPreferences = false;
						SetPlayerPreferences ();
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();


		}

		void GUIPlayerChoices (int windowID)
		{

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player1 Picks Rock")) {

						if (m_gameManager != null) {

								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player1;
								_play.playerChoice = GamePiece.PlayerChoice.Rock;

								m_gameManager.SendMessage ("TakeTurn", _play);
						}

						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

		
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player1 Picks Paper")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player1;
								_play.playerChoice = GamePiece.PlayerChoice.Paper;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player1 Picks Scissors")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player1;
								_play.playerChoice = GamePiece.PlayerChoice.Scissors;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player2 Picks Rock")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player2;
								_play.playerChoice = GamePiece.PlayerChoice.Rock;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player2 Picks Paper")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player2;
								_play.playerChoice = GamePiece.PlayerChoice.Paper;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player2 Picks Scissors")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player2;
								_play.playerChoice = GamePiece.PlayerChoice.Scissors;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player1 Hits Reset")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player1;
								_play.playerChoice = GamePiece.PlayerChoice.Reset;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Player2 Hits Reset")) {
			
						if (m_gameManager != null) {
				
								GamePiece _play = new GamePiece ();
								_play.player = GamePiece.Player.Player2;
								_play.playerChoice = GamePiece.PlayerChoice.Reset;
				
								m_gameManager.SendMessage ("TakeTurn", _play);
						}
			
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
		
				GUILayout.Space (m_space);


		}

		void GUIMessages (int windowID)
		{
		
				GUILayout.Space (m_space);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Label (m_scoreMessage);
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();

				GUILayout.Space (m_space);
		
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Label ("Press the space bar to show and hide these GUI windows " +
						"To test the game logic, press the buttons to the right. " +
						"The left panel is demonstrating preference setting and saving.");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();

		}

#endregion
	
}
