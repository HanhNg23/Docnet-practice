using DocnetCorePractice.Attribute;
using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;
using DocnetCorePractice.Service;
using DocnetCorePractice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Serilog;
using ILogger = Serilog.ILogger;
using DocnetCorePractice.Enum;

namespace DocnetCorePractice.Controllers
{
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public IUserService _userService;
        public ICaffeService _caffeService;
        private readonly ILogger _logger;
        

        public ActionController(IServiceProvider serviceProvider)
        {
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _caffeService = serviceProvider.GetRequiredService<ICaffeService>();
            _logger = Log.Logger;
        }

        [HttpPost]
        [Route("/api/[controller]/login")]
        public IActionResult Login(RequestLoginModel request)
        {
            return Ok(_authenticationService.Authenticator(request));
        }

        [ApiKey]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("/api/[controller]/getalluser")]
        public IActionResult GetAllUser()
        {
            var result = _userService.GetAllActiveUsers();
            return Ok(result);
        }

        
        [HttpPost("/api/[controller]/adduser")]
        public IActionResult AddUser([FromBody] UserModel model)
        {
            var result = _userService.AddUser(model);
            return Ok(result);
        }

        // 1. Viết API insert thêm caffe mới vào menu với input là CaffeModel, kiểm tra điều kiện:
        //      - Name và Id không tồn tại trong CaffeEntity (nếu không thỏa return code 400)
        //      - Price hoặc discount >= 0 (nếu không thỏa return code 400)
        //   Nếu có điều kiện nào vi phạm thì không insert và return failed.
        
        [HttpPost]
        [Route("/api/[controller]/new-caffe")]
        public IActionResult AddNewCaffe([FromBody][Required] CaffeModel caffe)
        {
            var existedCaffe = _caffeService.GetAllCaffeEntities()
                .Where(x => x.Id.Equals(caffe.Id) || x.Name.ToLower().Equals(caffe.Name.ToLower()))
                .FirstOrDefault();
            if (existedCaffe != null)
            {
                return BadRequest("Duplicated Caffe ID Or Name");
            }
            if (caffe.Price < 0 || caffe.Discount < 0)
            {
                return BadRequest("The Price and Discount must be equal or greater than 0");
            }
            
                var addedCaffe = _caffeService.AddNewCaffe(new CaffeEntity()
                {
                    Id = caffe.Id,
                    Name = caffe.Name,
                    Price = caffe.Price,
                    Discount = caffe.Discount,
                    CreateTimes = DateTime.Now,
                    CreateUser = "HA",
                    IsActive = true,
                    Type = Enum.ProductType.A,
                });

            if (addedCaffe == null) {
                return BadRequest("Added Failed");
            }
            
            return Ok(addedCaffe);
        }

        // 2. Viết API get all caffe có IsActive = true theo CaffeModel.
        // nếu không có caffe nào thì return code 204
        [HttpGet]
        [Route("/api/[controller]/active-caffe")]
        public IActionResult GetAllActiveCaffe()
        {
            var activeCaffe = _caffeService.GetAllCaffeEntities().Where(x => x.IsActive);
            if(activeCaffe.Count() <= 0) {
                return NoContent();
            }
            return Ok(activeCaffe);
        }

        // 3. Viết API get detail caffe có input là Id với điều kiện isActive bằng true.
        // Nếu không có user nào thì return code 204

        [HttpGet]
        [Route("/api/[controller]/caffe/{id}")]
        public IActionResult GetActiveCaffeById([FromQuery] string id)
        {
            try
            {
                var activeCaffe = _caffeService.GetAllCaffeEntities().Where(x => x.Id.Equals(id) && x.IsActive && x.CreateUser != null);
                if (activeCaffe.Count() <= 0)
                {
                    return NoContent();
                }
                return Ok(activeCaffe);
            }catch(Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        // 4. Viết API update caffe với input là Id, price và discount. kiểm tra điều kiện:
        //      - Id tồn tại trong CaffeEntity (nếu không thỏa return code 404)
        //      - Price hoặc discount >= 0 (nếu không thỏa return code 400)
        //   Nếu có điều kiện nào vi phạm thì không insert và return failed.

        [HttpPut]
        [Route("/api/[controller]/update-caffe/{id}")]
        public IActionResult UpdateCaffeById([FromQuery] string id, [FromBody] RequestUpdateCaffe caffe)
        {
            try
            {
                var activeCaffe = _caffeService.GetAllCaffeEntities().Where(x => x.Id.Equals(id)).FirstOrDefault();

                if (activeCaffe == null)
                {
                    return NotFound();
                }

                if (caffe.Price < 0 || caffe.Discount < 0)
                {
                    return BadRequest("Price and Discount must be equal or greater than 0 !");
                }
                activeCaffe.Price = caffe.Price;
                activeCaffe.Discount = caffe.Discount;
                activeCaffe.LastUpdateTimes = DateTime.Now;
                _caffeService.UpdateExistedCaffe(activeCaffe);

                return Ok(activeCaffe);
            }
            catch (Exception ex) {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
     
        }

        // 5. Viết API Delete caffe với input là Id. Caffe sẽ được delete nếu thỏa điều kiện sau:
        //  - Id tồn tại trong CaffeEntity (nếu không thỏa return code 400)

        [HttpDelete]
        [Route("/api/[controller]/delete-caffe/{id}")]
        public IActionResult DeleteCaffeById([FromQuery] string id)
        {
            try
            {
                var caffeToDelete = _caffeService.GetAllCaffeEntities().Where(x => x.Id.Equals(id)).FirstOrDefault();

                if (caffeToDelete == null)
                {
                    return BadRequest();
                }
                var deletedCaffe = _caffeService.DeleteCaffe(caffeToDelete);
                if (!deletedCaffe)
                    return BadRequest("Delete Failed !");

                return Ok("Deleted Success");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
       
        }

        // 6.Viết API insert thêm user mới với input là UserModel, kiểm tra điều kiện:
        //      - PhoneNumber và Id không tồn tại trong UserEntity (nếu không thỏa return code 400)
        //      - ngày sinh không được nhập quá Datatime.Now (nếu không thỏa return code 400)
        //      - PhoneNumber phải đúng 10 ký tự (nếu không thỏa return code 400)
        //      - Balance hoặc TotalProduct >= 0 (nếu không thỏa return code 400)
        //  Nếu có điều kiện nào vi phạm thì không insert và return failed.

        [HttpPost]
        [Route("/api/[controller]/new-user")]
        public IActionResult AddNewUser([FromBody] UserModel user)
        {
            try
            {
                if (user.DateOfBirth > DateTime.Now || user.PhoneNumber.Trim().Length != 10 || user.Balance < 0 || user.TotalProduct < 0)
                {
                    return BadRequest("- Ngày sinh không được nhập quá Datatime.Now" +
                        "\n- PhoneNumber phải đúng 10 ký tự" +
                        "\n- Balance hoặc TotalProduct >= 0");
                }

                var existedUsers = _userService.GetAllUsers();

                var existedUser = existedUsers.FirstOrDefault(x =>
                    x.PhoneNumber.Equals(user.PhoneNumber)
                    &&
                    x.Id.Equals(user.Id)
                );

                if (existedUser != null)
                {
                    return BadRequest("Phonenumber hoặc ID đã tồn tại !");
                }

                var currentUsers = _userService.AddUser(user);

                return Ok(currentUsers.FirstOrDefault(x => x.PhoneNumber.Equals(user.PhoneNumber)));
            }
            catch(Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
 
        }

        // 7.Viết API get all user data trả về được parse theo UserModel. nếu không có user nào thì return code 204
        [HttpGet]
        [Route("/api/[controller]/users")]
        public IActionResult GetAllUserModels()
        {
            try
            {
                var users = _userService.GetAllUsers();
                if (users == null || users.Count <= 0)
                {
                    return NoContent();
                }
                return Ok(users);
            }
            catch(Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
           
        }

        // 8.Với input là ngày sinh(có kiều dữ liệu DateTime) và role(có kiểu dữ liệu Enum), Viết API get users với điều kiện:
        //      - là thành viên vip(có thể là vip1 hoặc vip2) và sinh trong tháng theo input
        //  Nếu không có user nào thì return code 204
        [HttpGet]
        [Route("/api/[controller]/users")]
        public IActionResult GetAllUserByDateOfBirthAndRole([FromQuery] DateTime birthdate, [FromQuery] Roles role)
        {
            try
            {
                var existedUsers = _userService.GetAllUserEnities();
                var filteredUsers = existedUsers.Where(x => x.DateOfBirth.Month == birthdate.Month && x.Role.Equals(role));
                if (filteredUsers == null || filteredUsers.Count() <= 0)
                {
                    return NoContent();
                }
                return Ok(filteredUsers);
            }
            catch(Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        // 9.Viết API update user với input là UserModel, kiểm tra điều kiện:
        //      - Id phải tồn tại trong UserEntity (nếu không thỏa return code 404)
        //      - ngày sinh không được nhập quá Datatime.Now (nếu không thỏa return code 400)
        //      - PhoneNumber phải đúng 10 ký tự (nếu không thỏa return code 400)
        //      - Balance hoặc TotalProduct >= 0 (nếu không thỏa return code 400)
        //  Nếu có điều kiện nào vi phạm thì không update và return code 400 cho client.
        [HttpPut]
        [Route("/api/[controller]/update-user")]
        public IActionResult UpdateUser([FromBody] UserModel user)
        {
            try
            {
                var existedUsers = _userService.GetAllUsers();
                UserModel existedUser = existedUsers.FirstOrDefault(x => x.Id.Equals(user.Id));

                if (existedUser == null)
                {
                    return NotFound();
                }

                if (user.DateOfBirth > DateTime.Now || user.PhoneNumber.Trim().Length != 10 || user.Balance < 0 || user.TotalProduct < 0)
                {
                    return BadRequest("- Ngày sinh không được nhập quá Datatime.Now" +
                        "\n- PhoneNumber phải đúng 10 ký tự" +
                        "\n- Balance hoặc TotalProduct >= 0");
                }

                if(!user.PhoneNumber.Trim().Equals(existedUser.PhoneNumber))
                {
                    var duplicatedPhone = existedUsers.FirstOrDefault(x => x.PhoneNumber.Trim().Equals(user.PhoneNumber));
                    
                    if(duplicatedPhone != null)
                    {
                        return BadRequest("Số điện thoại này đã được sử dụng");
                    }
                }

                var currentUsers = _userService.UpdateUser(user);

                return Ok(currentUsers.FirstOrDefault(x => x.PhoneNumber.Equals(user.PhoneNumber)));
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        // 10. Viết API Delete user với input là Id. User sẽ được delete nếu thỏa các điều kiện sau:
        //      - Id tồn tại trong UserEntity (nếu không thỏa return code 400)
        //      - Balance của user bằng 0 (nếu không thỏa return code 400)

        [HttpDelete]
        [Route("/api/[controller]/delete-user/{id}")]
        public IActionResult DeleteUser([FromRoute] string id)
        {
            try
            {
                var existedUsers = _userService.GetAllUsers();
                UserModel existedUser = existedUsers.FirstOrDefault(x => x.Id.Equals(id));

                if (existedUser == null || existedUser.Balance == 0)
                {
                    return BadRequest("Xóa thất bại: User không tồn tại hoặc số dư vẫn còn nhiều !");
                }
                _userService.DeleteUser(existedUser);
                return Ok();
            }catch(Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

  


    }


}


















// 13. Viết function tính tổng tiền của một order với input là  CreateOrderResponse model.

// (Lưu ý: các API phải được đặt trong try/catch, nếu APi lỗi sẽ return về code 500)

// 11. Tạo CreateOrderRequest model để nhập các thông tin cho việc insert một order mới theo json:
//{
//  "userId": "string",
//  "items": [
//    {
//      "caffeeId": "string",
//      "volumn": 0,
//    }
//  ]
//}

// 12. Tạo CreateOrderResponse model để trả về thông tin cho việc insert một order mới theo json:
//{
//  "userId": "string",
//  "orderId": "string",
//  "total": 0,
//  "items": [
//    {
//      "name": "string",
//      "unitPrice": 0,
//      "volumn": 0,
//      "discount": 0,
//      "price": 0
//    }
//  ]
//}


// 14. Viết API tạo 1 order mới với input là CreateOrderRequest model được tạo ở bài 11. Yêu cầu:
//        - Kiểm tra nếu userId nếu không tồn tại thì return code 404
//        - Kiểm tra nếu list Items là rỗng thì return code 400
//        - khởi tạo một order và insert vào OrderEntity với status là WaitToPay
//        - lần lượt insert các item vào OrderItemEntity
//        - Dùng function trong bài 13 để tính TotalPrice và update vào OrderEntity
//        - Return code 200 theo CreateOrderResponse model.

// 15. Tạo UpdateOrderRequest model để nhập các thông tin cho việc update một order theo json:
//{
//  "orderId": "string",
//  "addItems": [
//    {
//      "caffeeId": "string",
//      "volumn": 0,
//    }
//  ],
//  "updateItems": [
//    {
//      "orderItemId": "string",
//      "volumn": 0
//    }
//  ],
//  "removeItems": [
//    {
//      "orderItemId": "string"
//    }
//  ]
//}

// 16. Viết API update 1 order với input là UpdateOrderRequest model được tạo ở bài 15. Yêu cầu:
//       - Kiểm tra nếu orderId nếu không tồn tại thì return code 404
//       - Kiểm tra Order, nếu Status không phải là WaitToPay => return code 400
//       - Thực hiện thêm các items từ list addItems vào OrderItemEntity
//       - Thực hiện update các items và tính lại ItemPrice từ list updateItems vào OrderItemEntity
//       - Thực hiện xóa các items từ list removeItems vào OrderItemEntity bằng cách thay đổi IsDeleted = true
//       - Thực hiện tính toán lại Totalprice và cập nhật lại OrderEntity.
//       - Return code 202

// 17 Viết API approved order với input là orderId. Yêu cầu:
//       - Kiểm tra nếu orderId nếu không tồn tại thì return code 404
//       - Kiểm tra Order, nếu Status không phải là WaitToPay => return code 400
//       - Kiểm tra Balance của User, nếu Balance < TotalPrice=> return code 400
//       - Thực hiện tính toán lại balance cho user
//       - Update lại status cho Order => Success
//       - Return code 200

