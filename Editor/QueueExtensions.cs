using System.Collections.Generic;

namespace Anecdote.TestHook.Editor
{
	// https://stackoverflow.com/questions/19141259/how-to-enqueue-a-list-of-items-in-c
	public static class QueueExtensions
	{
		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> enu)
		{
			foreach (T obj in enu)
				queue.Enqueue(obj);
		}
	}
}
