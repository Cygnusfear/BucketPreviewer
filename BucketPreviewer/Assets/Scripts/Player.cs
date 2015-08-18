using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	List<Texture> queue = new List<Texture>();
	public MeshRenderer renderer;
	public float frameRate = 25;
	float frameDuration {
		get {
			return 1/frameRate;
		}
	}

	public bool playing;

	public void Play()
	{
		StopCoroutine("PlaySequence");
		StartCoroutine(PlaySequence());
	}

	public void Load(List<Texture> newQueue)
	{
		queue = newQueue;
	}

	IEnumerator PlaySequence()
	{
		Debug.Log("Playing sequence ("+queue.Count+" frames @"+ frameRate +"fps)");
		foreach(var t in queue)
		{
			renderer.material.mainTexture = t;
			renderer.material.SetTexture("_EmissionMap",t);
			yield return new WaitForSeconds(frameDuration);
		}
		StartCoroutine(PlaySequence());
	}

}
