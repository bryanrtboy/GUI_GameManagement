//Author: Bryan Leister
//Date: April, 2014
//
//Description: 
//Placeholder script for a Singleton instance of a GameManager. Each scene in a game should have one central Game Manager
//which takes care of the basic game logic, setup for the game etc. The other example scenes show a more complex manager
//tied to a game of Rock, Paper, Scissors, This is a really simple one, just starting up a splash screen, showing buttons
//and then loading the next level.
//
//Instructions: You should have buttons in the scene that send messages to this script to do stuff. In this case, all we 
//are doing is launching Level 1 of the game. Thus, your Build Settings must have two scenes. Build settings should look
//like this:
//
//GUI_GameControls/Scenes/StartupExample.unity 		-- 0
//GUI_GameControls/Scenes/VirtualUI.unity 			-- 1
//
//That will mean that when the buttons sends us the message "NextLevel",1 we will load the VirtualUI scene.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManagerCustom : MonoBehaviour
{

		public string 				m_nextLevelName = "VirtualUI";			//Set this in the inspector to the number of the next level according to your Build Settings
		public Transform 			m_backgroundToFadeOut;					//Drag a quad here that has a texture of movie as a material
		public float				m_fadeAlphaFrom = 1.0f;					//A value of 1 is Opaque, a value of 0 is transparent
		public float 				m_fadeAlphaTo = 0.0f;					//A value of 1 is Opaque, a value of 0 is transparent
		public float				m_fadeDelay = 0f;						//Delay time before starting fade
		public float 				m_fadeDuration = 2f;					//How long the fade lasts
		public GameObject[] 		m_objectsToHideInstantly;				//When we click exit, hide these before doing any fading out
		public bool 				m_autoLaunchNextLevel = false;			//If we don't have any buttons or anything to send us a StartGame message, we can just launch it automatically
		public float				m_autoLaunchDelay = 6f;					//Amount of time to wait before autolaunching the next level


		void Start ()
		{
				if (m_autoLaunchNextLevel) {
						StartGame ();
				}
		}
		
		IEnumerator AutoLaunch ()
		{
				yield return new WaitForSeconds (m_autoLaunchDelay);
				NextLevel (m_nextLevelName);
		}

		void NextLevel (string level)
		{
				if (m_backgroundToFadeOut != null) {
						Material m = m_backgroundToFadeOut.renderer.material;
			
						if (m != null) {															//Cool, we have a material. Use our Scripts/Helpers/Texture_Extensions to fade
								StartCoroutine (m.FadeMaterialAlpha (m_fadeAlphaTo, m_fadeAlphaFrom, 0, m_fadeDuration, () => {
										Application.LoadLevel (level);									//Once the fade is finished, do this thing
								}));
				
						} else {
								Application.LoadLevel (level);					
						}
				} else {
			
						if (m_autoLaunchNextLevel) {
								Application.LoadLevel (level);									
						}
				}
		}

		void DoSomething ()
		{
				//If something sends us this message, do something cool here..
				//Debug.Log ("I would like to DoSomething, but you haven't told me anything to do!");
		}

		void StartGame ()
		{

				foreach (GameObject g in m_objectsToHideInstantly) {
						if (g != null)
								g.SetActive (false);
				}

				if (m_backgroundToFadeOut != null) {
						Material m = m_backgroundToFadeOut.renderer.material;

						if (m != null) {															//Cool, we have a material. Use our Scripts/Helpers/Texture_Extensions to fade
								StartCoroutine (m.FadeMaterialAlpha (m_fadeAlphaFrom, m_fadeAlphaTo, m_fadeDelay, m_fadeDuration, () => {
										
										if (m_autoLaunchNextLevel) {
												StartCoroutine (AutoLaunch ());
										} else {
												NextLevel (m_nextLevelName);									//Once the fade is finished, do this thing
										}
								}));

						} else {
								Debug.Log ("No material, guess we won't fade. Curious though, why specify a background if we aren't " +
										" planning to fade it out?");

								if (m_autoLaunchNextLevel) {
										StartCoroutine (AutoLaunch ());
								} else {
										NextLevel (m_nextLevelName);									//Once the fade is finished, do this thing
								}					
						}
				} else {

						if (m_autoLaunchNextLevel) {
								StartCoroutine (AutoLaunch ());
						} else {
								NextLevel (m_nextLevelName);									//Once the fade is finished, do this thing
						}
				}
		}


}