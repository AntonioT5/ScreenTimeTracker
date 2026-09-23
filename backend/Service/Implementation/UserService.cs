using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;

        public UserService(IRepository<User> repository)
        {
            _repository=repository;
        }
        public async Task ChangeUserData(Guid userId, string? username, string? mail)
        {
            var user = await _repository.Get(
                selector: x=> x,
                predicate: x=> x.Id==userId
            );

            if (user is null)
            {
                return;
            }

            if(username is not null)
            {
                var existingUser = await _repository.Get(
                    selector: x=> x,
                    predicate: x=> x.Username==username && x.Id != userId
                );

                if(existingUser is not null)
                {
                    throw new InvalidOperationException("Username or email is already taken.");
                }

                user.Username = username;
            }

            if(mail is not null)
            {
                var existingUser = await _repository.Get(
                    selector: x=> x,
                    predicate: x=> x.Email==mail && x.Id != userId
                );

                if(existingUser is not null)
                {
                    throw new InvalidOperationException("Username or email is already taken.");
                }

                user.Email = mail;
            }

            await _repository.UpdateAsync(user);
        }
    }
}