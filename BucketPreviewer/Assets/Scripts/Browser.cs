using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SkywardRay.FileBrowser;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class Browser : MonoBehaviour {
	public GameObject prefabBrowser;

	private SkywardFileBrowser fileBrowser;
	private string defaultPath = "/Users/";
	private string[] extensions = { ".jpg", ".png", "" };
	private Stopwatch restartTimer;

	private void Start () {
		// Create the File Browser
		fileBrowser = Instantiate(prefabBrowser).GetComponent<SkywardFileBrowser>();

		// Change some settings
		fileBrowser.Settings.ShowHiddenFiles = false;

		// There are multiple examples in the asset, so in order to give each file browser it's own settings file
		fileBrowser.Settings.SettingsSaveFileName = "SfbOverlayExampleSettings";

		// Open the File Browser
		OpenFileBrowser(SfbMode.Open, defaultPath, Output, extensions);
	}

	private void Update () {
		// Quit the demo by pressing escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		// Restart the browser when it is closed, so we don't have to reopen the demo
		ReOpenFileBrowser();
	}

	// Opens the File Browser in the specified mode
	private void OpenFileBrowser (SfbMode mode, string path, Action<string[]> outputCallback, string[] extensions = null) {
		if (mode == SfbMode.Save) {
			fileBrowser.SaveFile(path, outputCallback, extensions);
		}
		else {
			fileBrowser.OpenFile(path, outputCallback, extensions);
		}
	}

	private void ReOpenFileBrowser () {
		// Don't start the timer when the file browser is still open
		if (!fileBrowser.IsWindowOpen && restartTimer == null) {
			restartTimer = Stopwatch.StartNew();
		}
		if (!fileBrowser.IsWindowOpen && restartTimer.ElapsedMilliseconds > 500) {
			// Reopen the File Browser
			var mode = fileBrowser.Mode;
			var path = fileBrowser.GetCurrentDirectoryPath();
			OpenFileBrowser(mode, path, Output, extensions);

			restartTimer = null;
		}
	}

	// The function recieving the output from the filebrowser
	private void Output (string[] output) {
		// Simply print the paths to show something happened. 
		// Replace this with your own code to use the File Browser output
		foreach (string path in output) {
			Debug.Log("File browser output: " + path);
		}
	}
}
