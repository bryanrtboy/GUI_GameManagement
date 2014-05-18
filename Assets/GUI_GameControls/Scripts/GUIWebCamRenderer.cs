using UnityEngine;
using System.Collections;

public class GUIWebCamRenderer : MonoBehaviour
{

#if UNITY_EDITOR || UNITY_WEBPLAYER

		private WebCamTexture webcamTexture;

		// Use this for initialization
		void Start ()
		{
				webcamTexture = new WebCamTexture ();
				renderer.material.mainTexture = webcamTexture;
				webcamTexture.Play ();
	
		}
	
#endif
}
