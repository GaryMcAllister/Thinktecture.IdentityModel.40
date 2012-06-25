﻿/*
 * Copyright (c) Dominick Baier & Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinktecture.IdentityModel.Http.Cors.HttpContext
{
    public class HttpContextConfiguration
    {
        static HttpContextConfiguration()
        {
            Configuration = new CorsConfiguration();
        }

        public static CorsConfiguration Configuration { get; private set; }

    }
}
