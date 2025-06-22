using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Linq;

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
