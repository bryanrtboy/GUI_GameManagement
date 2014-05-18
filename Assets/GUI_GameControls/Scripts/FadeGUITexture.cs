using UnityEngine;
using System.Collections;


[RequireComponent (typeof(GUITexture))]
public class FadeGUITexture : MonoBehaviour
{

		public float 		m_fadeTime = 2f;
		public float 		m_delay = 0;
		public bool 		m_autoDestroy = true;		//Die after fading in or out?
		public bool 		m_fullScreen = true;		//Scales the GUITexture to fill the screen, if false it will be left as it is in the Editor
		public float 		m_fadeFrom = 0;				//A value of 0 means we start at Transparent Alpha = 0
		public float 		m_fadeTo = 1;				//A value of 1 means we end at Opaque Alpha = 1
		public GameObject	m_messageReciever;		//If we drag a gameObject in here, we will send it the message once the fade is complete
		public string		m_message;				//This should be a function in the recieving object, i.e. m_messagerReciever.SendMessage("MyFunction");

		private GUITexture 	m_guiTexture;
		

		void Awake ()
		{
				m_guiTexture = this.GetComponent<GUITexture> () as GUITexture;

				if (m_fullScreen) {
						m_guiTexture.transform.position = new Vector3 (.5f, .5f, 0);
						m_guiTexture.transform.localScale = new Vector3 (1, 1, 1);
				}

		}

		void OnEnable ()
		{

				m_guiTexture.color = new Color (m_guiTexture.color.r, m_guiTexture.color.g, m_guiTexture.color.b, m_fadeFrom);
		
				StartCoroutine (m_guiTexture.FadeGUITexture (m_fadeFrom, m_fadeTo, m_delay, m_fadeTime, () =>
				{
						if (m_autoDestroy)
								Destroy (this.gameObject);
			
						if (m_messageReciever != null)
								m_messageReciever.SendMessage (m_message, SendMessageOptions.DontRequireReceiver);
				}));

		}
	

}
