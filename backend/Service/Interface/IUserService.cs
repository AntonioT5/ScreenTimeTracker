using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUserService
    {
        Task ChangeUserData(Guid userId, string? username, string? mail);
    }
}