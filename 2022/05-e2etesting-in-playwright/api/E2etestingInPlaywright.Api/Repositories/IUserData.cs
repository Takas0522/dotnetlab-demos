using E2etestingInPlaywright.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2etestingInPlaywright.Api.Repositories
{
    public interface IUserData
    {
        Task DeletetAsync(long id);
        Task<IEnumerable<User>> GetAllDatasAsync();
        Task<User> GetDataAsync(long id);
        Task UpsertAsync(User data);
    }
}