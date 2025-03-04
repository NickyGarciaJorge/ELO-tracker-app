using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Models.Entities;

namespace BierpongProjectWebApi.Services
{
    public class UserService
    {
        public UserService()
        {
            
        }
        private readonly CustomDbContext _dbContext;
        public UserService(CustomDbContext dbContext) => _dbContext = dbContext;

        public virtual void AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public virtual User GetUser(string username) => _dbContext.Users.FirstOrDefault(x => x.Username == username);
        public virtual string GetUserRole(string username) => _dbContext.Users.FirstOrDefault(x => x.Username == username).Role.ToString();

        public virtual List<User> GetUsers() => _dbContext.Users.ToList();

        public virtual bool ValidateUser(string username, string password) => _dbContext.Users.Any(x => x.Username == username && x.Password == password);

        public virtual bool UserExists(string username) => _dbContext.Users.Any(x => x.Username == username);

        public virtual void UpdateUser(string username, string name, string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) return;
            user.Name = name;
            user.Email = email;
            user.Password = password;
            _dbContext.SaveChanges();
        }

        public virtual void DeleteUser(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) return;
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
