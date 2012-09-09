﻿/*
 * Copyright (c) Dominick Baier & Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Claims;
using Microsoft.IdentityModel.Claims;
using System;
using System.IdentityModel.Tokens;
using System.Web;
using System.Text;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Thinktecture.IdentityModel.Tokens.Http
{
    public class AuthenticationHandler : DelegatingHandler
    {
        HttpAuthentication _authN;

        public AuthenticationHandler(AuthenticationConfiguration configuration, HttpConfiguration httpConfiguration = null)
        {
            _authN = new HttpAuthentication(configuration);

            if (httpConfiguration != null)
            {
                InnerHandler = new HttpControllerDispatcher(httpConfiguration);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_authN.Configuration.InheritHostClientIdentity == false)
            {
                SetPrincipal(Principal.Anonymous);
            }

            try
            {
                // try to authenticate
                // returns an anonymous principal if no credential was found
                var principal = _authN.Authenticate(request);

                if (principal == null)
                {
                    throw new InvalidOperationException("No principal set");
                }

                if (principal.Identity.IsAuthenticated)
                {
                    // check for token request - if yes send token back and return
                    if (_authN.IsSessionTokenRequest(request))
                    {
                        return SendSessionTokenResponse(principal);
                    }

                    // else set the principal
                    SetPrincipal(principal);
                }
            }
            catch (SecurityTokenValidationException)
            {
                return SendUnauthorizedResponse();
            }
            catch (SecurityTokenException)
            {
                return SendUnauthorizedResponse();
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    var response = task.Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        SetAuthenticateHeader(response);
                    }

                    return response;
                });
        }

        private Task<HttpResponseMessage> SendUnauthorizedResponse()
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                SetAuthenticateHeader(response);

                return response;
            });
        }

        private Task<HttpResponseMessage> SendSessionTokenResponse(ClaimsPrincipal principal)
        {
            var token = _authN.CreateSessionToken(principal);
            var tokenResponse = _authN.CreateSessionTokenResponse(token);

            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(tokenResponse, Encoding.UTF8, "application/json");

                return response;
            });
        }

        protected virtual void SetAuthenticateHeader(HttpResponseMessage response)
        {
            if (_authN.Configuration.SendWwwAuthenticateResponseHeader)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_authN.Configuration.DefaultAuthenticationScheme));
            }
        }

        protected virtual void SetPrincipal(ClaimsPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    }
}