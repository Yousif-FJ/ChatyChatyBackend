﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Message
    {
        public Message(string body, ConversationId conversationId, UserId senderId)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentNullException(nameof(body),"Message body can't be empty");
            }
            Id = new MessageId();
            SentTime = DateTime.UtcNow;
            Body = body;
            ConversationId = conversationId ?? throw new ArgumentNullException(nameof(conversationId));
            SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
        }

        public MessageId Id { get; private set; }
        public string Body { get; private set; }
        public ConversationId ConversationId { get; private set; }
        public UserId SenderId { get; private set; }
        public string SenderUsername { get; private set; }
        public AppUser Sender { get; private set; }
        public Conversation Conversation { get; private set; }
        public bool Delivered { get; private set; }
        public DateTime SentTime { get; private set; }
        public DateTime? StatusUpdateTime { get; private set; }
        public Message MarkAsDelivered()
        {
            Delivered = true;
            StatusUpdateTime = DateTime.UtcNow;
            return this;
        }

        public Message SetSender(string username)
        {
            SenderUsername = username;
            return this;
        }
    }

    public record MessageId : IdBase
    {
        public MessageId() : base() { }
        public MessageId(string value) : base(value){}
        override public string ToString() { return base.ToString(); }
    }
}
