using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetIdentityDemo.Security
{
    public class MyIdentityRole:IdentityRole
    {
        //do nothing currently

        public MyIdentityRole()
        {

        }

        public MyIdentityRole(string roleName) : base(roleName)
        {

        }

        public MyIdentityRole(string roleName,string description)
            : base(roleName)
        {
            this.Description = description;
        }


        public string Description { get; set; }

    }
}
