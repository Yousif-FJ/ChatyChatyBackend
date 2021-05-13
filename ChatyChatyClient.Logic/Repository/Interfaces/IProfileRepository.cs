﻿using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Interfaces
{
    public interface IProfileRepository
    {
        public ValueTask Set(UserProfile profile);
        public ValueTask<UserProfile> Get();
        public ValueTask Update(UserProfile profile);
    }
}
