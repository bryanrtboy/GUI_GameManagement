using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class TextMessageReciever : MonoBehaviour
{
		
		private TextMesh m_textMesh;

		void Awake ()
		{
				m_textMesh = this.GetComponent<TextMesh> () as TextMesh;
		}

		void MessageIs (string message)
		{
				m_textMesh.text = message;
		}
	
		void ScoreIs (Vector2 scores)
		{
				string message = "Player 1 = " + scores.x + "\nPlayer 2 = " + scores.y;
				m_textMesh.text = message;
		}
}
