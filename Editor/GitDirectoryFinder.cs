using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Anecdote.TestHook.Editor
{
	/// <summary>
	/// Provides the .git directory of the repo this package is installed in, regardless of whether it's in the project root, a child folder, or a parent folder
	/// </summary>
	public class GitDirectoryFinder
	{
		public DirectoryInfo dInfo { get; private set; }
		public DirectoryInfo hooksDir { get; private set; }

		public GitDirectoryFinder()
		{
			dInfo = FindGitRootDirectory();

			if (dInfo == null ||
				dInfo.Exists == false)
			{
				var e = new DirectoryNotFoundException($"Cannot initialise {nameof(GitDirectoryFinder)} - no .git folder could be found. Does this repo contain/is contained within a Git working directory?");
				throw e;
			}

			string hooksDirPath = Path.Combine(dInfo.FullName, "hooks");
			hooksDir = new DirectoryInfo(hooksDirPath);

			if (hooksDir == null ||
				hooksDir.Exists == false)
			{
				var e = new DirectoryNotFoundException($"Cannot initialise {nameof(GitHookInstaller)} - no '/hooks' folder was found inside .git folder at '{dInfo.FullName}. Has Git been properly initialised?'");
				throw e;
			}
		}

		static DirectoryInfo FindGitRootDirectory()
		{
			var dirsToSearch = new Queue<DirectoryInfo>();

			dirsToSearch.Enqueue(new(Environment.CurrentDirectory));

			// Recursively search children for a .git folder
			do
			{
				var result = SearchChildrenForGitDir(dirsToSearch.Dequeue(), out DirectoryInfo[] children);

				if (result != null)
				{
					return result;
				}

				dirsToSearch.EnqueueRange(children);
			}
			while (dirsToSearch.Count > 0);

			dirsToSearch.Enqueue(new(Environment.CurrentDirectory));

			// Recursively search parents for a .git folder
			do
			{
				var result = SearchParentForGitDir(dirsToSearch.Dequeue(), out DirectoryInfo parent);

				dirsToSearch.Enqueue(parent);

				if (result != null)
				{
					return result;
				}
			}
			while (dirsToSearch.Count > 0);

			var e = new DirectoryNotFoundException($"Cannot initialise {nameof(GitDirectoryFinder)} - no .git folder could be found. Does this repo contain/is contained within a Git working directory?");
			throw e;
		}

		static DirectoryInfo SearchChildrenForGitDir(DirectoryInfo dir, out DirectoryInfo[] children)
		{
			children = dir.GetDirectories();

			foreach (DirectoryInfo child in children)
			{
				if (child.Name == ".git")
					return child;
			}

			return null;
		}

		static DirectoryInfo SearchParentForGitDir(DirectoryInfo dir, out DirectoryInfo parent)
		{
			parent = dir.Parent;

			foreach (DirectoryInfo child in parent.GetDirectories())
			{
				if (child.Name == ".git")
					return child;
			}

			return null;
		}

		[MenuItem("Anecdote/Debug/Print Git Directory")]
		static void TestGitRoot()
		{
			var gitFinder = new GitDirectoryFinder();

			Debug.Log($"GitDirectoryFinder successfully found: {gitFinder.dInfo}");
		}
	}
}
