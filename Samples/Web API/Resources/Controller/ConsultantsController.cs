﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Security;

namespace Resources
{
    [Authorize]
    public class ConsultantsController : ApiController
    {
        IConsultantsRepository _repository;

        public ConsultantsController()
        {
            _repository = new InMemoryConsultantsRepository();
        }

        [AllowAnonymous]
        public IQueryable<Consultant> Get()
        {
            //throw new SecurityException("go away, bad guy!!");

            return _repository.GetAll().AsQueryable();
        }

        [AllowAnonymous]
        public Consultant Get(int id)
        {
            var consultant = _repository.GetAll().FirstOrDefault(c => c.ID == id);

            if (consultant == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            return consultant;
        }

        public HttpResponseMessage Post(Consultant consultant)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }

            consultant.Owner = Thread.CurrentPrincipal.Identity.Name;
            var id = _repository.Add(consultant);

            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri.AbsoluteUri + "/" + id.ToString());
            
            return response;
        }

        public void Put(int id, Consultant consultant)
        {
            // check if consultant exists
            var oldConsultant = _repository.GetAll().FirstOrDefault(c => c.ID == id);

            if (oldConsultant == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            consultant.ID = id;
            consultant.Owner = Thread.CurrentPrincipal.Identity.Name;

            if (oldConsultant.Owner != consultant.Owner)
            {
                throw new SecurityException("Not authorized to change record");
            }
            
            _repository.Update(consultant);
        }
    }
}
