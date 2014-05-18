using UnityEngine;
using System.Collections;

public class GUIWebCamRenderer : MonoBehaviour
{

		private WebCamTexture webcamTexture;

		// Use this for initialization
		void Start ()
		{
				webcamTexture = new WebCamTexture ();
				renderer.material.mainTexture = webcamTexture;
				webcamTexture.Play ();
	
		}
	
		// Update is called once per frame
		void Update ()
		{

	
		}
}
