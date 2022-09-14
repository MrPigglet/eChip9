using System;
using System.Threading;

namespace eChip9
{
    public static class ThreadSafeRandom // This is a better way to do Randomness
    {
        [ThreadStatic] private static Random Local;
        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
