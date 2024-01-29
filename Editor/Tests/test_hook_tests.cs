using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Anecdote.TestHook.Editor;

public class test_hook_tests
{
	GitDirectoryFinder gitFinder;
	PackageDirectoryFinder pkgFinder;
	GitHookInstaller hookInstaller;

	[SetUp]
	public void SetUp()
	{
		gitFinder = new GitDirectoryFinder();
		pkgFinder = new PackageDirectoryFinder();
		hookInstaller = new GitHookInstaller(gitFinder, pkgFinder);
	}

	[TearDown]
	public void TearDown()
	{
		gitFinder = null;
		pkgFinder = null;
		hookInstaller = null;
	}

	[Test]
    public void git_path_found()
    {
		Assert.NotNull(gitFinder.dInfo);
		Assert.True(gitFinder.dInfo.Exists);
    }

	[Test]
	public void git_hooks_path_found()
	{
		Assert.NotNull(gitFinder.hooksDir);
		Assert.True(gitFinder.hooksDir.Exists);
	}

	[Test]
	public void pkg_path_found()
	{
		Assert.NotNull(pkgFinder.dInfo);
		Assert.True(pkgFinder.dInfo.Exists);
	}

	[Test]
	public void pkg_tool_hooks_path_found()
	{
		Assert.NotNull(pkgFinder.toolHooksDir);
		Assert.True(pkgFinder.toolHooksDir.Exists);
	}

	[Test]
	public void git_hook_installs()
	{
		hookInstaller.UninstallHooks();
		hookInstaller.InstallHooks();

		Assert.True(hookInstaller.preCommitHookDetected);
	}

	[Test]
	public void git_hook_auto_installs()
	{
		hookInstaller.UninstallHooks();
		hookInstaller = new GitHookInstaller(gitFinder, pkgFinder);

		Assert.True(hookInstaller.preCommitHookDetected);
	}

	[Test]
	public void git_hook_uninstalls()
	{
		hookInstaller.InstallHooks();
		hookInstaller.UninstallHooks();

		Assert.False(hookInstaller.preCommitHookDetected);
	}

	[Test]
	public void enqueue_range()
	{
		Queue<int> queue = new Queue<int>();
		queue.Enqueue(1);
		queue.EnqueueRange(new int[] { 2, 3, 4 });
		queue.Enqueue(5);

		Assert.AreEqual(5, queue.Count);
	}
}
