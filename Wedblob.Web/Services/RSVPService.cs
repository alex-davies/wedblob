using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Wedblob.Web.Services
{
    public interface IRSVPService
    {
        Task<RSVP> Store(RSVP rsvp);
        Task<RSVP> Fetch(int id);
        Task<List<RSVP>> FetchAll();
    }

    public class RSVPService : IRSVPService
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IIdGenerator<RSVP> _idGenerator;
        private readonly IMongoCollection<RSVP> _rsvpCollection;

        public RSVPService(IMongoDatabase mongoDatabase, IIdGenerator<RSVP> idGenerator)
        {
            this._mongoDatabase = mongoDatabase;
            this._idGenerator = idGenerator;
            this._rsvpCollection = _mongoDatabase.GetCollection<RSVP>(typeof(RSVP).Name);
        }

        public async Task<RSVP> Store(RSVP rsvp)
        {
            rsvp.Id = rsvp.Id ?? await _idGenerator.Next();

            var collection = _mongoDatabase.GetCollection<RSVP>(typeof(RSVP).Name);
            var result = await collection.ReplaceOneAsync(
                new BsonDocument("_id", rsvp.Id), 
                rsvp, 
                new UpdateOptions() { IsUpsert = true});

            return rsvp;
        }

        public async Task<RSVP> Fetch(int id)
        {
            var findResult = await _rsvpCollection.Find<RSVP>(new BsonDocument("_id", id)).FirstOrDefaultAsync();
            return findResult;
        }

        public async Task<List<RSVP>> FetchAll()
        {
            var findResult = await _rsvpCollection.FindAsync(new BsonDocument());
            return await findResult.ToListAsync();
        }
    }

 

    public class RSVP
    {
        [BsonId]
        public int? Id { get; set; }

        public bool? Attending { get; set; }

        public string[] Guests { get; set; }
    }
}