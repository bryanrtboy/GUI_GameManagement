//TriggerOnPresence adapted to C# from Angrybots project
//Author: Bryan Leister
//Date: June 2012
//
//Description: This script is used to trigger door openings. It keeps track of who triggered the door, and
//when that thing leaves it closes the door.
//
//Instructions: This is attached to the a gameObject with a Collider attached to it, set to Trigger. When
//a player tagged "Player" hit this, it will send the signals specifed.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerOnPresence : MonoBehaviour
{
	
		public string 				m_triggerTag = "Player";			//Check if the thing that hit us is tagged with this, or do nothing
		public bool 				m_lookingForTriggers = true;		//Is the thing meant to hit us a Trigger or a Collider? 
		public bool 				m_debugMode = false;				//If the button is not sending messages, turn this on to get more information about collisions
		public SignalSender 		m_enterSignals;
		public SignalSender 		m_enterOnceSignals;
		public SignalSender 		m_exitSignals ;
		private List<GameObject> 	m_objects;

		void Awake ()
		{
				m_objects = new List<GameObject> ();
				enabled = false;
		}

		void OnTriggerEnter (Collider other)
		{
				if (m_debugMode) {
						string triggerType;
						if (other.isTrigger) {
								triggerType = " a trigger";
						} else {
								triggerType = " a collider";
						}
						Debug.Log (this.name + " got hit by " + other.name + ". " + other.name + " collider is set to " + triggerType);
						if (m_lookingForTriggers) {
								Debug.Log (this.name + " is looking for a trigger to hit it.");
						} else {
								Debug.Log (this.name + " is looking for a collider to hit it.");
						}
				}
			
				if (m_lookingForTriggers && !other.isTrigger)											//Other object needs to be a trigger, this allows us to make the collider larger than the physics collider
						return;

				if (other.tag == m_triggerTag) {
						bool wasEmpty = (m_objects.Count == 0);					//if there are no objects, wasEmpty is true, otherwise false;
		
						m_objects.Add (other.gameObject);
		
						if (wasEmpty) {
								if (m_debugMode) {
										Debug.Log (this.name + " is SendingSignals to " + other.name);
								}
								m_enterSignals.SendSignals (this);
								m_enterOnceSignals.SendSignals (this);
								enabled = true;
						}
				}
		}

		void OnTriggerExit (Collider other)
		{
				if (!other.isTrigger)
						return;
	
				if (m_objects.Contains (other.gameObject))
						m_objects.Remove (other.gameObject);
	
				if (m_objects.Count == 0) {
						m_exitSignals.SendSignals (this);
						enabled = false;
				}
		}

		void SendEnterSignals ()
		{
				//Debug.Log ("Sending Enter Signals");
				m_enterSignals.SendSignals (this);
		}

		void SendExitSignals ()
		{
				//Debug.Log ("Sending Exit Signals");
				m_exitSignals.SendSignals (this);
		}
	

}
