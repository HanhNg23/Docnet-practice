using DocnetCorePractice.Data;
using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;
using System.Globalization;

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
        private readonly string DobPattern = "dd-mm-yyyy";

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
                    Id = x.Id,
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
                    Id = x.Id,
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
            var userToUpdate = _initData.GetAllUsers().FirstOrDefault(x => x.Id.Equals(userModel.Id));
            
            userToUpdate.FirstName = userModel.FirstName;
            userToUpdate.LastName = userModel.LastName;
            userToUpdate.Address = userModel.Address;
            userToUpdate.DateOfBirth = userModel.DateOfBirth;
            userToUpdate.Balance = userModel.Balance;
            userToUpdate.IsActive = true;
            userToUpdate.PhoneNumber = userModel.PhoneNumber;
            userToUpdate.Sex = userModel.Sex;
            userToUpdate.TotalProduct = userModel.TotalProduct;
            userToUpdate.Role = Enum.Roles.Basic;

            return GetAllUsers().Where(x => userModel.Id.Equals(x.Id)).ToList();

        }

        public Boolean DeleteUser(UserModel userModel)
        {
            var entity = _initData.GetAllUsers().FirstOrDefault(x => x.Id.Equals(userModel.Id));
            return _initData.RemoveUser(entity);
        }

        
        
       
    }
}
