using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System.Data;

namespace pos_system.pos.DAL.Repositories
{
    public class RoleRepository
    {
        public List<Role> GetAllRoles()
        {
            const string query = "SELECT Role_ID, RoleName, Description FROM Role";
            var dt = DbHelper.GetDataTable(query, CommandType.Text);

            return dt.AsEnumerable().Select(row => new Role
            {
                Role_ID = row.Field<int>("Role_ID"),
                RoleName = row.Field<string>("RoleName"),
                Description = row.Field<string>("Description")
            }).ToList();
        }
    }
}
