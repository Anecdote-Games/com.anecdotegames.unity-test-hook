using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Anecdote.TestHook.Editor
{
	/// <summary>
	/// TestResultSubscriber registers a callback to the completion of a test run.
	/// When a test run finishes, TestResultSubscriber will export the results to an XML file in the specified folder.
	/// </summary>
	[InitializeOnLoad]
	public class TestResultSubscriber : UnityEditor.TestTools.TestRunner.Api.ICallbacks
	{
		UnityEditor.TestTools.TestRunner.Api.TestRunnerApi api;

		readonly GitDirectoryFinder gitFinder;

		static TestResultSubscriber()
		{
			// Not required to listen to results in these scenarios, and doing so may slow down the process
			if (EditorApplication.isPlaying ||
				EditorApplication.isPaused  ||
				EditorApplication.isCompiling)
			{
				return;
			}

			new TestResultSubscriber();
		}

		TestResultSubscriber()
		{
			api = ScriptableObject.CreateInstance<TestRunnerApi>();
			api.RegisterCallbacks(this);

			gitFinder = new GitDirectoryFinder();

			SubscribeToEditorApplicationCallback();
		}

		void SubscribeToEditorApplicationCallback()
		{
			var executionSettings = new ExecutionSettings { runSynchronously = true };

			EditorApplication.CallbackFunction callback = delegate { };
			callback = () =>
			{
				api.Execute(executionSettings);
				EditorApplication.update -= callback;
			};

			EditorApplication.update += callback;
		}

		public void RunStarted(ITestAdaptor testsToRun) { /* Intentionally blank */ }

		public void RunFinished(ITestResultAdaptor result)
		{
			var pathToTestResultsFile = Path.Combine(gitFinder.hooksDir.FullName, "testResults.xml");

			using (var xw = XmlWriter.Create(pathToTestResultsFile, new XmlWriterSettings { Indent = true }))
			{
				// Write entire tests results to file at testResultsPath
				result.ToXml().WriteTo(xw);
			}

			Debug.Log($"Test Result: '{result.ResultState}'" +
				$" | Time taken: '{result.Duration * 1000}ms'" +
				$" | Tests passed: '{result.PassCount}'" +
				$" | Tests failed: '{result.FailCount}'" +
				$" | Tests skipped: '{result.SkipCount}'" +
				$" | Inconclusive: '{result.InconclusiveCount}'" +
				$" | Asserts thrown: '{result.AssertCount}'" +
				$"\nWritten to: '{pathToTestResultsFile}'");
		}

		public void TestStarted(ITestAdaptor test) { /* Intentionally blank */ }

		public void TestFinished(ITestResultAdaptor result) { /* Intentionally blank */ }
	}
}