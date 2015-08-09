using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Browser))]
public class LoadTexture : MonoBehaviour {

	public Texture2D texture;
	public MeshRenderer renderer;

	public void Load(string[] paths)
	{
		//check all paths

		if (paths.Length > 1)
		{
			if (IsImage(paths[0]))
			{
				Debug.Log("Image sequence");
				StartCoroutine(SetSequence(paths));
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
					StartCoroutine(SetTexture(path));
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

	IEnumerator SetTexture(string path)
	{
		WWW www = new WWW("file:///"+path);
		yield return www;
		renderer.material.mainTexture = www.texture;
		renderer.material.SetTexture("_EmissionMap",www.texture);
	}

	IEnumerator SetSequence(string[] paths)
	{
		StopCoroutine("SetSequence");
		foreach(var path in paths)
		{
			StartCoroutine(SetTexture(path));
			// Debug.Log("set " + path);
			yield return new WaitForSeconds(0.04f);
		}
		StartCoroutine(SetSequence(paths));
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

}
