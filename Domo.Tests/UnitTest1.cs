using System;
using NUnit.Framework;
using System.Text.Json;

namespace Domo.Tests
{
    public readonly record struct TestRecord(int X, int Y);

    public class Tests
    {
        [Test]
        public static void Test1()
        {
            var rec = new TestRecord(1, 2);
            Console.WriteLine($"{rec}");
            var jsonString = JsonSerializer.Serialize(rec);
            Console.WriteLine(jsonString);
            var rec2 = JsonSerializer.Deserialize<TestRecord>(jsonString);
            Console.WriteLine(rec2);
            var jsonString2 = JsonSerializer.Serialize(rec2);
            Assert.AreEqual(jsonString, jsonString2);
        }

    }
}