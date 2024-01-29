using System;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Anecdote.TestHook.Editor
{
	public class GitHookInstaller
	{
		public bool preCommitHookDetected { get; private set; }

		internal	 GitDirectoryFinder gitFinder { get; }
		internal PackageDirectoryFinder pkgFinder { get; }

		public GitHookInstaller(GitDirectoryFinder gitFinder, PackageDirectoryFinder pkgFinder)
		{
			this.gitFinder = gitFinder;
			this.pkgFinder = pkgFinder;

			UpdateHookDetection();
		}

		internal void UpdateHookDetection()
		{
			bool newDetectionStatus = false;

			foreach (var file in gitFinder.hooksDir.GetFiles())
			{
				if (file.Name == "pre-commit")
					newDetectionStatus = true;
			}

			preCommitHookDetected = newDetectionStatus;
		}

		public void InstallHooks()
		{
			foreach (var file in pkgFinder.toolHooksDir.GetFiles())
			{
				var dest = Path.Combine(gitFinder.hooksDir.FullName, file.Name);

				file.CopyTo(dest, true);
			}

			UpdateHookDetection();

			if (preCommitHookDetected)
			{
				Debug.Log("Pre-commit hook successfully installed!");
			}
			else
			{
				Debug.LogError("Tried to install Git hooks, but still can't detect pre-commit hook");
			}
		}

		public void UninstallHooks()
		{
			foreach (var installedFile in gitFinder.hooksDir.GetFiles())
			{
				foreach (var sourceFile in pkgFinder.toolHooksDir.GetFiles())
				{
					if (installedFile.Name == sourceFile.Name &&
						installedFile.Length == sourceFile.Length)
					{
						Debug.Log($"installedFile '{installedFile}' and sourceFile '{sourceFile}' match name and size - deleting installedFile");
						installedFile.Delete();
						continue;
					}
				}
			}

			UpdateHookDetection();
		}
	}
}
