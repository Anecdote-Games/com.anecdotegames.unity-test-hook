# Unity Test Hook
UTH is a Unity Editor Plugin which enforces unit tests passing before permitting a Git commit. 
If test results are outdated, will ask Unity Editor to re-run tests.

# Pre-requisities
1. Unity Editor (Tested with Unity 2022, but any 2019 version or newer should work)
2. Git (Tested with Git version 2.38.1)
3. Python (Tested with Python 3.12)

# How to install
The simplest way to add UTH to your project is as a Unity Package (UPM).

## Installing as a Unity Package
1. Open the Unity Package Manager window inside your Unity Project 
2. Click "Add package from Git URL..."
3. Enter the URL of this repo: https://github.com/AnecdoteGames/com.anecdotegames.unity-test-hook
4. Unity should automatically download and install the package.
5. Open the UTH Editor Window by clicking Anecdote > Unity Test Hook > Show Window 
6. Complete the instructions as shown to install the Git precommit hook.
7. You're done! Your commits will now require a recent full pass of the project's unit test suite, and if one isn't available will automatically request the editor to run them.

# How it works
UTH largely comprises of three primary components:

1. TestResultSubscriber.cs 
  The Result Subscriber will attach to the Unity Editor's callback for when a test run has occurred, and write an XML file of the test results to disk. This is important so those results can be validated by the pre-commit hook.
  
2. TestRequestScanner.cs
  The Request Scanner watches for an incoming request to perform a fresh run of the test suite, and when detecting such a request performs said run. The Result Subscriber (see 1.) will then output the results to the XML file.
  
3. Git pre-commit hook (Written in Python)
  The Python hook has two primary functions:
  a. Detects if a test results file exists, and if it does validates the results are recent, passing, and comprise the entire unit test suite.
  b. If the file is not found or is found to be too stale, it creates a request to run the tests (in the form of a blank test.request file), which is then detected by the Request Scanner (see 2.).
  
# License
UTH is distributed under the Anti-Capitalist Software License, you can see the full terms of which in the LICENSE.md file.
If your entity does not meet the license terms required to use this software for free, you may apply for a paid commercial license by contacting us at [calum@anecdotegames.com].

# FAQ

## Q. Which Git clients does UTH work with?
A. UTH should work with any/all Git clients, as it simply installs a hook against the core Git protocol.

## Q. Which Unity versions does UTH work with?
A. It has only been tested with Unity 2022, but should work with any fairly recent version. Note that it does have a dependency on Unity's Editor Coroutines package, which the Package Manager will download automatically.

## Q. Which Operating Systems does UTH work with?
A. It has only been tested with Windows 11, but should work with any OS that supports Unity, Git and Python.

## Q. Is UTH editor-only?
A. Yes. UTH is not added to your builds, it only runs in the editor.

## Q. Is UTH available on the Unity Asset Store?
A. Not at this stage, but if you'd like it to be please email us at [calum@anecdotegames.com] and let us know!

## Q. I've found an issue/bug / I have a feature request.
A. Please feel free to add it yourself and submit a [PR](https://github.com/AnecdoteGames/com.anecdotegames.unity-test-hook/pull-requests)! Otherwise you can log an issue in the [Github issue tracker](https://github.com/AnecdoteGames/com.anecdotegames.unity-test-hook/issues)

# Credits
UTH was created by Calum Spring, and the initial idea for UTH was also proposed by Dan Serebro. Very special thanks to Dan Serebro & Alex Short for their ideas & support!
