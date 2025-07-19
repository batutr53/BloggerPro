using AutoMapper;
using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserFollowerService _userFollowerService;

        public ChatService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserFollowerService userFollowerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userFollowerService = userFollowerService;
        }

        public async Task<DataResult<List<ConversationDto>>> GetUserConversationsAsync(Guid userId)
        {
            var conversations = await _unitOfWork.Conversations
                .FindByCondition(c => (c.User1Id == userId || c.User2Id == userId) && c.IsActive)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.LastMessage)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();

            var conversationDtos = new List<ConversationDto>();

            foreach (var conversation in conversations)
            {
                var otherUser = conversation.User1Id == userId ? conversation.User2 : conversation.User1;
                
                // Get unread message count
                var unreadCount = await _unitOfWork.Messages
                    .FindByCondition(m => m.SenderId == otherUser.Id && m.ReceiverId == userId && !m.ReadAt.HasValue)
                    .CountAsync();

                // Get presence information
                var presence = await _unitOfWork.UserPresences
                    .FindByCondition(up => up.UserId == otherUser.Id)
                    .FirstOrDefaultAsync();

                conversationDtos.Add(new ConversationDto
                {
                    Id = conversation.Id,
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.UserName,
                    OtherUserDisplayName = !string.IsNullOrEmpty(otherUser.FirstName) && !string.IsNullOrEmpty(otherUser.LastName) 
                        ? $"{otherUser.FirstName} {otherUser.LastName}" 
                        : otherUser.UserName,
                    OtherUserProfileImage = otherUser.ProfileImageUrl,
                    LastMessage = conversation.LastMessage?.Content,
                    LastMessageTime = conversation.LastMessage?.SentAt,
                    UnreadCount = unreadCount,
                    IsOnline = presence?.IsOnline ?? false,
                    LastSeen = presence?.LastSeen
                });
            }

            return new SuccessDataResult<List<ConversationDto>>(conversationDtos);
        }

        public async Task<DataResult<List<MessageDto>>> GetConversationMessagesAsync(Guid userId, Guid otherUserId, int page = 1, int pageSize = 50)
        {
            // Check if users can chat (mutual followers)
            var canChat = await CanUsersStartConversationAsync(userId, otherUserId);
            if (!canChat.Data)
            {
                return new ErrorDataResult<List<MessageDto>>("Bu kullanıcıyla mesajlaşabilmek için karşılıklı takip etmeniz gerekiyor.");
            }

            var messages = await _unitOfWork.Messages
                .FindByCondition(m => 
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderUserName = m.Sender.UserName,
                ReceiverId = m.ReceiverId,
                ReceiverUserName = m.Receiver.UserName,
                Content = m.Content,
                SentAt = m.SentAt,
                DeliveredAt = m.DeliveredAt,
                ReadAt = m.ReadAt
            }).Reverse().ToList();

            return new SuccessDataResult<List<MessageDto>>(messageDtos);
        }

        public async Task<DataResult<MessageDto>> SendMessageAsync(Guid senderId, CreateMessageDto messageDto)
        {
            // Check if users can chat (mutual followers)
            var canChat = await CanUsersStartConversationAsync(senderId, messageDto.ReceiverId);
            if (!canChat.Data)
            {
                return new ErrorDataResult<MessageDto>("Bu kullanıcıyla mesajlaşabilmek için karşılıklı takip etmeniz gerekiyor.");
            }

            // Create message
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                SentAt = DateTime.UtcNow,
                DeliveredAt = DateTime.UtcNow // Assume immediate delivery for now
            };

            await _unitOfWork.Messages.AddAsync(message);

            // Create or update conversation
            var existingConversation = await _unitOfWork.Conversations
                .FindByCondition(c => 
                    (c.User1Id == senderId && c.User2Id == messageDto.ReceiverId) ||
                    (c.User1Id == messageDto.ReceiverId && c.User2Id == senderId))
                .FirstOrDefaultAsync();

            if (existingConversation == null)
            {
                var conversation = new Conversation
                {
                    User1Id = senderId,
                    User2Id = messageDto.ReceiverId,
                    LastMessageId = message.Id,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Conversations.AddAsync(conversation);
            }
            else
            {
                existingConversation.LastMessageId = message.Id;
                existingConversation.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Conversations.UpdateAsync(existingConversation);
            }

            await _unitOfWork.SaveChangesAsync();

            // Get full message with user info
            var fullMessage = await _unitOfWork.Messages
                .FindByCondition(m => m.Id == message.Id)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync();

            var resultMessageDto = new MessageDto
            {
                Id = fullMessage.Id,
                SenderId = fullMessage.SenderId,
                SenderUserName = fullMessage.Sender.UserName,
                ReceiverId = fullMessage.ReceiverId,
                ReceiverUserName = fullMessage.Receiver.UserName,
                Content = fullMessage.Content,
                SentAt = fullMessage.SentAt,
                DeliveredAt = fullMessage.DeliveredAt,
                ReadAt = fullMessage.ReadAt
            };

            return new SuccessDataResult<MessageDto>(resultMessageDto);
        }

        public async Task<Result> MarkMessagesAsReadAsync(Guid userId, MarkMessagesAsReadDto markAsReadDto)
        {
            var messages = await _unitOfWork.Messages
                .FindByCondition(m => markAsReadDto.MessageIds.Contains(m.Id) && m.ReceiverId == userId)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.ReadAt = DateTime.UtcNow;
                await _unitOfWork.Messages.UpdateAsync(message);
            }

            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult("Mesajlar okundu olarak işaretlendi.");
        }

        public async Task<Result> MarkMessagesAsDeliveredAsync(List<Guid> messageIds)
        {
            var messages = await _unitOfWork.Messages
                .FindByCondition(m => messageIds.Contains(m.Id) && !m.DeliveredAt.HasValue)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.DeliveredAt = DateTime.UtcNow;
                await _unitOfWork.Messages.UpdateAsync(message);
            }

            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult("Mesajlar teslim edildi olarak işaretlendi.");
        }

        public async Task<DataResult<bool>> CanUsersStartConversationAsync(Guid userId1, Guid userId2)
        {
            if (userId1 == userId2)
                return new SuccessDataResult<bool>(false);

            var areMutual = await _userFollowerService.AreMutualFollowersAsync(userId1, userId2);
            return new SuccessDataResult<bool>(areMutual.Data);
        }
    }
}