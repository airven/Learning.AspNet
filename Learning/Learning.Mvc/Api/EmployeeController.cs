using Learning.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learning.Mvc.Api
{
    [RoutePrefix("api")]
    public class EmployeeController : ApiController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: api/Employee
        public IEnumerable<Employee> Get()
        {
            return db.Employees.ToList();
        }

        [Route("GetV2")]
        public HttpResponseMessage GetV2()
        {
            var employeeList = db.Employees.Select(item => new { item.EmployeeID, item.FirstName, item.LastName }).ToList();
            var message = Request.CreateResponse(HttpStatusCode.OK, employeeList);
            return message;
        }

        // GET: api/Employee/5
        public Employee Get(int id)
        {
            return db.Employees.FirstOrDefault(e => e.EmployeeID == id);
        }

        // POST: api/Employee
        [HttpPost]
        [Route("PostV1")]
        public void Post([FromBody] Employee employee)
        {
            using (NorthwindEntities dbContext = new NorthwindEntities())
            {
                dbContext.Employees.Add(employee);
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Route("PostV2")]
        public HttpResponseMessage PostV2(Employee employee)
        {
            try
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                message.Headers.Location = new Uri(Request.RequestUri +
                    employee.EmployeeID.ToString());
                return message;
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
           
        }

        [HttpPost]
        [Route("PostV3")]
        public HttpResponseMessage PostV3([FromBody]Employee employee)
        {
            try
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                message.Headers.Location = new Uri(Request.RequestUri +
                    employee.EmployeeID.ToString());
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT: api/Employee/5
        public void Put(int id, [FromBody] Employee employee)
        {
            var entity = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
            entity.FirstName = employee.FirstName;
            entity.LastName = employee.LastName;
            entity.City = employee.City;
            entity.Notes = employee.Notes;
            db.SaveChanges();
        }

        [HttpPut]
        [Route("PutV2")]
        public HttpResponseMessage PutV2(int id, [FromBody] Employee employee)
        {
            try
            {
                var entity = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id " + id.ToString() + " not found to update");
                }
                else
                {
                    entity.FirstName = employee.FirstName;
                    entity.LastName = employee.LastName;
                    entity.City = employee.City;
                    entity.Notes = employee.Notes;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
            db.Employees.Remove(db.Employees.FirstOrDefault(e => e.EmployeeID == id));
            db.SaveChanges();
        }

        [HttpDelete]
        [Route("DeleteV2")]
        public HttpResponseMessage DeleteV2(int id)
        {
            try
            {
                var entity = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id = " + id.ToString() + " not found to delete");
                }
                else
                {
                    db.Employees.Remove(entity);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
