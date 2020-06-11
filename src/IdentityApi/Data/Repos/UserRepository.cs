using IdentityApi.Models;

namespace IdentityApi.Data.Repos
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;


        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void smt()
        {

        }

    }
}