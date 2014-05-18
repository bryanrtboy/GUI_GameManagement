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
		public GameObject				m_countDownMovie;				//Use a short movie to countdown for the Players, when it's finished playing, the Shoot screen should show up.
		public float					m_delayBeforeCountdown = 2f;	//How long to pause before starting to play the count down movie?

		private int 					m_player1_Score = 0;
		private int 					m_player2_Score = 0;
		private bool 					m_canPlay = true;				//A trigger to be false while we add up scores, show screens, etc.
		private bool 					m_player1TakesTurn = false;		//Has player 1 played?
		private bool 					m_player2TakesTurn = false;		//Has player 2 played?
		private GamePiece.PlayerChoice 	m_player1Play;					//What is the choice that Player 1 made?
		private GamePiece.PlayerChoice 	m_player2Play;					//What is the choice that Player 2 made?

		private bool 					m_isUsingResponseScreens = false;
		private GameObject				m_currentScreen = null;
	

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
				}

				//Turn off the countdownmovie if it exists
				if (m_countDownMovie != null) {
						m_countDownMovie.SetActive (false);
				}
		}

		void Start ()
		{
				StartCoroutine (StartGame (31f));
		}
	
		public void TakeTurn (GamePiece play)
		{ 
				if (!m_canPlay) {																						//Don't let them do anything if we are showing screens and messages still
						m_messagesReceiver.SendMessage ("MessageIs", "Wait a second..."); 	
#if UNITY_IOS || UNITY_ANDROID
						Handheld.Vibrate ();
#endif
						return;
				}


				if (play.playerChoice == GamePiece.PlayerChoice.Reset) { 
						ShowResponseScreen (2, true);
						StartCoroutine (ResetGame (2f, 1f));
						return;
						;
						//Don't do anything else, somebody hit Reset
				}
				
				switch (play.player) {
				case GamePiece.Player.Player1:
						if (!m_player1TakesTurn) {        																//If Player 1 has not already played, register her play choices
								m_player1Play = play.playerChoice;
								m_player1TakesTurn = true;

								if (m_player2TakesTurn) {																//If Player 2 has played, we can decide who wins
										CheckWhoWins ();
								} else {

										ShowResponseScreen (10, false);														//Base this value on what you've set up in the Editor to be turned on in this condition						

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
										CheckWhoWins ();
								} else {

										ShowResponseScreen (9, false);													//Base this value on what you've set up in the Editor to be turned on in this condition						

										m_messagesReceiver.SendMessage ("MessageIs", "Waiting for Player 1 to play"); 	//Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
								}
				
						} else {
								m_messagesReceiver.SendMessage ("MessageIs", "Wait your turn Player 2!");				//Make sure the m_messageReceiver object has a function called "MessageIs" attached to it.
						}
						break;
				case GamePiece.Player.Computer:
					//Do something
						break;
				}
		}

		void CheckWhoWins ()
		{
				m_player1TakesTurn = false;
				m_player2TakesTurn = false;
				m_canPlay = false;
		
				if (m_player2Play == m_player1Play) {
						ShowResponseScreen (3, true);
#if UNITY_IOS || UNITY_ANDROID
						Handheld.Vibrate ();
#endif
						StartCoroutine (ResetGame (2f, 1f));
						m_messagesReceiver.SendMessage ("MessageIs", "It's a tie!");
						return;
				}
		
				switch (m_player1Play) {																				//Since both have played, just check against what Player 1 played
				case GamePiece.PlayerChoice.Paper:
			
						if (m_player2Play == GamePiece.PlayerChoice.Rock) {												//We know it's not a tie, already checked that, so check for a winning play
								ShowResponseScreen (7, true);			
								m_messagesReceiver.SendMessage ("MessageIs", "Player 1 wins!");
								m_player1_Score++;
								
						} else {																						//Player 1 must be a loser
								ShowResponseScreen (8, true);				
								m_messagesReceiver.SendMessage ("MessageIs", "Player 2 wins!");
								m_player2_Score++;	
						}
						break;
				case GamePiece.PlayerChoice.Rock:
						if (m_player2Play == GamePiece.PlayerChoice.Paper) {
								ShowResponseScreen (8, true);				
								m_messagesReceiver.SendMessage ("MessageIs", "Player 2 wins!");
								m_player2_Score++;
						} else {
								
								ShowResponseScreen (7, true);
								m_messagesReceiver.SendMessage ("MessageIs", "Player 1 wins!");
								m_player1_Score++;	
						}
						break;
				case GamePiece.PlayerChoice.Scissors:
			
						if (m_player2Play == GamePiece.PlayerChoice.Paper) {
								ShowResponseScreen (7, true);
								m_messagesReceiver.SendMessage ("MessageIs", "Player 1 wins!");
								m_player1_Score++;		
						} else {
								
								ShowResponseScreen (8, true);
								m_messagesReceiver.SendMessage ("MessageIs", "Player 2 wins!");
								m_player2_Score++;		
						}
						break;
				}
				
				Vector2 scores = new Vector2 (m_player1_Score, m_player2_Score);
				m_scoreReceiver.SendMessage ("ScoreIs", scores);											//Make sure the m_scoreReceiver object has a function called "ScoreIs" attached to it.
																
				CheckMatchStatus ();

		}

		void CheckMatchStatus ()
		{

				if (m_player1_Score >= 3) {

						m_player1_Score = 0;
						m_player2_Score = 0;
						m_messagesReceiver.SendMessage ("MessageIs", "Match to Player 1!");
						m_scoreReceiver.SendMessage ("ScoreIs", Vector2.zero);								//Reset the scoreboard
						ShowResponseScreen (5, true);
						//Reset the game, first number is to read the results, then a countdown movie shows up
						//Second number should be the length of the countdownMovie
						StartCoroutine (ResetGame (4f, 1f));
						return;
				} else if (m_player2_Score >= 3) {

						m_player1_Score = 0;
						m_player2_Score = 0;
						m_scoreReceiver.SendMessage ("ScoreIs", Vector2.zero);								//Reset the scoreboard
						m_messagesReceiver.SendMessage ("MessageIs", "Match to Player 2!");
						ShowResponseScreen (6, true);
						//Reset the game, first number is to read the results, then a countdown movie shows up
						//Second number should be the length of the countdownMovie
						StartCoroutine (ResetGame (4f, 1f));
						return;
				} 

				StartCoroutine (ResetGame (2f, 1f));	

		}

		void ShowResponseScreen (int screenNumber, bool blackBackground)
		{
				if (!m_isUsingResponseScreens)																//Quick out if we aren't using the response screens
						return;


				if (m_currentScreen != null) {																//If we are using them, check if we've got an active screen

						m_currentScreen.SetActive (false);											//No animator, so turning it off instantly
				}

				if (m_responseScreens [screenNumber] == null) {
						Debug.LogWarning ("There is no screen in " + screenNumber + " position set up. Check the Editor that the " + this.name + " Response Screens are not empty." +
								"I will stop trying to work with the Response screens now...");
						return;																				//Bail, apparently somebody didn't set this up in the Editor correctly.
				}

				m_responseScreens [screenNumber].SetActive (true);											//No matter what, let's make it active.

				m_currentScreen = m_responseScreens [screenNumber];											//We now are storing it as it's the current screen

				if (blackBackground) {
						m_responseScreens [0].SetActive (true);												//A black background, set to Fade In when it's activated.
				}

		}

		IEnumerator StartGame (float delay)
		{

				m_messagesReceiver.SendMessage ("MessageIs", "Welcome.");									//Send a message to another script or to the log
				yield return new WaitForSeconds (delay);

				StartCoroutine (ResetGame (m_delayBeforeCountdown, 2f));									//start the game turn sequence

		}

		IEnumerator ResetGame (float delayBeforeStartingMovie, float countDownLength)
		{
				
				yield return new WaitForSeconds (delayBeforeStartingMovie);									//Savor the victory time, whatever sceen is up, keep it up for this time
				m_messagesReceiver.SendMessage ("MessageIs", "Get Ready...");								//Send a message to another script or to the log
				if (m_countDownMovie != null) {
						
						PlayMovie moviePlayer = m_countDownMovie.GetComponent<PlayMovie> () as PlayMovie;
						if (moviePlayer != null && moviePlayer.m_movie != null) {
								moviePlayer.m_movie.Stop ();												//Rewind the movie
								countDownLength = moviePlayer.m_movie.duration;								//Set the next delay to the length of the movie
						}

						m_countDownMovie.SetActive (true);
				}

				yield return new WaitForSeconds (countDownLength);											//Length of Time to wait for the movie to finish
				
				if (m_countDownMovie != null) {
						m_countDownMovie.SetActive (false);
				}
				m_messagesReceiver.SendMessage ("MessageIs", "Shoot!");
				ShowResponseScreen (4, true);																//Make your play screen

				if (m_isUsingResponseScreens) {
						//m_responseScreens [1].SetActive (true);											//Turn on the Fade out Screen, it will automatically start to fade out
						m_responseScreens [0].SetActive (false);											//Turn off the FadeInScreen
				}
				m_player1TakesTurn = false;
				m_player2TakesTurn = false;
				m_canPlay = true;

		}

		void MessageIs (string message)																		//This function only gets called if nothing was assigned to get these messages in the Editor
		{
				Debug.Log (message);
		}

		void ScoreIs (Vector2 scores)																		//This function only gets called if nothing was assigned to get the score messages in the Editor
		{
				Debug.Log ("Player 1 = " + scores.x + "\nPlayer 2 = " + scores.y);
		}
}