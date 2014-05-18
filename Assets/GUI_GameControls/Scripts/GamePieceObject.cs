//Author: Bryan Leister
//Date: April, 2014
//Description:
//
//Attach this script to an object in the Editor that is turned on and off (Enabled) during the game. Set the type of object,
//And which player is turning it on or off. Alternately, you can create in your script, a direct call by including all of this
//in another script that can call the public function MakeAPlay();

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePieceObject : MonoBehaviour
{

		public GameObject					m_gameManager;									//The central GameManager to track plays, scores, etc.
		public GamePiece.Player 			m_player = GamePiece.Player.Player1;			//Which player's object is this?
		public GamePiece.PlayerChoice 		m_choice = GamePiece.PlayerChoice.Paper; 		//What kind of object am I?
		public bool 						m_makeAPlayOnEnabled = false;

		private GamePiece 					m_play;
	
		void Awake ()
		{
				m_play = new GamePiece ();													//Make ourselves into a compact object to send to the GameManager
				m_play.player = m_player;
				m_play.playerChoice = m_choice;

				if (m_gameManager == null) {
						Debug.LogWarning ("We don't have a game manager for " + this.name + ", not sure where to send" +
								" messages... I guess I'll send messages to myself.");
						m_gameManager = this.gameObject;
				}
		}

		void OnEnable ()
		{
				if (m_makeAPlayOnEnabled) {
						MakeAPlay ();
				}
		}

		public void MakeAPlay ()
		{
				m_gameManager.SendMessage ("TakeTurn", m_play);

		}

		void TakeTurn (GamePiece play)
		{
		
				Debug.Log (this.name + " talking here. No game manager object specified, so not keeping score. But, just so you know -\n"
						+ play.player.ToString () + " chose the " + play.playerChoice.ToString ());
		
		}

}
