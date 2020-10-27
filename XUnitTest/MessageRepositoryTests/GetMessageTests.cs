﻿using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class GetMessageTests : BaseMessageRepositoryTest
    {
        [Fact]
        public async Task OneMessage_OneChat_ShouldReturn_OneMessage()
        {
            //Arrange
            var messageBody = "Some message";
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var message = dbContext.Messages.Add(new Message(messageBody, chat.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var returnedMessage = await repository.GetMessageAsync(message.Id);
            //Assert
            Assert.True(returnedMessage.Body == message.Body);
            Assert.True(returnedMessage.ConversationId == message.ConversationId);
            Assert.True(returnedMessage.SenderId == message.SenderId);
        }
    }
}