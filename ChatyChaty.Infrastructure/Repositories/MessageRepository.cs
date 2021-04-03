﻿using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.MessageRepository
{
    /// <summary>
    /// class that encapsulate the logic of accessing messages from EntityFramework managed DB
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatyChatyContext dBContext;

        public MessageRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<Message> AddAsync(Message Message)
        {
            var DBMessage = await dBContext.Messages.AddAsync(Message);
            await dBContext.SaveChangesAsync();
            return DBMessage.Entity;
        }

        public async Task<Message> GetAsync(MessageId Id)
        {
            return await dBContext.Messages.FindAsync(Id);
        }

        public async Task<IList<Message>> GetForChatAsync(ConversationId conversationId, int count = 100)
        {
            return await dBContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .AsSplitQuery()
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetAllAsync(UserId userId)
        {
            return await dBContext.Messages
            .Where(m => m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)
            .Include(m => m.Sender)
            .AsSplitQuery()
            .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetNewAsync(MessageId messageId, UserId userId)
        {
            var message = await GetAsync(messageId);

            return await dBContext.Messages.Where(
                m => m.TimeSent > message.TimeSent &&
                m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId
                ).Include(c => c.Sender)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Message> Messages)
        {
            dBContext.Messages.UpdateRange(Messages);
            await dBContext.SaveChangesAsync();
        }
    }
}
