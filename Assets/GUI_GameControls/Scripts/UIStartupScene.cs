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
public class UIStartupScene : MonoBehaviour
{

		public GameObject		m_gameManager;							//Send our UI messages to this object or default to this.gameObject...
		public GUISkin 			m_guiSkin;								//Style the buttons by specifying a GUI Skin
		public bool 			m_checkMouseOrTouch = false;			//Turn this on here or in the GUI preferences to use your mouse or touch for the buttons
		public LayerMask 		m_layersToCheck;						//We should limit our detection to only the layer the button is on 
		public string			m_buttonTag = "Button";					//We will also check for it to be tagged "Button"
		public AudioClip 		m_clickSound;							//play a click sound when clicking a button
		public GameObject[]		m_objectsToToggleOnOff;					//Turn off all of these objects if we don't want to use them

		private bool 			m_showObjects = false;					//Hide the objects in our m_objectsToToggleOnOff array
		private string			m_listOfObjectsToToggle;				//We'll report to the GUI what objects will be hidden
		private AudioSource 	m_audioSource;							//Audio attached to this object for gui clicks
		private bool			m_showGUI = false;						//Hide the gui when we don't want it.
		private bool 			m_isSelected = false;					//Boolean to keep track of our mouse and touch sending
		private Transform 		m_hitTransform = null;					//When using checkMouseOrTouch, we'll store the object selected, then deselect it if hover time is not reached.

		private Rect 			m_preferenceWindow = new Rect (50, 50, 200, 200);	//Default values for our GUI windows, we'll change this to percentage of screen size

		void Awake ()
		{
				m_audioSource = this.audio;
				CheckForPlayerPreferences ();
				DisplayGameObjects (m_showObjects, m_objectsToToggleOnOff);

				if (m_guiSkin == null) {
						Debug.LogWarning ("No GUI skin attached to " + this.name + ". Will use the default GUI instead. Yuck!");
				}

				foreach (GameObject g in m_objectsToToggleOnOff) {
						if (g != null)
								m_listOfObjectsToToggle += g.name + ", ";
				}

				if (m_gameManager == null) {
						Debug.LogWarning ("We don't have a game manager for " + this.name + ", not sure where to send" +
								" messages... I guess I'll use the " + this.name + " as a message receiver. Not sure if it will do anything though...");
						m_gameManager = this.gameObject;
				}
		}

		void Start ()
		{
				//Center the preference window in the middle of the screen
				float windowWidth = Screen.width * .25f;
				float windowHeight = Screen.height * .5f;
				Vector2 centerPointOfScreen = new Vector2 (Screen.width * .5f, Screen.height * .5f);

				m_preferenceWindow = new Rect (centerPointOfScreen.x - windowWidth / 2, centerPointOfScreen.y - windowHeight / 2, windowWidth, windowHeight);
		}

		void Update ()
		{
				if (!m_checkMouseOrTouch)
						return;								//Don't waste time listening for mouse or touch motion, since it's not needed

				#if UNITY_EDITOR || UNITY_STANDALONE
				MouseMoveUpdate ();
				#endif

				#if UNITY_ANDROID || UNITY_IPHONE
				TouchMoveUpdate ();
				#endif
		}
	
		void OnGUI ()
		{

				if (m_guiSkin != null) 
						GUI.skin = m_guiSkin;
		
				GUILayout.BeginHorizontal ();
				m_showGUI = GUILayout.Toggle (m_showGUI, "Preferences");
				GUILayout.EndHorizontal ();

				if (!m_showGUI)												//Don't continue if we don't want to see the preference window
						return;

				m_preferenceWindow = GUILayout.Window (0, m_preferenceWindow, GUIPreferenceWindow, "Settings");
		}

		void GUIPreferenceWindow (int windowID)
		{
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				m_showObjects = GUILayout.Toggle (m_showObjects, "Deactivate " + m_listOfObjectsToToggle);
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();


				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				m_checkMouseOrTouch = GUILayout.Toggle (m_checkMouseOrTouch, "Check for Mouse and Touch");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();

				GUILayout.FlexibleSpace ();
		
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Space (10);
				if (GUILayout.Button ("Press When Finished")) {
						if (m_clickSound != null) {
								m_audioSource.playClip (m_clickSound);
						}

						//If we are using this button to DoSomething on the Game Manager Object, 
						//the function name would be sent here. You can optionally require a reciever
						//and/or pass a parameter. This passes the int 1 to the GameManager function 'void AddScore(int score)':
						//
						//	m_gameManager.SendMessage("AddScore", 1); 
						//
						m_gameManager.SendMessage ("DoSomething", SendMessageOptions.DontRequireReceiver); 

						SetPlayerPreferences ();															//In this case, it's GUI related, so we'll just do our own functions directly
						DisplayGameObjects (m_showObjects, m_objectsToToggleOnOff);							
						m_showGUI = false;																	//We made our selection, let's hide the preferences
			
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();

				GUILayout.FlexibleSpace ();
		
		}

		void StartGame ()																					//This function is thrown in here in case a default button is hooked up to it, and we want to do something.
		{
				Debug.Log ("Got the button message, loading the default level");
				StopAllCoroutines ();
				Application.LoadLevel (0);
		}
	
		void CheckForPlayerPreferences ()
		{
				int check = PlayerPrefs.GetInt ("ShowObjects");
		
				if (check == 1) {
						m_showObjects = true;
				} else {
						m_showObjects = false;
				}
		}
	
		void SetPlayerPreferences ()
		{
				if (!m_showObjects) {
						PlayerPrefs.SetInt ("ShowObjects", 0);
				} else {
						PlayerPrefs.SetInt ("ShowObjects", 1);
				}

		}

		void DisplayGameObjects (bool show, GameObject[] objects)
		{
				foreach (GameObject g in objects) {
						if (g != null)
								g.SetActive (show);
				}
		}
	
		void MouseMoveUpdate ()
		{

				
				if (Input.GetMouseButton (0)) {
			
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit;
			
						if (Physics.Raycast (ray, out hit, 100f, m_layersToCheck)) {

								if (!m_isSelected && hit.transform.tag == m_buttonTag) {
										m_isSelected = true;
										m_hitTransform = hit.transform;
										m_hitTransform.SendMessage ("SendEnterSignals", SendMessageOptions.DontRequireReceiver);
								}
						}
				}

				if (Input.GetMouseButtonUp (0)) {
						m_isSelected = false;
						if (m_hitTransform != null) {
								m_hitTransform.SendMessage ("SendExitSignals", SendMessageOptions.DontRequireReceiver);
								m_hitTransform = null;
						}
				}
		}
	
		void TouchMoveUpdate ()
		{
		
				if (Input.touchCount == 0)
						return;
		
				for (int i = 0; i < Input.touchCount; i++) {								//Cycle through all of the touches. Assumes a limit of 5 touches can be active on the device

						if (Input.GetTouch (i).phase == TouchPhase.Ended || Input.GetTouch (i).phase == TouchPhase.Canceled) {
								m_isSelected = false;
						}

						if (Input.GetTouch (i).phase == TouchPhase.Moved) {
								Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);
								RaycastHit hit;
								if (Physics.Raycast (ray, out hit, 10f, m_layersToCheck)) {

										if (!m_isSelected && hit.transform.tag == m_buttonTag) {
												hit.transform.SendMessage ("SendEnterSignals", SendMessageOptions.DontRequireReceiver);
												m_isSelected = true;
										}
								}
						}
				}
		}
}