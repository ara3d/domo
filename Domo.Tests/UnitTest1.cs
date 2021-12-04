using System;
using System.ComponentModel;
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
            var modelId = repo.Model.Id;
            OutputRepo(repo);
            repo.Model.Value = new TestRecord(1, 3);
            Assert.AreEqual(modelId, repo.Model.Id);
            Assert.AreEqual(3, repo.Model.Value.Y);
            OutputRepo(repo);
            repo.Model.Value = repo.Model.Value with { Y = 4 };
            Assert.AreEqual(modelId, repo.Model.Id);
            Assert.AreEqual(4, repo.Model.Value.Y);
            OutputRepo(repo);
            repo.Model.Update(x => x with { Y = 5 });
            Assert.AreEqual(modelId, repo.Model.Id);
            Assert.AreEqual(5, repo.Model.Value.Y);
            OutputRepo(repo);
            repo.Update(repo.Model.Id, x => x with { Y = 6 });
            Assert.AreEqual(modelId, repo.Model.Id);
            Assert.AreEqual(6, repo.Model.Value.Y);
            OutputRepo(repo);
        }

        [Test]
        public static void TestAggregateRepo()
        {
            var store = new DataStore();
            var repo = store.AddAggregateRepository<TestRecord>();

            Assert.AreEqual(0, repo.GetModels().Count);
            var rec1 = new TestRecord { X = 1, Y = 2 };
            var model = repo.Add(rec1);
            var modelId = model.Id;
            Assert.AreEqual(modelId, repo.GetModels()[0].Id);
            Assert.AreEqual(rec1, repo.GetModels()[0].Value);
            Assert.AreEqual(1, repo.GetModels().Count);
            OutputRepo(repo);

            var rec2 = new TestRecord { X = 3, Y = 4 };
            var model2 = repo.Add(rec2);
            var modelId2= model2.Id;
            Assert.AreNotEqual(modelId2, modelId);
            Assert.AreEqual(2, repo.GetModels().Count);

            Assert.AreEqual(rec1, repo.GetValue(modelId));
            Assert.AreEqual(rec2, repo.GetValue(modelId2));

            Assert.AreEqual(modelId, repo.GetModel(modelId).Id);
            Assert.AreEqual(modelId2, repo.GetModel(modelId2).Id);
        }
    }
}