using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public enum Gender_e
    {
        Male,
        Female
    }
    
    public enum Role_e
    {
        Viewer,
        Admin,
        ProjectManager
    }
}
