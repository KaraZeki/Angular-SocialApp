using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.Helpers;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ServiceFilter(typeof(LastActiveActionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ISocialRepository<Message> _messageRepository;
        private readonly ISocialRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(ISocialRepository<User> userrepository, ISocialRepository<Message> messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userrepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreateDTO messageForCreateDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreateDTO.SenderId = userId;

            var recipient = await _userRepository.GetUser(messageForCreateDTO.RecipientId);

            if (recipient == null)
                return BadRequest("mesaj göndermek istediğiniz kullanıcı yok.");

            var message = _mapper.Map<Message>(messageForCreateDTO);

            _messageRepository.Add(message);

            if (await _messageRepository.SaveChanges())
            {
                var messageDTO = _mapper.Map<MessageForCreateDTO>(message);
                return Ok(messageDTO);
            }
            throw new System.Exception("error");
        }

        [HttpGet]
        public IActionResult GetUserMessages(int userId)
        {

            var senderId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result =_messageRepository.GetMessages(senderId,userId);
            // var data =result.Select(x=>x.DateAdded).FirstOrDefault();
            var data = result.Select(data => new MessageResultDto()
            {
                SenderId=data.SenderId,
                RecipientId=data.RecipientId,
                SenderDeleted = data.SenderDeleted,
                Text = data.Text,
                DateAdded = data.DateAdded,
                DateRead = data.DateRead,
                IsRead = data.IsRead,
                RecipientDeleted=data.RecipientDeleted
            });
            // var data =_mapper.Map<MessageResultDto>(result);

            return Ok(data);
        }

        [HttpGet("GetUserAllMessages")]
        public IActionResult GetUserAllMessages()
        {

            var userId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var message =_messageRepository.GetUserAllMessages(userId);
            return Ok(message);
        }
    }
}