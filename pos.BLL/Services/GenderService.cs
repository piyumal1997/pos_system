using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System.Collections.Generic;

namespace pos_system.pos.BLL.Services
{
    internal class GenderService
    {
        private readonly GenderRepository _repository = new GenderRepository();

        public List<Gender> GetAllGenders()
        {
            return _repository.GetAllGenders();
        }

        public bool GenderExists(int genderId)
        {
            return _repository.GenderExists(genderId);
        }
    }
}