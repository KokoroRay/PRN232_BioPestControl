using article_service.Data;
using article_service.Models;
using article_service.Services.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace article_service.Services.Implements
{
    public class ContactService : IContactService
    {
        private readonly MongoDbContext _context;

        public ContactService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _context.Contacts.Find(_ => true).SortByDescending(c => c.SubmittedAt).ToListAsync();
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await _context.Contacts.InsertOneAsync(contact);
            return contact;
        }

        public async Task<bool> ResolveAsync(string id, string resolutionNotes)
        {
            var update = Builders<Contact>.Update
                .Set(c => c.IsResolved, true)
                .Set(c => c.ResolutionNotes, resolutionNotes)
                .Set(c => c.ResolvedAt, DateTime.UtcNow);

            var result = await _context.Contacts.UpdateOneAsync(c => c.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }
}
