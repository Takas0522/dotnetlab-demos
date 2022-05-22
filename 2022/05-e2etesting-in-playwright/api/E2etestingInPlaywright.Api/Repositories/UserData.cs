using E2etestingInPlaywright.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2etestingInPlaywright.Api.Repositories
{
    public class UserData : IUserData
    {
        private IEnumerable<User> _users;

        public UserData()
        {
            // めっちゃ雑だがInitialize時にBaseData作っちゃう(DIをSingletonでやるからDemo中は生き残る…)
            _users = new List<User>()
            {
                new User { Id = 1, Name = "hoge", Email = "hoge@example.com", Role = RoleType.Admin },
                new User { Id = 2, Name = "fuga", Email = "fuga@example.com", Role = RoleType.User },
                new User { Id = 3, Name = "hege", Email = "hege@example.com", Role = RoleType.User },
                new User { Id = 4, Name = "DevTakas", Email = "devtakas@example.com", Role = RoleType.Admin }
            };
        }

        public async Task<IEnumerable<User>> GetAllDatasAsync()
        {
            return await Task.Run(() =>
            {
                return _users;
            });
        }

        public async Task<User> GetDataAsync(long id)
        {
            return await Task.Run(() =>
            {
                var u = _users.Where(x => x.Id == id);
                if (u.Any())
                {
                    return u.FirstOrDefault();
                }
                return null;
            });
        }

        public async Task UpsertAsync(User data)
        {
            await Task.Run(async () =>
            {
                var u = await GetDataAsync(data.Id);
                if (u != null)
                {
                    u.Email = data.Email;
                    u.Name = data.Name;
                    u.Role = data.Role;
                }
                else
                {
                    var l = _users.ToList();
                    l.Add(data);
                    _users = l;
                }
            });
        }

        public async Task DeletetAsync(long id)
        {
            await Task.Run(async () =>
            {
                var u = await GetDataAsync(id);
                if (u != null)
                {
                    var l = _users.ToList();
                    l.Remove(u);
                    _users = l;
                }
            });
        }

    }
}
