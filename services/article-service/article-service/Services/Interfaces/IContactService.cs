using article_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace article_service.Services.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact> CreateAsync(Contact contact);
        Task<bool> ResolveAsync(string id, string resolutionNotes);
    }
}
