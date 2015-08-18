using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Browser))]
public class LoadTexture : MonoBehaviour {

	public Texture2D texture;
	public MeshRenderer renderer;
	public float frameRate;
	float frameDuration {
		get {
			return 1/frameRate;
		}
	}


	public void Load(string[] paths)
	{
		//check all paths

		if (paths.Length > 1)
		{
			if (IsImage(paths[0]) && IsSequence(paths))
			{
				Debug.Log("Image sequence");
				StartCoroutine(LoadSequence(paths));
			}
		}
		else
		{
			foreach(var path in paths)
			{
				//check extension
				Debug.Log(path);

				//if video load video to videotexture
				if (IsVideo(path))
				{
					Debug.Log("Is Video");
					StartCoroutine(SetMovieTexture(path));
				}
				//if image sequence load image sequence
				else if (IsImage(path))
				{

					Debug.Log("Is Image");
					StartCoroutine(FetchTexture(path));
				}
				//if single texture simply load as texture
			}
		}
	}

	bool IsSequence(string[] paths)
	{
		foreach(var path in paths)
		{
			int partOfSequence = IsPartOfSequence(path);
			if (partOfSequence < 0)
			{
				return false;
			}
		}
		return true;
	}

	int IsPartOfSequence(string path)
	{
		Regex regex = new Regex(@"\d+");
		Match match = regex.Match("[0-9][0-9][0-9]");
		if (match.Success)
		{
			return 1;
		}
		return -1;
	}

	bool IsImage(string path)
	{
		foreach(var extension in GetComponent<Browser>().extensions)
		{
			if (path.Contains(extension)) return true;
		}
		return false;
	}

	bool IsVideo(string path)
	{
		foreach(var extension in GetComponent<Browser>().videoExtensions)
		{
			if (path.Contains(extension)) return true;
		}
		return false;
	}

	IEnumerator FetchTexture(string path)
	{
		WWW www = new WWW("file:///"+path);
		yield return www;
		SetTexture(www.texture);
	}

	void SetTexture(Texture t)
	{
		renderer.material.mainTexture = t;
		renderer.material.SetTexture("_EmissionMap",t);
	}

	IEnumerator LoadSequence(string[] paths)
	{
		StopCoroutine("PlaySequence");
		paths.OrderBy(x => x).ToArray();
		List<Texture> sequence = new List<Texture>();
		foreach(var path in paths)
		{
			WWW www = new WWW("file:///"+path);
			yield return www;
			sequence.Add(www.texture);
			// Debug.Log(path + " loaded into memory");
			// StartCoroutine(LoadTexture(path));
			// Debug.Log("set " + path);
			// yield return new WaitForSeconds(frameDuration);
		}
		StartCoroutine(PlaySequence(sequence));
	}

	IEnumerator PlaySequence(List<Texture> s)
	{
		StopCoroutine("PlaySequence");
		Debug.Log("playing sequence");
		foreach(var t in s)
		{
			SetTexture(t);
			yield return new WaitForSeconds(frameDuration);
		}
		StartCoroutine(PlaySequence(s));
	}

	IEnumerator SetMovieTexture(string path)
	{
		WWW www = new WWW("file:///"+path);
		var movieTexture = www.movie;
		while (!movieTexture.isReadyToPlay)
			yield return new WaitForFixedUpdate();

		renderer.material.mainTexture = movieTexture;
		renderer.material.SetTexture("_EmissionMap",movieTexture);

		movieTexture.Play();
	}

	class ImageSequence
	{
		public Texture texture;

	}

}
