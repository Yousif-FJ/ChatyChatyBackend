﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public class Chat
    {
        public Chat(string id, UserProfile profile)
        {
            Id = id;
            Profile = profile;
        }

        public string Id { get; }
        public UserProfile Profile { get; set; }
        public bool IsThereNewMessage { get; set; }
    }
}