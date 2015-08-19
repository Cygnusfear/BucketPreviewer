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

	public Renderer rails;

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
			defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		#endif

		#if UNITY_EDITOR_OSX
			defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		#endif

		#if UNITY_STANDALONE_WIN
			defaultPath = Environment.SpecialFolder.Personal;
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
			Application.LoadLevel(0);
		}
		if (Input.GetKeyDown(KeyCode.H)) {
			rails.enabled = !rails.enabled;
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			controller.transform.localScale = controller.transform.localScale + new Vector3(0,0.1f,0);
		}
		if (Input.GetKeyDown(KeyCode.Q)) {
			controller.transform.localScale = controller.transform.localScale - new Vector3(0,0.1f,0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Vector3 pos = controller.transform.position;
			pos.y = 4.45f;
			controller.transform.position = pos;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Vector3 pos = controller.transform.position;
			pos.y = 7.849999f;
			controller.transform.position = pos;
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
