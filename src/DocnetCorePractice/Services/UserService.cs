using DocnetCorePractice.Data;
using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;

namespace DocnetCorePractice.Service
{
    public interface IUserService
    {
        List<UserModel> GetAllActiveUsers();
        List<UserModel> GetAllUsers();

        List<UserEntity> GetAllUserEnities();

        List<UserModel>? AddUser(UserModel userModel);

        List<UserModel>? UpdateUser(UserModel userModel);

        Boolean DeleteUser(UserModel userModel);

    }
    public class UserService : IUserService
    {
        private readonly IInitData _initData;

        public UserService(IInitData initData)
        {
            _initData = initData;
        }

        public List<UserEntity>? GetAllUserEnities()
        {
            return _initData.GetAllUsers();
        }

        
        public List<UserModel>? GetAllActiveUsers()
        {
            var entity = _initData.GetAllActiveUser();
            if (entity == null || !entity.Any())
            {
                return null;
            }
            var result = new List<UserModel>();
            entity.ForEach(x =>
            {
                var model = new UserModel
                {
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Balance = x.Balance,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Sex = x.Sex,
                    TotalProduct = x.TotalProduct
                };
                result.Add(model);
            });
            return result;
        }

        public List<UserModel>? GetAllUsers()
        {
            var entity = _initData.GetAllUsers();
            if (entity == null || !entity.Any())
            {
                return null;
            }
            var result = new List<UserModel>();
            entity.ForEach(x =>
            {
                var model = new UserModel
                {
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Balance = x.Balance,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Sex = x.Sex,
                    TotalProduct = x.TotalProduct
                };
                result.Add(model);
            });
            return result;
        }

        public List<UserModel>? AddUser(UserModel userModel)
        {
            var entity = new UserEntity
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Address = userModel.Address,
                DateOfBirth = userModel.DateOfBirth,
                Balance = userModel.Balance,
                IsActive = true,
                PhoneNumber = userModel.PhoneNumber,
                Sex = userModel.Sex,
                TotalProduct = userModel.TotalProduct,
                Role = Enum.Roles.Basic
            };
            _initData.AddUser(entity);
            return GetAllActiveUsers();
        }

        public List<UserModel>? UpdateUser(UserModel userModel)
        {
            var entity = new UserEntity
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Address = userModel.Address,
                DateOfBirth = userModel.DateOfBirth,
                Balance = userModel.Balance,
                IsActive = true,
                PhoneNumber = userModel.PhoneNumber,
                Sex = userModel.Sex,
                TotalProduct = userModel.TotalProduct,
                Role = Enum.Roles.Basic
            };
            var userToUpdate = _initData.GetAllUsers().FirstOrDefault(x => x.Id.Equals(userModel.Id));
            userToUpdate = entity;
            _initData.AddUser(userToUpdate);
            return GetAllUsers();

        }

        public Boolean DeleteUser(UserModel userModel)
        {
            var entity = _initData.GetAllUsers().FirstOrDefault(x => x.Id.Equals(userModel.Id));
            return _initData.RemoveUser(entity);
        }
       
    }
}
