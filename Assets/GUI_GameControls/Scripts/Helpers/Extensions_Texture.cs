//Audio Extensions
//Author: Bryan Leister
//Date: Apr 2014
//Description: Helper class to automate fading of GUI Textures and material colors using the Alpha
//The material needs to be a transparent material for the fade to show up.
//
//Usage - Start a coroutine like below. Null is passed if you want nothing to happen after the coroutine is finished:
//
//			StartCoroutine (myGUItexture.FadeGUITexture (1.0f, 0.0f, 1.0f, 2.0f, null));
//
//If you want stuff to happen after complete, pass it like this:
//
//			StartCoroutine (myGUItexture.FadeGUITexture (1.0f, 0.0f, 1.0f, 2.0f, () =>
//			{
//				isGuiVisible = false;
//				DoSomeCoolNewThing ();
//			}
//			));


using UnityEngine;
using System;
using System.Collections;

public static class Extensions_Texture
{

		public static IEnumerator FadeGUITexture (this GUITexture myGUItexture, float startLevel, float endLevel, float delay, float duration, Action onComplete)
		{
				yield return new WaitForSeconds (delay);
		
				float speed = 1.0f / duration;   
		
				for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {
			
						float f = Mathf.Lerp (startLevel, endLevel, t);
						myGUItexture.color = new Color (myGUItexture.color.r, myGUItexture.color.g, myGUItexture.color.b, f);
						yield return null;
			
				}

				onComplete ();
		
		}

		public static IEnumerator FadeMaterialAlpha (this Material materialToFade, float startLevel, float endLevel, float delay, float duration, Action onComplete)
		{
				yield return new WaitForSeconds (delay);
		
				float speed = 1.0f / duration;   

				for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {
			
						float f = Mathf.Lerp (startLevel, endLevel, t);
						materialToFade.color = new Color (materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, f);
						yield return null;
			
				}

				onComplete ();
		
		}
}
