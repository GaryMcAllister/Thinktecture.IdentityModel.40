﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Thinktecture.IdentityModel.Http.Cors.WebAPI
{
    public class WebAPICorsConfiguration : CorsConfiguration
    {
        public void RegisterGlobal(HttpConfiguration httpConfig)
        {
            httpConfig.MessageHandlers.Add(new CorsMessageHandler(this, httpConfig));
        }

        public DelegatingHandler CreateHandler(HttpConfiguration httpConfig, DelegatingHandler innerHandler = null)
        {
            return new CorsMessageHandler(this, httpConfig)
            {
                InnerHandler = innerHandler
            };
        }
    }
}
