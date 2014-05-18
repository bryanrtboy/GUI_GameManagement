using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Material))]
public class FadeTexture : MonoBehaviour
{

		public float 		m_fadeTime = 2f;
		public float 		m_delay = 0;
		public bool 		m_autoDestroy = true;		//Die after fading in or out?
		public float 		m_fadeFrom = 0;				//A value of 0 means we start at Transparent Alpha = 0
		public float 		m_fadeTo = 1;				//A value of 1 means we end at Opaque Alpha = 1
		public GameObject	m_messageReciever;		//If we drag a gameObject in here, we will send it the message once the fade is complete
		public string		m_message;				//This should be a function in the recieving object, i.e. m_messagerReciever.SendMessage("MyFunction");

		private Material 	m_material;
		

		void Awake ()
		{
				m_material = this.renderer.material;

		}

		void OnEnable ()
		{
				m_material.color = new Color (m_material.color.r, m_material.color.g, m_material.color.b, m_fadeFrom);
		
				StartCoroutine (m_material.FadeMaterialAlpha (m_fadeFrom, m_fadeTo, m_delay, m_fadeTime, () =>
				{
						if (m_autoDestroy)
								Destroy (this.gameObject);
			
						if (m_messageReciever != null)
								m_messageReciever.SendMessage (m_message);
				}));

		}

}
