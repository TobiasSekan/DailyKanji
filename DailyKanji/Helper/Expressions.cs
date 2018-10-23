using System;
using System.Collections.Generic;

namespace DailyKanji.Helper
{
    internal static class Expressions
    {
        // https://en.wikipedia.org/wiki/Fisher–Yates_shuffle

        internal static void Shuffle<T>(this IList<T> deck)
        {
            var r = new Random();

            for(var n = deck.Count - 1; n > 0; --n)
            {
                var k    = r.Next(n + 1);
                var temp = deck[n];

                deck[n] = deck[k];
                deck[k] = temp;
            }
        }
    }
}
