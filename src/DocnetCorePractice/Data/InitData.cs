﻿using DocnetCorePractice.Data.Entity;
using System.Linq;

namespace DocnetCorePractice.Data
{
    public interface IInitData
    {
        List<UserEntity> GetAllActiveUser();
        List<UserEntity> GetAllUsers();
        bool AddUser(UserEntity entity);
        bool RemoveUser(UserEntity entity);
        List<CaffeEntity> GetAllCaffeEntities();
    }
    public class InitData : IInitData
    {
        private static List<UserEntity> users = new List<UserEntity>()
        {
            new UserEntity()
            {
                //Id = Guid.NewGuid().ToString("N"),
                FirstName = "Hoang Anh",
                LastName = "Nguyen",
                Sex = Enum.Sex.Female,
                Address = "Ho chi Minh",
                Balance = 100000,
                DateOfBirth = new DateTime(year:2003, month:5, day:1),
                PhoneNumber = "0123456789",
                Role = Enum.Roles.Basic,
                TotalProduct = 0,
                IsActive = true
            }
        };

        private static List<CaffeEntity> caffes = new List<CaffeEntity>()
        {
            new CaffeEntity()
            {
                //Id = Guid.NewGuid().ToString("N"),
                Name = "Ca phe sua",
                Price = 20000,
                Discount = 10,
                Type = Enum.ProductType.A,
                CreateUser = "Hoang Anh",
                IsActive = true
            }
        };

        private static List<OrderItemEntity> orderItems = new List<OrderItemEntity>();
        private static List<OrderEntity> orders = new List<OrderEntity>();

        public List<UserEntity> GetAllActiveUser()
        {
            return users.Where(x => x.IsActive == true).ToList();
        }

        public List<UserEntity> GetAllUsers()
        {
            return users.ToList();
        }

        public bool AddUser(UserEntity entity)
        {
            users.Add(entity);
            return true;
        }

        public bool RemoveUser(UserEntity entity)
        {
            users.Remove(entity);
            return true;
        }

        public List<CaffeEntity> GetAllCaffeEntities()
        {
            return caffes;
        }
    }
}
