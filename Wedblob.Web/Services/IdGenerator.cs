using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Wedblob.Web.Services
{
    public interface IIdGenerator<T>
    {
        Task<int> Next();
    }

    public class IdGenerator<T> : IIdGenerator<T>
    {
        public class Counter
        {
            [BsonId]
            public string Name { get; set; }

            public int Sequence { get; set; }
        }

        private readonly IMongoDatabase _mongoDatabase;


        public IdGenerator(IMongoDatabase mongoDatabase)
        {
            this._mongoDatabase = mongoDatabase;
        }

        public async Task<int> Next()
        {
            var filter = new BsonDocument("_id", typeof(T).Name);
            var update = new BsonDocument("$inc", new BsonDocument("Sequence", 1));
            var options = new FindOneAndUpdateOptions<Counter, Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            var counter = await _mongoDatabase.GetCollection<Counter>(typeof(Counter).Name).FindOneAndUpdateAsync(filter, update, options);
            return counter.Sequence;
        }
    }
}