//Author: Bryan Leister
//Date: April, 2014
//
//Description: 
//Very basic game inspired by this post: http://forum.unity3d.com/threads/99055-Need-Help-rock-paper-scissors
//
//The GUI stuff is located in the 'UI' script...

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerRPScissors : MonoBehaviour
{


		public GameObject 				m_messagesReceiver;				//Send messages to something that can display text
		public GameObject 				m_scoreReceiver;				//Send the player scores so they can see
		public GameObject[]				m_responseScreens;				//Optional game objects that can be turned on or off as a response to actions in the game
		public GameObject[]				m_countDownTextures;
		public float					m_delayBeforeCountdown = 2f;	//How long to pause before starting to play the count down movie?
		public int 						m_numberOfRoundsInAMatch = 3;
		public GameObject     			m_blackoutScreen;

		private int 					m_player1_Score = 0;
		private int 					m_player2_Score = 0;
		private bool 					m_canPlay = false;				//A trigger to be false while we add up scores, show screens, etc.
		private bool 					m_player1TakesTurn = false;		//Has player 1 played?
		private bool 					m_player2TakesTurn = false;		//Has player 2 played?
		private GamePiece.PlayerChoice 	m_player1Play;					//What is the choice that Player 1 made?
		private GamePiece.PlayerChoice 	m_player2Play;					//What is the choice that Player 2 made?
		private Material				m_blackoutMaterial;				//Cache the blackout material since we use it so much...
		private float  					m_countDownDuration = 1f;

		private bool 					m_isUsingResponseScreens = false;
	

		void Awake ()
		{
				//Need to drag an object in the Editor to this field, it should be able to do something with the messages it gets from this script
				if (m_messagesReceiver == null) {										
						Debug.Log ("Nothing is recieving the " + this.name + " game messages. I'll send them to myself...");		
						m_messagesReceiver = this.gameObject;
				}
				//Need to drag an object in the Editor to this field, it should be able to do something with the messages it gets from this script
				if (m_scoreReceiver == null) {
						Debug.Log ("Nothing to recieve the " + this.name + " scoring messages. I'll send them to myself...");
						m_scoreReceiver = this.gameObject;
				}

				//If we are using a response screen system to show results, turn them all of at the beginning of the game
				if (m_responseScreens.Length > 0) {
						m_isUsingResponseScreens = true;
						foreach (GameObject g in m_responseScreens) {
								if (g != null)
										g.SetActive (false);
						}

						if (m_blackoutScreen != null) {
								m_blackoutScreen.SetActive (true);
								m_blackoutMaterial = m_blackoutScreen.renderer.material;			//Error here means you didn't set the first screen as a blackout screen with a texture
						} else {
								Debug.LogWarning ("No Blackout screen is specified");
						}
				}

				if (m_countDownTextures.Length > 0) {
						foreach (GameObject g in m_responseScreens) {
								if (g != null)
										g.SetActive (false);
						}
						m_countDownDuration = m_countDownTextures.Length;
				}
		}

		void Start ()
		{
				StartMatch ();
		}
	
		public void TakeTurn (GamePiece play)
		{ 
				if (!m_canPlay) {																						//Don't let them do anything if we are showing screens and messages still
						m_messagesReceiver.SendMessage ("MessageIs", "Wait a second..."); 	
						#if UNITY_IPHONE || UNITY_ANDROID
						Handheld.Vibrate ();
						#endif
						return;
				}
				if (play.playerChoice == GamePiece.PlayerChoice.Reset) { 
						StartMatch ();
						ResetScores ();
						return;
				}
				
				switch (play.player) {
				case GamePiece.Player.Player1:
						if (!m_player1TakesTurn) {        																//If Player 1 has not already played, register her play choices
								m_player1Play = play.playerChoice;
								m_player1TakesTurn = true;

								if (m_player2TakesTurn) {																//If Player 2 has played, we can decide who wins
										GetTurnResults ();
								} else {
										#if UNITY_IPHONE || UNITY_ANDROID
										Handheld.Vibrate ();
										#endif
										m_messagesReceiver.SendMessage ("MessageIs", "Waiting for Player 2 to play");   //Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
								}
						} else {
								m_messagesReceiver.SendMessage ("MessageIs", "Wait your turn Player 1!");				//Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
						}
						break;
				case GamePiece.Player.Player2:

						if (!m_player2TakesTurn) {        					
								m_player2Play = play.playerChoice;
								m_player2TakesTurn = true;
				
								if (m_player1TakesTurn) {					
										GetTurnResults ();
								} else {
										#if UNITY_IPHONE || UNITY_ANDROID
										Handheld.Vibrate ();
										#endif
										//ShowResponseScreen (9, false);													//Base this value on what you've set up in the Editor to be turned on in this condition						
										m_messagesReceiver.SendMessage ("MessageIs", "Waiting for Player 1 to play"); 	//Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
								}
				
						} else {
								#if UNITY_IPHONE || UNITY_ANDROID
								Handheld.Vibrate ();
								#endif				
								m_messagesReceiver.SendMessage ("MessageIs", "Wait your turn Player 2!");				//Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
						}
						break;
				case GamePiece.Player.Computer:
					//Do something
						break;
				}
		}

		void GetTurnResults ()
		{
				m_player1TakesTurn = false;
				m_player2TakesTurn = false;
				m_canPlay = false;
		
				if (m_player2Play == m_player1Play) {
						DisplayTurnResults (3);
						#if UNITY_IPHONE || UNITY_ANDROID
						Handheld.Vibrate ();
						#endif
						m_messagesReceiver.SendMessage ("MessageIs", "It's a tie!");
						StartCoroutine (StartCountDownSequence (m_delayBeforeCountdown));
						return;
				}
		
				switch (m_player1Play) {																				//Since both have played, just check against what Player 1 played
				case GamePiece.PlayerChoice.Paper:
			
						if (m_player2Play == GamePiece.PlayerChoice.Rock) {												//We know it's not a tie, already checked that, so check for a winning play
								PlayerOneWins ();
						} else {
								PlayerTwoWins ();	
						}
						break;
				case GamePiece.PlayerChoice.Rock:
						if (m_player2Play == GamePiece.PlayerChoice.Paper) {
								PlayerTwoWins ();
						} else {
								PlayerOneWins ();
						}
						break;
				case GamePiece.PlayerChoice.Scissors:
			
						if (m_player2Play == GamePiece.PlayerChoice.Paper) {
								PlayerOneWins ();
						} else {
								PlayerTwoWins ();
						}
						break;
				}
				
				Vector2 scores = new Vector2 (m_player1_Score, m_player2_Score);
				m_scoreReceiver.SendMessage ("ScoreIs", scores);											//Make sure the m_scoreReceiver object has a function called "ScoreIs" attached to it.
																

		}

		void ResetScores ()
		{
				m_player1_Score = 0;
				m_player2_Score = 0;
				m_scoreReceiver.SendMessage ("ScoreIs", Vector2.zero);										//Reset the scoreboard
		}

		void PlayerOneWins ()
		{
				float delay = m_delayBeforeCountdown;
				FadeInBackground (1f, 1f);
				m_player1_Score++;
				if (m_player1_Score >= m_numberOfRoundsInAMatch) {
						DisplayTurnResults (5);
						m_messagesReceiver.SendMessage ("MessageIs", "Match to Player 1!");
						ResetScores ();
						delay += m_delayBeforeCountdown + 2;
				} else {
						DisplayTurnResults (7);
						m_messagesReceiver.SendMessage ("MessageIs", "Player 1 wins!");
				}
				StartCoroutine (StartCountDownSequence (delay));
		}

		void PlayerTwoWins ()
		{
				float delay = m_delayBeforeCountdown;

				FadeInBackground (1f, 1f);
				m_player2_Score++;
				if (m_player2_Score >= m_numberOfRoundsInAMatch) {
						DisplayTurnResults (6);
						m_messagesReceiver.SendMessage ("MessageIs", "Match to Player 2!");
						ResetScores ();
						delay += m_delayBeforeCountdown + 2;
				} else {
						DisplayTurnResults (8);
						m_messagesReceiver.SendMessage ("MessageIs", "Player 2 wins!");
				}
				StartCoroutine (StartCountDownSequence (delay));
		}

		void DisplayTurnResults (int screenNumber)
		{
				if (!m_isUsingResponseScreens)																//Quick out if we aren't using the response screens
						return;

				if (m_responseScreens [screenNumber] == null) {
						Debug.LogWarning ("There is no screen in " + screenNumber + " position set up. " +
								"Check the Editor that the " 
								+ this.name + " Response Screens are not empty." +
								"I will stop trying to work with the Response screens now...");
						return;																				//Bail, apparently somebody didn't set this up in the Editor correctly.
				}

				Material z = m_responseScreens [screenNumber].renderer.material;
				z.color = new Color (z.color.r, z.color.g, z.color.b, 1);									//Make sure the response screen is Opaque since we're animating it in
				m_responseScreens [screenNumber].SetActive (true);											//No matter what, let's make it active.

				float delay = m_delayBeforeCountdown / 2;
				if (screenNumber == 5 || screenNumber == 6) {												//We have a match winning situation
						delay = m_delayBeforeCountdown * 2;													//Delay twice as long as normal
				}
				StartCoroutine (z.FadeMaterialAlpha (1f, 0f, delay, 1f, () => 
				{
						m_responseScreens [screenNumber].SetActive (false);
				}));
		}

		void StartMatch ()
		{
				if (m_blackoutScreen != null) {
			
						m_blackoutMaterial.color = new Color (m_blackoutMaterial.color.r, m_blackoutMaterial.color.g, m_blackoutMaterial.color.b, 1);
						m_blackoutScreen.SetActive (true);														//Activate the blackout screen
				}

				StartCoroutine (StartCountDownSequence (m_delayBeforeCountdown));							//start the game turn sequence	
		}

		IEnumerator StartCountDownSequence (float delayBeforeStartingMovie)
		{
				yield return new WaitForSeconds (delayBeforeStartingMovie);									//Savor the victory time, whatever sceen is up, keep it up for this time
				m_messagesReceiver.SendMessage ("MessageIs", "Get Ready...");								//Send a message to another script or to the log
				StartCoroutine (StartCountdownSequence ());
				FadeOutBackground (0, .5f);
				m_messagesReceiver.SendMessage ("MessageIs", "Waiting for " + m_countDownTextures.Length + " seconds.");

				yield return new WaitForSeconds (m_countDownDuration);										//Length of Time to wait for the movie to finish
				
				m_messagesReceiver.SendMessage ("MessageIs", "Shoot!");
				//DisplayTurnResults (4);																//Make your play screen, don't check scores 

				m_player1TakesTurn = false;
				m_player2TakesTurn = false;
				m_canPlay = true;

		}

		void FadeInBackground (float delay, float duration)
		{
				if (m_blackoutScreen != null) {
						m_blackoutMaterial.color = new Color (m_blackoutMaterial.color.r, m_blackoutMaterial.color.g, m_blackoutMaterial.color.b, 0);
						m_blackoutScreen.SetActive (true);														//Activate the blackout screen
		
						StartCoroutine (m_blackoutMaterial.FadeMaterialAlpha (0, 1f, delay, duration, () => 
						{
								//Do something once the screen has faded in
						}));
				}

		}

		void FadeOutBackground (float delay, float duration)
		{
				if (m_blackoutScreen != null) {
						m_blackoutMaterial.color = new Color (m_blackoutMaterial.color.r, m_blackoutMaterial.color.g, m_blackoutMaterial.color.b, 1);
						m_blackoutScreen.SetActive (true);														//Activate the blackout screen
		
						StartCoroutine (m_blackoutMaterial.FadeMaterialAlpha (1f, 0f, delay, duration, () => 
						{
								m_blackoutScreen.SetActive (false);
						}));
				}

		}

		void MessageIs (string message)																		//This function only gets called if nothing was assigned to get these messages in the Editor
		{
				Debug.Log (message);
		}

		void ScoreIs (Vector2 scores)																		//This function only gets called if nothing was assigned to get the score messages in the Editor
		{
				Debug.Log ("Player 1 = " + scores.x + "\nPlayer 2 = " + scores.y);
		}

		IEnumerator StartCountdownSequence ()
		{
				if (m_countDownTextures.Length < 1)
						yield break;

				int finished = m_countDownTextures.Length;

				for (int i = 0; i < finished; i++) {
						m_countDownTextures [i].SetActive (true);			//get ready
						yield return new WaitForSeconds (1f);
						m_countDownTextures [i].SetActive (false);			//5
				}


		}
}