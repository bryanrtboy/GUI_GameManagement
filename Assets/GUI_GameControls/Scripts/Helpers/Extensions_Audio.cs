//Audio Extensions
//Author: Bryan Leister
//Date: Jan 2013
//
//
//Description: A static method for doing audio things. Does not have to be in the scene hierarchy
//
//Instructions:
//Simple play an audioclip on this object would be:
//
// _audioSource.playClip(_audioClip);
//
//
//PlayClipOnComplete will do something after the clip has played. On your object with the sound, use a script like this:
//
//		StartCoroutine(_audioSource.playClipAndOnComplete( chaseWarning, () =>
//		{
//			DoSomeCoolNewThing();
//		}
//		));
//
//
//You can also pass a null if you want by doing this:
//StartCoroutine(_audioSource.playClipAndOnComplete( chaseWarning, null));
//
	
using UnityEngine;
using System;
using System.Collections;
//using System.Collections.Generic;


public static class Extensions_Audio
{
	
		public static void playClip (this AudioSource audioSource, AudioClip audioClip)
		{
				audioSource.clip = audioClip;
				audioSource.Play ();
		}
	
		public static IEnumerator playClipAndOnComplete (this AudioSource audioSource, AudioClip audioClip, Action onComplete)
		{
				audioSource.playClip (audioClip);
		
				while (audioSource.isPlaying)
						yield return null;
		
				onComplete ();
		
		}
	
		public static IEnumerator playRandomClip (this AudioSource audioSource, AudioClip[] clips, Action onComplete)
		{
				//audioSource.Stop();
		
				int clipIndex = UnityEngine.Random.Range (0, clips.Length);
				audioSource.playClip (clips [clipIndex]);
		
				while (audioSource.isPlaying)
						yield return null;
		
				if (onComplete != null)
						onComplete ();
		
		}
	
		public static IEnumerator fadeOut (this AudioSource audioSource, AudioClip audioClip, float duration, Action onComplete)
		{
				audioSource.playClip (audioClip);
				var startingVolume = audioSource.volume;
		
				//fade out over time
				while (audioSource.volume > 0.0F) {
						audioSource.volume -= Time.deltaTime * startingVolume / duration;
						yield return null;
				}

				audioSource.Stop ();
				audioSource.volume = 1;
				//done fading out
				if (onComplete != null)
						onComplete ();
				
		}
	
		public static IEnumerator fadeIn (this AudioSource audioSource, AudioClip audioClip, float duration, Action onComplete)
		{
		
				var endingVolume = audioSource.volume;
				audioSource.volume = 0.0F;
		
				audioSource.playClip (audioClip);
		
				//fade in over time
				while (audioSource.volume < endingVolume) {
						audioSource.volume += Time.deltaTime * endingVolume / duration;
						yield return null;
				}
		
				//done fading out
				if (onComplete != null)
						onComplete ();
		}
	
		public static float Remap (this float value, float from1, float to1, float from2, float to2)
		{
		
				return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		
		}

		//Debug.Log(2.Remap(1, 3, 0, 10));    // 5
	
}
