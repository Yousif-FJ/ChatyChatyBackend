﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Pages
{
    public partial class Chat
    {
        [Parameter]
        public long ChatId { get; set; }
    }
}