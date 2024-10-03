using System.Linq;
using DotnetAPI.Models;

namespace DotnetAPI.Data;

public class UserRepository : IUserRepository {
    private readonly DataContextEF _entityFramework;

    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    public bool SaveChanges() {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        if (entityToAdd != null)
        {
            _entityFramework.Add(entityToAdd);
        }
    }

    public void RemoveEntity<T>(T entityToRemove)
    {
        if (entityToRemove != null)
        {
            _entityFramework.Remove(entityToRemove);
        }
    }

    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User>? users = _entityFramework?.Users?.ToList<User>();
        if (users is not null){
            return users;
        }
        throw new Exception("Could not retrieve any users from database");
    }

    public User GetSingleUser(int userId)
    {
        User? user = _entityFramework?.Users?
                                      .Where(user => user.UserId == userId)
                                      .FirstOrDefault<User>();
        if (user is not null){
            return user;
        }
        throw new Exception("Could not retrieve User with UserId " + userId);
    }

    // We could make these generic by creating two more repositories inheriting from IUser with generics
    public UserJobInfo GetUserJobInfo(int userId)
    {
        UserJobInfo? jobInfo = _entityFramework?.UserJobInfo?
                                      .Where(uji => uji.UserId == userId)
                                      .FirstOrDefault<UserJobInfo>();
        if (jobInfo is not null){
            return jobInfo;
        }
        throw new Exception("Could not retrieve JobInfo for User with UserId " + userId);
    }

    public UserSalary GetUserSalary(int userId)
    {
        UserSalary? salary = _entityFramework?.UserSalary?
                                      .Where(sal => sal.UserId == userId)
                                      .FirstOrDefault<UserSalary>();
        if (salary is not null){
            return salary;
        }
        throw new Exception("Could not retrieve Salary for User with UserId " + userId);
    }
}
 