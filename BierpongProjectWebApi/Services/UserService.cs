using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Domain.Entities;

namespace BierpongProjectWebApi.Services
{
    public class UserService
    {
        private readonly CustomDbContext _dbContext;
        public UserService(CustomDbContext dbContext) => _dbContext = dbContext;

        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public User GetUser(string username) => _dbContext.Users.FirstOrDefault(x => x.Username == username);

        public bool ValidateUser(string username, string password) => _dbContext.Users.Any(x => x.Username == username && x.Password == password);

        public bool UserExists(string username) => _dbContext.Users.Any(x => x.Username == username);

        public void UpdateUser(string username, string name, string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) return;
            user.Name = name;
            user.Email = email;
            user.Password = password;
            _dbContext.SaveChanges();
        }

        public void DeleteUser(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) return;
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
