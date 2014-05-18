using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GamePiece
{

		public enum PlayerChoice
		{
				Rock=0,
				Paper=1,
				Scissors=2,
				Reset = 3
		}
	
		public enum Player
		{
				Computer = 0,
				Player1 = 1,
				Player2 = 2
		}


		public  PlayerChoice playerChoice = PlayerChoice.Rock;
		public  Player player = Player.Player1;
	

}
