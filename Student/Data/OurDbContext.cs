using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Data
{
    public class OurDbContext:IdentityDbContext
    {
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options)
        {

        }
    }
}
