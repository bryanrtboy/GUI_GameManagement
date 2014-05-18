//Send Signals or groups of Signals
//Author: Bryan Leister (based off AngryBots demo scene)
//Date: Jan 2013
//
//Description: A static method that can be used from any object in game to send messages.
//
//Instructions: In whatever script you want, set up a variable that will send the messages. Such as:
//
//	public GSK_SignalSender 	damageSignals;
//	public GSK_SignalSender 	dieSignals;
//	public GSK_SignalSender 	awardSignals;
//
//In the inspector, set up how many signals and to whom to send them to.
//
//Then, in your script, send the messages when needed by doing :
//	damageSignals.SendSignals (this);
//


using UnityEngine;
using System.Collections;

[System.Serializable]
public class ReceiverItem
{
		public GameObject receiver;
		public string action = "OnSignal";
		public float delay ;
	
		public IEnumerator SendWithDelay (MonoBehaviour sender)
		{
				yield return new WaitForSeconds (delay);
				if (receiver)
						receiver.SendMessage (action);
				else
						Debug.LogWarning ("No receiver of signal \"" + action + "\" on object " + sender.name + " (" + sender.GetType ().Name + ")", sender);
		}
}

[System.Serializable]
public class SignalSender
{
		public bool onlyOnce;
		public ReceiverItem[] receivers;
	
		private bool hasFired = false;
	
		public void SendSignals (MonoBehaviour sender)
		{
				if (hasFired == false || onlyOnce == false) {
						for (int i = 0; i < receivers.Length; i++) {
								//Debug.Log ("Sender is " + sender.name + "; Reciver is " + receivers[i].receiver.name);
								sender.StartCoroutine (receivers [i].SendWithDelay (sender));
						}
						hasFired = true;
				}
		}
}

