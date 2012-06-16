﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Tokens.Http
{
    public class SessionAuthenticationConfiguration
    {
        public bool Enabled { get; set; }
        public SessionSecurityTokenHandler TokenHandler { get; set; }

        public SessionAuthenticationConfiguration()
        {
            Enabled = false;
            TokenHandler = new MachineKeySessionSecurityTokenHandler();
        }
    }
}
