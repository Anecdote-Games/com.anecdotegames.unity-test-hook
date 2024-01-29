using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using Unity.EditorCoroutines.Editor;
using System;

namespace Anecdote.TestHook.Editor
{
	/// <summary>
	/// TestRequestScanner actively monitors for the appearance of a 'test.request' file in the expected folder. 
	/// If this file is detected, TestRequestScanner will execute a run of the Edit Mode test suite.
	/// It then deletes the 'test.request' file as the request has been fufilled.
	/// </summary>
	[InitializeOnLoad]
	public class TestRequestScanner
	{
		const string REQ_FIL_NAME = "test.request";
		const float SCAN_DELAY = 5.0f;

		readonly UnityEditor.TestTools.TestRunner.Api.TestRunnerApi api;

		readonly GitDirectoryFinder gitFinder;

		static TestRequestScanner()
		{
			// Not required to listen to results in these scenarios, and doing so may slow down the process
			if (EditorApplication.isPlaying ||
				EditorApplication.isPaused ||
				EditorApplication.isCompiling)
			{
				return;
			}

			new TestRequestScanner();
		}

		TestRequestScanner()
		{
			api = ScriptableObject.CreateInstance<TestRunnerApi>();

			gitFinder = new GitDirectoryFinder();

			EditorCoroutineUtility.StartCoroutine(DoRecurringScan(gitFinder.hooksDir), this);
		}

		IEnumerator DoRecurringScan(DirectoryInfo dInfo)
		{
			var delay = new EditorWaitForSeconds(SCAN_DELAY);

			while (true)
			{
				yield return delay;

				CheckForRequestFile(dInfo);
			}
		}

		void CheckForRequestFile(DirectoryInfo dInfo)
		{
			//Debug.Log($"Checking for {requestFileName} file at {DateTime.Now.ToLongTimeString()}...");

			var files = dInfo.GetFiles();

			foreach (var file in files)
			{
				if (file.Name == REQ_FIL_NAME)
				{
					// Run test suite
					RunTests();

					// Consume request
					file.Delete();

					Debug.Log($"'{REQ_FIL_NAME}' file found, running tests");

					return;
				}
			}

			//Debug.Log("Not found.");
		}

		void RunTests()
		{
			var filter = new Filter
			{
				testMode = TestMode.EditMode
			};

			var executionSettings = new ExecutionSettings(filter)
			{
				runSynchronously = true
			};

			api.Execute(executionSettings);
		}
	}
}