﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Message
    {
        public Message(string body, ConversationId conversationId, UserId senderId)
        {
            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException(nameof(body),"Message body shouldn't be empty");
            }
            Id = new MessageId();
            TimeSent = DateTime.Now; 
            Body = body;
            ConversationId = conversationId ?? throw new ArgumentNullException(nameof(conversationId));
            SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
            Delivered = false;
        }
        public MessageId Id { get; private set; }
        public string Body { get; private set; }
        public ConversationId ConversationId { get; private set; }
        //TODO Add shadow property for sender name
        public UserId SenderId { get; private set; }
        public AppUser Sender { get; private set; }
        public Conversation Conversation { get; private set; }
        public bool Delivered { get; private set; }
        public DateTime TimeSent { get; private set; }

        public Message MarkAsDelivered()
        {
            Delivered = true;
            return this;
        }
    }

    public record MessageId(string Value)
    {
        public MessageId() : this(Guid.NewGuid().ToString()) { }

        public override string ToString()
        {
            return Value;
        }
    }
}
