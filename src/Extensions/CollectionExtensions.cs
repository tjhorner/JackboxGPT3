using System;
using System.Collections;

namespace JackboxGPT3.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random random = new Random();

        public static int RandomIndex(this ICollection collection)
        {
            return random.Next(0, collection.Count);
        }
    }
}
