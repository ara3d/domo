using System;
using System.Linq;
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

        

        public static void OutputRepo(IRepository r)
        {
            Console.WriteLine($"{r.RepositoryId} {r.ValueType.Name} {r.GetType().Name}");
            var models = string.Join(", ", r.GetModels().Select(m => m.ToDebugString()));
            Console.WriteLine($"[{models}]");
        }

        [Test]
        public static void TestSingletonRepo()
        {
            var rec = new TestRecord(1, 2);
            var store = new DataStore();
            var repo = store.AddSingletonRepository(rec);
            OutputRepo(repo);
            repo.Model.Value = new TestRecord(2, 3);
            OutputRepo(repo);
            repo.Model.Value = repo.Model.Value with { X = 3, Y = 4 };
            OutputRepo(repo);
            repo.Model.Update(x => x with { X = 4, Y = 5 });
            OutputRepo(repo);
            repo.Update(repo.Model.Id, x => x with { X = 4, Y = 5 });
            OutputRepo(repo);
        }

    }
}