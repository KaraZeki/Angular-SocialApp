using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class SocialRepository<T> : ISocialRepository<T> where T : class
    {
        private readonly SocialContext _context;
        public SocialRepository(SocialContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(i => i.Images)
                                .FirstOrDefaultAsync(i => i.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers(UserQueryParams userParams)
        {
            var users = _context.Users
                        .Where(i => i.Id != userParams.UserId)
                        .Include(i => i.Images)
                        .OrderByDescending(i => i.LastActive)
                        .AsQueryable();

            if (userParams.Followers)
            {
                // takip edenler
                var result = await GetFollows(userParams.UserId, false);
                users = users.Where(u => result.Contains(u.Id));
            }

            if (userParams.Followings)
            {
                // takip edilenler
                var result = await GetFollows(userParams.UserId, true);
                users = users.Where(u => result.Contains(u.Id));
            }

            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                users = users.Where(i => i.Gender == userParams.Gender);
            }

            if (userParams.minAge != 18 || userParams.maxAge != 100)
            {
                var today = DateTime.Now;
                var min = today.AddYears(-(userParams.maxAge + 1));
                var max = today.AddYears(-userParams.minAge);

                users = users.Where(i => i.DateOfBirth >= min && i.DateOfBirth <= max);
            }

            if (!string.IsNullOrEmpty(userParams.City))
            {
                users = users.Where(i => i.City.ToLower() == userParams.City.ToLower());
            }

            if (!string.IsNullOrEmpty(userParams.Country))
            {
                users = users.Where(i => i.Country.ToLower() == userParams.Country.ToLower());
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                if (userParams.OrderBy == "age")
                {
                    users = users.OrderBy(i => i.DateOfBirth);
                }
                else if (userParams.OrderBy == "created")
                {
                    users = users.OrderByDescending(i => i.Created);
                }
            }

            return await users.ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsAlreadyFollowed(int followerUserId, int userId)
        {
            return await _context.UserToUser
                .AnyAsync(i => i.FollowerId == followerUserId && i.UserId == userId);
        }

        private async Task<IEnumerable<int>> GetFollows(int userId, bool IsFollowings)
        {
            var user = await _context.Users
                                .Include(i => i.Followers)
                                .Include(i => i.Followings)
                                .FirstOrDefaultAsync(i => i.Id == userId);

            if (IsFollowings)
            {
                return user.Followers
                            .Where(i => i.FollowerId == userId)
                            .Select(i => i.UserId);
            }
            else
            {
                return user.Followings
                            .Where(i => i.UserId == userId)
                            .Select(i => i.FollowerId);
            }
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public IEnumerable<Message> GetMessages(int sender, int recipient)
        {
            var messages = _context.Messages.AsQueryable();

            IEnumerable<Message> filteredResult = from message in messages
                                                  where (message.SenderId == sender && message.RecipientId == recipient) ||
                                                        (message.SenderId == recipient && message.RecipientId == sender)
                                                  orderby message.DateAdded
                                                  select message;

            return filteredResult.ToList();


        }

        public IEnumerable<MessageListDto> GetUserAllMessages(int userId)
        {
            var messages = _context.Messages.AsQueryable();

            var filteredResult = from message in messages
                                 where message.SenderId == 8 || message.RecipientId == 8
                                 group message by new
                                 {
                                     message.SenderId,
                                     message.RecipientId,

                                 } into msg
                                 select new MessageListDto()
                                 {
                                     SenderId = msg.Key.SenderId,
                                     RecipientId = msg.Key.RecipientId,
                                 };

            var newFilterMessage = new List<MessageListDto>();

            foreach (var result in filteredResult)
            {



                var message = _context.Messages
                .Where(x => x.SenderId == result.SenderId && x.RecipientId == result.RecipientId)
                .OrderBy(x => x.DateAdded)
                .LastOrDefault();



                if (result.SenderId != userId)
                {
                    var user = _context.Users
                        .Where(x => x.Id == result.SenderId)
                        .Include(x => x.Images)
                        .FirstOrDefault();
                    newFilterMessage.Add(new MessageListDto
                    {
                        UserName = user.Name,
                        ImageUrl = user.Images.Where(x => x.IsProfile == true).FirstOrDefault()is null ?"":user.Images.Where(x => x.IsProfile == true).FirstOrDefault().Name,
                        LastMessage = message.Text,
                        SenderId = result.SenderId,
                        RecipientId = result.RecipientId
                    });
                }
                else if(result.RecipientId != userId)
                {
                    var user = _context.Users
                       .Where(x => x.Id == result.RecipientId)
                       .Include(x => x.Images)
                       .FirstOrDefault();
                    newFilterMessage.Add(new MessageListDto
                    {
                        UserName = user.Name,
                        ImageUrl = user.Images.Where(x => x.IsProfile == true).FirstOrDefault()is null ?"":user.Images.Where(x => x.IsProfile == true).FirstOrDefault().Name,
                        LastMessage = message.Text,
                        SenderId = result.SenderId,
                        RecipientId = result.RecipientId
                    });
                }
            }



            return newFilterMessage.ToList();

        }
    }
}