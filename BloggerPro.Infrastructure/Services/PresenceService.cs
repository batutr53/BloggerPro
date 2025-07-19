using AutoMapper;
using BloggerPro.Application.DTOs.Chat;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using BloggerPro.Shared.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BloggerPro.Infrastructure.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PresenceService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> UpdateUserPresenceAsync(Guid userId, bool isOnline, string? connectionId = null)
        {
            var presence = await _unitOfWork.UserPresences
                .FindByCondition(up => up.UserId == userId)
                .FirstOrDefaultAsync();

            if (presence == null)
            {
                presence = new UserPresence
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    LastSeen = DateTime.UtcNow,
                    ConnectionId = connectionId,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.UserPresences.AddAsync(presence);
            }
            else
            {
                presence.IsOnline = isOnline;
                presence.LastSeen = DateTime.UtcNow;
                presence.ConnectionId = connectionId;
                presence.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserPresences.UpdateAsync(presence);
            }

            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult("Kullanıcı durumu güncellendi.");
        }

        public async Task<DataResult<UserPresenceDto>> GetUserPresenceAsync(Guid userId)
        {
            var presence = await _unitOfWork.UserPresences
                .FindByCondition(up => up.UserId == userId)
                .FirstOrDefaultAsync();

            if (presence == null)
            {
                return new SuccessDataResult<UserPresenceDto>(new UserPresenceDto
                {
                    UserId = userId,
                    IsOnline = false,
                    LastSeen = DateTime.UtcNow
                });
            }

            var presenceDto = _mapper.Map<UserPresenceDto>(presence);
            return new SuccessDataResult<UserPresenceDto>(presenceDto);
        }

        public async Task<DataResult<List<UserPresenceDto>>> GetMultipleUserPresenceAsync(List<Guid> userIds)
        {
            var presences = await _unitOfWork.UserPresences
                .FindByCondition(up => userIds.Contains(up.UserId))
                .ToListAsync();

            var presenceDtos = new List<UserPresenceDto>();

            foreach (var userId in userIds)
            {
                var presence = presences.FirstOrDefault(p => p.UserId == userId);
                presenceDtos.Add(new UserPresenceDto
                {
                    UserId = userId,
                    IsOnline = presence?.IsOnline ?? false,
                    LastSeen = presence?.LastSeen ?? DateTime.UtcNow
                });
            }

            return new SuccessDataResult<List<UserPresenceDto>>(presenceDtos);
        }

        public async Task<Result> UserConnectedAsync(Guid userId, string connectionId)
        {
            return await UpdateUserPresenceAsync(userId, true, connectionId);
        }

        public async Task<Result> UserDisconnectedAsync(Guid userId, string connectionId)
        {
            var presence = await _unitOfWork.UserPresences
                .FindByCondition(up => up.UserId == userId && up.ConnectionId == connectionId)
                .FirstOrDefaultAsync();

            if (presence != null)
            {
                return await UpdateUserPresenceAsync(userId, false, null);
            }

            return new SuccessResult("Kullanıcı bağlantısı sonlandırıldı.");
        }
    }
}