﻿/*
 * Copyright (c) Dominick Baier & Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Tokens.Http
{
    public class AuthenticationOptionMapping
    {
        public AuthenticationOptions Options { get; set; }
        public SecurityTokenHandlerCollection TokenHandler { get; set; }
    }
}
