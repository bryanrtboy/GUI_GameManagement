using UnityEngine;
using System.Collections;

public class PlayMovie : MonoBehaviour
{

		public MovieTexture m_movie;
		public bool			m_loopMovie = true;
		public float 		m_delayBeforePlaying = 0;

		// Use this for initialization
		void Awake ()
		{
				if (m_movie == null) {
						Debug.LogWarning ("No Movie specified! Destroying myself");
						Destroy (this.gameObject);
						return;									//Don't go any further, we have no movie...
				}

				m_movie.loop = m_loopMovie;
				m_movie.Stop ();								//Stop rewinds the movie, so we make sure it's rewound at the start
	
		}
	
		// Update is called once per frame
		void OnEnable ()
		{
				StartCoroutine (StartMovie ());
					
		}

		IEnumerator StartMovie ()
		{
				yield return new WaitForSeconds (m_delayBeforePlaying);
				if (m_movie != null)
						m_movie.Play ();

		}

		void OnDisable ()
		{
				if (m_movie != null)
						m_movie.Stop ();								//If we are turning this game object on or off, let's rewind when turned off
		}
}
