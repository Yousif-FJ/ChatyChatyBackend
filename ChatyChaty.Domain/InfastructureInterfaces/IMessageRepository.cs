﻿using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    /// <summary>
    /// Interface that encapsulate the logic of accessing messages from a source
    /// </summary>
    public interface IMessageRepository
    {
        Task<Message> GetAsync(MessageId Id);
        Task<List<Message>> GetAllAsync(UserId userId, DateTime AfterDateTime = default);
        Task<List<Message>> GetForChatAsync(ConversationId conversationId);
        Task<List<Message>> GetStatusAsync(UserId userId, DateTime AfterDateTime = default);
        Task UpdateRangeAsync(IEnumerable<Message> messages);
        Task<Message> AddAsync(Message message);
        Task RemoveOverLimitAsync(UserId userId, int numberOfMessageToRemove = 100);
    }
}
