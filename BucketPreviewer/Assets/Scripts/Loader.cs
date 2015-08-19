using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Loader : MonoBehaviour {

	List<WWW> queue = new List<WWW>();
	List<WWW> finished = new List<WWW>();
	List<Texture> sequence = new List<Texture>();

	public LoaderText text;

	public void Load(string[] paths)
	{
		text.Set("Loading Sequence " + paths[0]);
		//check all paths
		if (paths.Length == 1)
		{
			if (IsImage(paths[0]))
			{
				Debug.Log("Image sequence");
				DiscoverSequence(paths);
			}
			else
				text.Set("Not an image or sequence");
		}
		else
		{
			LoadSequence(paths);
		}
		// else
		// {
		// 	foreach(var path in paths)
		// 	{
		// 		//check extension
		// 		Debug.Log(path);
		//
		// 		//if video load video to videotexture
		// 		if (IsVideo(path))
		// 		{
		// 			Debug.Log("Is Video");
		// 			// StartCoroutine(SetMovieTexture(path));
		// 		}
		// 		//if image sequence load image sequence
		// 		else if (IsImage(path))
		// 		{
		//
		// 			Debug.Log("Is Image");
		// 			// StartCoroutine(FetchTexture(path));
		// 		}
		// 		//if single texture simply load as texture
		// 	}
		// }
	}

	bool IsSequence(string[] paths)
	{
		foreach(var path in paths)
		{
			if (!IsImage(path)) return false;
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

	void DiscoverSequence(string[] paths)
	{
		//find filename
		Debug.Log(Path.GetFileNameWithoutExtension(paths[0]));
		Debug.Log(Path.GetDirectoryName(paths[0]));
		Debug.Log(Path.GetExtension(paths[0]));
		Debug.Log(Path.GetFullPath(paths[0]));
		string extension = Path.GetExtension(paths[0]);
		string filename = Path.GetFileNameWithoutExtension(paths[0]);
		string directory = Path.GetDirectoryName(paths[0]);
		//find numbers
		Regex rgx = new Regex("[0-9]*");
		string fileClean = rgx.Replace(filename, "");
		Debug.Log(fileClean);
		string numbers = filename.Replace(fileClean,"");
		Debug.Log(numbers);
		int index = Int32.Parse(numbers);
		string format = "";
		for (int i = 0; i < numbers.Length; i++)
		{
			format += "0";
		}
		// Debug.Log(index.ToString(format));
		string start = fileClean+index.ToString(format)+extension;
		string originalFile = fileClean+index.ToString(format)+extension;
		StartCoroutine(LoadPath(start));
		for(int i = index; i > 0; i--)
		{
			string path = directory+"/"+fileClean+i.ToString(format)+extension;
			// Debug.Log(path);
			if (File.Exists(path))
			{
					start = fileClean+i.ToString(format)+extension;
					StartCoroutine(LoadPath(path));
			}
			else
			{
				break;
			}
		}
		string end = originalFile;
		for(int i = index; i < 999999; i++)
		{
			string path = directory+"/"+fileClean+i.ToString(format)+extension;
			// Debug.Log(path);
			if (File.Exists(path))
			{
				end = fileClean+i.ToString(format)+extension;
					StartCoroutine(LoadPath(path));
			}
			else
			{
				break;
			}
		}
		text.Set("Loading: "+start+" until "+end);

	}

	void LoadSequence(string[] paths)
	{
		paths.OrderBy(x => x).ToArray();
		// Start threads to load images
		foreach(var p in paths)
		{
			StartCoroutine(LoadPath(p));
		}
		text.Set("Loading selected images");
		// List<Texture> sequence = new List<Texture>();
		//
		// foreach(var path in paths)
		// {
		// 	WWW www = new WWW("file:///"+path);
		// 	yield return www;
		// 	www.texture.name = path;
		// 	sequence.Add(www.texture);
		// 	Debug.Log(www.texture.name + " " + path + " loaded into memory");
		// 	// StartCoroutine(LoadTexture(path));
		// 	// Debug.Log("set " + path);
		// 	// yield return new WaitForSeconds(frameDuration);
		// }
	}

	IEnumerator LoadPath(string path)
	{
			WWW www = new WWW("file:///"+path);
			queue.Add(www);
			yield return www;
			queue.Remove(www);
			finished.Add(www);
			// Debug.Log("loaded" + path);
	}

	void Update()
	{
		if (queue.Count == 0 && finished.Count > 0)
		{
			finished = finished.OrderBy(x => x.url).ToList();
			foreach(var www in finished)
			{
				// Debug.Log(www.url);
				sequence.Add(www.texture);
			}
			finished.Clear();
			var player = GetComponent<Player>();
			player.Load(sequence.ToList());
			sequence.Clear();
			player.Play();
			text.Set("");
		}
	}


}
