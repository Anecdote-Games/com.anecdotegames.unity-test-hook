using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Anecdote.TestHook.Editor
{
	/// <summary>
	/// Provides the directory that the contents of the specified package are located in, regardless of whether it's installed as a local package or an authoring package
	/// </summary>
	public class PackageDirectoryFinder
	{
		public DirectoryInfo dInfo { get; }
		/// <summary>
		/// Folder inside this package's location that contains the hook files that will be copied to git's hooks directory
		/// </summary>
		public DirectoryInfo toolHooksDir { get; }

		const string PKG_NAME = "com.anecdotegames.unity-test-hook";

		public PackageDirectoryFinder()
		{
			dInfo = FindPackageDirectory();

			string toolHooksDirPath = Path.Combine(dInfo.FullName, @".tools", "hooks");
			toolHooksDir = new DirectoryInfo(toolHooksDirPath);

			if (toolHooksDir == null ||
				toolHooksDir.Exists == false)
			{
				var e = new DirectoryNotFoundException($"Cannot initialise {nameof(GitHookInstaller)} - no '/.tools/hooks' folder was found at the package location - '{dInfo.FullName}. Have you imported the package as the README.md instructed?'");
				throw e;
			}
		}

		DirectoryInfo FindPackageDirectory()
		{
			DirectoryInfo result = null;

			result = CheckPackageAuthoringLocation();

            if (result != null)
            {
				return result;
			}

			result = CheckPackageCacheLocation();

			if (result != null)
			{
				return result;
			}

			var e = new DirectoryNotFoundException($"Cannot initialise {nameof(PackageDirectoryFinder)} - no '{PKG_NAME}' folder could be found. Does this repo contain/is contained within a Git working directory?");
			throw e;
		}

		DirectoryInfo CheckPackageAuthoringLocation()
		{
			string[] path = { "Packages" };

			return CheckLocation(path);
		}

		DirectoryInfo CheckPackageCacheLocation()
		{
			string[] path = { "Library",
							  "PackageCache" };

			return CheckLocation(path);
		}

		DirectoryInfo CheckLocation(params string[] dirsRelativeToRoot)
		{
			var paths = new List<string>();
			paths.Add(Environment.CurrentDirectory);
			paths.AddRange(dirsRelativeToRoot);
			paths.Add(PKG_NAME);

			string pkgCacheDirPath = Path.Combine(paths.ToArray());

			var pkgCacheDirInfo = new DirectoryInfo(pkgCacheDirPath);

			if (pkgCacheDirInfo.Exists)
			{
				return pkgCacheDirInfo;
			}

			return null;
		}

		[MenuItem("Anecdote/Debug/Print Package Directory")]
		static void TestGitRoot()
		{
			var pkgFinder = new PackageDirectoryFinder();

			Debug.Log($"PackageDirectoryFinder successfully found: {pkgFinder.dInfo}");
		}
	}
}
