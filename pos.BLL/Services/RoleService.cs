using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace pos_system.pos.BLL.Services
{
    public class RoleService
    {
        private readonly RoleRepository _repository = new RoleRepository();

        public List<Role> GetAllRoles()
        {
            return _repository.GetAllRoles();
        }
    }
}
