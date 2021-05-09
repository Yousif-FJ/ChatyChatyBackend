﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatyChatyClient.Actions.Request.Messaging;
using ChatyChatyClient.Entities;
using MediatR;

namespace ChatyChatyClient.Actions.Handler.Messaging
{
    public class GetChatsHandler : IRequestHandler<GetChatsRequest, IList<Chat>>
    {
        public Task<IList<Chat>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}