using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Data
{
    public interface ISocialRepository<T> where T : class
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChanges();
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers(UserQueryParams userParams);
        Task<bool> IsAlreadyFollowed(int followerUserId, int userId);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);
        IEnumerable<Message> GetMessages(int sender, int recipient);
        IEnumerable<MessageListDto> GetUserAllMessages(int userId);

    }
}