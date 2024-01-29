using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Anecdote.TestHook.Editor
{
	[InitializeOnLoad]
	public class TestHookEditorWindow : EditorWindow
    {
		readonly GitDirectoryFinder		gitFinder;
		readonly PackageDirectoryFinder pkgFinder;
		readonly GitHookInstaller		hookInstaller;

		#region MENU ITEM STATIC METHODS

		[MenuItem("Anecdote/Test Hook/Show Window")]
		static void OpenEditorWindow()
		{
			// Get existing window or if none, open a new one:
			TestHookEditorWindow window = EditorWindow.GetWindow<TestHookEditorWindow>();
			window.titleContent.text = "Anecdote Test Hook";
			window.Show();
		}

		#endregion MENU ITEM STATIC METHODS

		#region CONSTRUCTOR

		TestHookEditorWindow()
		{
			gitFinder = new GitDirectoryFinder();

			pkgFinder = new PackageDirectoryFinder();

			hookInstaller = new GitHookInstaller(gitFinder, pkgFinder);

			if (hookInstaller.preCommitHookDetected)
			{
				Debug.Log($"TestHookEditorWindow initialised - Pre-commit hook already installed");
			}
			else
			{
				Debug.Log($"TestHookEditorWindow initialised - Pre-commit hook not already installed, attempting install & showing editor window...");
				hookInstaller.InstallHooks();
			}
		}

		#endregion CONSTRUCTOR

		void OnGUI()
		{
			if (hookInstaller == null)
			{
				GUILayout.Label("Could not initialise Test Hook: See errors in console");
				return;
			}

			// Show status of precommit hook 
			if (hookInstaller.preCommitHookDetected)
			{
				GUILayout.Label("Pre-commit Hook Status: Installed!");
			}
			else
			{
				GUILayout.Label("Pre-commit Hook Status: Not installed");

				if (GUILayout.Button("Re-check for Git Precommit Hook"))
				{
					hookInstaller.UpdateHookDetection();
				}

				// Show a button to install the precommit hook
				if (GUILayout.Button("Install Git Precommit Hook"))
				{
					hookInstaller.InstallHooks();
				}
			}
		}
	}
}
