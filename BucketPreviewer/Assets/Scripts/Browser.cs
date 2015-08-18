using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SkywardRay.FileBrowser;
using SkywardRay.FileBrowser.Example;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Browser))]
public class Browser : MonoBehaviour {
	public GameObject prefabBrowser;

	private SkywardFileBrowser fileBrowser;
	private string defaultPath = "/Users/";
	public string[] extensions = { ".jpg", ".png", "tiff" };
	public string[] videoExtensions = { ".mov", ".avi"};

	public FirstPersonController controller;

	string[] AllExtensions()
	{
		var allExtensions = extensions.ToList();
		allExtensions.AddRange(videoExtensions);
		return allExtensions.ToArray();
	}

	private void Start () {
		// Create the File Browser
		fileBrowser = Instantiate(prefabBrowser).GetComponent<SkywardFileBrowser>();

		fileBrowser.Settings.ShowHiddenFiles = true;

		fileBrowser.Settings.SettingsSaveFileName = "SfbOverlayExampleSettings";

		#if UNITY_STANDALONE_OSX
			defaultPath = "/Users/";
		#endif

		#if UNITY_EDITOR_OSX
			defaultPath = "/Users/";
		#endif

		#if UNITY_STANDALONE_WIN
			defaultPath = "/";
		#endif
		// Open the File Browser
		Open();
	}

	private void Update () {
		// Quit by pressing escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		if (Input.GetKeyDown(KeyCode.O)) {
			Open();
		}
	}

	public void Open()
	{
		var allExtensions = extensions.ToList();
		allExtensions.AddRange(videoExtensions);
		OpenFileBrowser(SfbMode.Open, defaultPath, Output, AllExtensions());
	}

	private void OpenFileBrowser (SfbMode mode, string path, Action<string[]> outputCallback, string[] extensions = null) {
		controller.enabled = false;
		Cursor.visible = true;
		if (mode == SfbMode.Save) {
			fileBrowser.SaveFile(path, outputCallback, extensions);
		}
		else {
			fileBrowser.OpenFile(path, outputCallback, extensions);
		}
	}

	private void Output (string[] output) {
		Cursor.visible = false;
		controller.enabled = true;
		GetComponent<Loader>().Load(output);
	}
}
