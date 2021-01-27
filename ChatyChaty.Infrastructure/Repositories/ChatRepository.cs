﻿using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.ChatRepository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatyChatyContext dBContext;

        public ChatRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<Conversation> GetConversationAsync(long ConversationId)
        {
            return await dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public async Task<IEnumerable<Conversation>> GetConversationsWithUsersAsync(long UserId)
        {
            return await dBContext.Conversations
                .Where(c => c.FirstUserId == UserId || c.SecondUserId == UserId)
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .ToListAsync();
        }

        public async Task<Conversation> CreateConversationAsync(long User1Id, long User2Id)
        {
            var conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                (c => (c.FirstUserId == User1Id && c.SecondUserId == User2Id) ||
                      (c.FirstUserId == User2Id && c.SecondUserId == User1Id)
            ));

            //convestaion not found create one
            if (conversation == null)
            {
                var newConversation = new Conversation(User1Id, User2Id);
                var resultConv = await dBContext.Conversations.AddAsync(newConversation);
                await dBContext.SaveChangesAsync();
                return resultConv.Entity;
            }
            else
            {
                return conversation;
            }
        }


        public async Task<IEnumerable<Conversation>> GetConversationsAsync(long userId)
        {
            return await dBContext.Conversations
               .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
               .ToListAsync();
        }
    }
}