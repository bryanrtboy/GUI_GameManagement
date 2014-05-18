//Author: Bryan Leister
//Date: April 2014
//Description:
//
//A multipurpose button script for creating buttons in the scene as their own objects. This is meant to work with
//OnTriggerPresence or some other script that sends this script a message.  We then animate, or start an indication
//sequence, which is useful for creating Kinect-like buttons.
//
//Instructions:
//Place this button on an empty game object at the top of a hierarchy of other objects. The other objects should be
//organized about like this:
//
//	-Button (required and must have this script attached)
//		--Animated Object (optional & attach Animator to this empty game object)
//			---Visual Object (required if you are Animating anything, this will move with it's Parent's animation)
//		--IndicatorRing (optional)
//			---Background (optional)
//		--Label (optional)
//		--Trigger (required & should have TriggerOnPresence attached, set up to send signals to this object)
//
//Note: The Button Prefab demo is using a 3D Text Mesh, you can use an image instead. The 3D Text mesh renderer
//has a bug that makes the text disappear in some instances (Z-fighting is my guess), so I have swapped out the
//default Font Material and used a 3DFontMaterial to fix this problem. To fix this if your TextMesh has the problem
//go to the Text Meshes material, replace it with the 3DFontMaterial. Then, open up your Font and drag it's Font
//Texture to the texture area of the 3DFontMaterial.
//
//Or, use an image instead of a TextMesh for your label
//

using UnityEngine;
using System.Collections;

public class ButtonBehaviour : MonoBehaviour
{
		public GameObject	m_messageReciever;						//required - Send our button messages to this object, if null will send messages to ourself
		public string 		m_messageToSend = "StartGame";			//required - This is the message that will be sent to the Game manager when this button is triggered.
		public Animator 	m_objectAnimator;						//optional - If we want an animation when triggered, drag an Animator here
		public float 		m_hoverTimeUntilSelected = 2;			//how long to wait when hovered over (used when we have a selection indicator
		public Transform 	m_selectionIndicator;					//object with a material that has a Cutoff property, used to show elapsed time
	
		private bool 		m_startSelectionCountdown = false;
		private Material 	m_indicatorRingMaterial;
		private float 		m_countDown;
		private float 		m_repeatRate = .05f;
		private bool 		m_useAnimation = false;
		private bool 		m_useSelectionIndicator = false;


		void Awake ()
		{
				if (m_objectAnimator == null) {
						m_objectAnimator = this.GetComponentInChildren<Animator> () as Animator;       	//check for animation in the children of the Button

						if (m_objectAnimator != null) {													//only use animation if we found one.
								m_useAnimation = true;
						}
				} else {
						m_useAnimation = true;															//they specified an animator, let's use it!
				}

				if (m_selectionIndicator != null) {
						m_indicatorRingMaterial = m_selectionIndicator.renderer.material;
						m_indicatorRingMaterial.SetFloat ("_Cutoff", 1f);
						m_useSelectionIndicator = true;
				}

				if (m_messageReciever == null) {
						Debug.Log ("We don't have a message receiver for " + this.name + ", I'll just send messages to myself then...");
						m_messageReciever = this.gameObject;
				}

		}

		void Start ()
		{
				if (m_useAnimation) {
						m_objectAnimator.SetTrigger ("rotate");									//we are assuming the Animator has a trigger called 'rotate'
						//m_objectAnimator.SetTrigger ("donothing");						//we can use this to stopanimating if the Animator has a trigger called 'donothing'

				}
			
		}
		
		void GetExcited ()
		{
				if (m_startSelectionCountdown)          				//If we are in a selection state, don't do anything else until we're done checking...
						return;

				if (m_useAnimation)
						m_objectAnimator.SetTrigger ("bounce");		//if we are using an Animator, send a message to it to start the animation..

				if (m_useSelectionIndicator) {
						m_startSelectionCountdown = true;			//We won't want to repeat this, so now we are in a selection state.

						StartCoroutine (CheckSelectionState (m_hoverTimeUntilSelected));
				}
				
		}

		void SettleDown ()
		{
				StopAllCoroutines ();

				if (m_selectionIndicator) {
						m_indicatorRingMaterial.SetFloat ("_Cutoff", 1f);		//This requires that the indicatorRingMaterial has a _Cutoff property
						CancelInvoke ();
				}

				m_startSelectionCountdown = false;
		}

		IEnumerator CheckSelectionState (float delay)
		{
				if (m_selectionIndicator != null) {
						m_indicatorRingMaterial.SetFloat ("_Cutoff", 1f);
						m_countDown = Time.time + delay;
						InvokeRepeating ("ShowProgress", 0, m_repeatRate);
				}

				yield return new WaitForSeconds (delay);

				if (m_selectionIndicator != null) {
						m_indicatorRingMaterial.SetFloat ("_Cutoff", -0.1f);
						CancelInvoke ("ShowProgress");
				}

				m_messageReciever.SendMessage (m_messageToSend, SendMessageOptions.DontRequireReceiver);				//Send a message to the other script attached to this game piece, that will tell it to make a play
				m_startSelectionCountdown = false;
		}

		void ShowProgress ()
		{
				float remainingTime = m_countDown - Time.time;
				remainingTime = remainingTime.Remap (0f, m_hoverTimeUntilSelected, 0f, 1f);

				if (remainingTime < 0)
						remainingTime = 0;

				m_indicatorRingMaterial.SetFloat ("_Cutoff", remainingTime);


		}
}
