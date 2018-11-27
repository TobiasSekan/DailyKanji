using System;
using System.Collections.Generic;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Expression methods for lists
    /// </summary>
    public static class ListExpressions
    {
        /// <summary>
        /// Shuffle the <see cref="IList{T}"/>
        /// <para>see <a href="https://en.wikipedia.org/wiki/Fisher–Yates_shuffle">"Fisher Yates - Shuffle"</a>
        /// for more information</para>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the list</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();

            for(var oldIndex = list.Count - 1; oldIndex > 0; --oldIndex)
            {
                var newIndex = random.Next(oldIndex + 1);
                var tempCopy = list[oldIndex];

                list[oldIndex] = list[newIndex];
                list[newIndex] = tempCopy;
            }
        }
    }
}
