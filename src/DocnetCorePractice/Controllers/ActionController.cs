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
using System.Globalization;

namespace DocnetCorePractice.Controllers
{
    [Route("/api/[controller]/")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public readonly IUserService _userService;
        public readonly ICaffeService _caffeService;
        private readonly ILogger _logger;
        private readonly string DobPattern = "dd-mm-yyy";
        DateTime today;
      

        public ActionController(IServiceProvider serviceProvider)
        {
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _caffeService = serviceProvider.GetRequiredService<ICaffeService>();
            _logger = Log.Logger;
            DateTime.TryParse(DateTime.Now.ToShortDateString(), out today);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(RequestLoginModel request)
        {
            return Ok(_authenticationService.Authenticator(request));
        }

        [ApiKey]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getallusers")]
        public IActionResult GetAllUser()
        {
            var result = _userService.GetAllActiveUsers();
            return Ok(result);
        }

        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] UserModel model)
        {
            var result = _userService.AddUser(model);
            return Ok(result);
        }

        // 6.Viết API insert thêm user mới với input là UserModel, kiểm tra điều kiện:
        //      - PhoneNumber và Id không tồn tại trong UserEntity (nếu không thỏa return code 400)
        //      - ngày sinh không được nhập quá Datatime.Now (nếu không thỏa return code 400)
        //      - PhoneNumber phải đúng 10 ký tự (nếu không thỏa return code 400)
        //      - Balance hoặc TotalProduct >= 0 (nếu không thỏa return code 400)
        //  Nếu có điều kiện nào vi phạm thì không insert và return failed.

        [HttpPost]
        [Route("new-user")]
        public IActionResult AddNewUser([FromBody] UserModel user)
        {
            try
            {
                Console.WriteLine("Date of birth: " + user.DateOfBirth.ToShortDateString());
               
                if (DateTime.Compare(user.DateOfBirth, today) >= 0 || user.PhoneNumber.Trim().Length != 10 || user.Balance < 0 || user.TotalProduct < 0)
                {
                    return BadRequest("- Ngày sinh không được nhập quá hôm nay " + DateTime.Now.Date.ToString() +
                       "\n- Ngày sinh phải định dạng dd-mm-yyyy" +
                        "\n- PhoneNumber phải đúng 10 ký tự" +
                        "\n- Balance hoặc TotalProduct >= 0");
                }

                var existedUsers = _userService.GetAllUsers();

                var existedUser = existedUsers.FirstOrDefault(x =>
                    x.PhoneNumber.Equals(user.PhoneNumber)
                    &&
                    !x.Id.Equals(user.Id)
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
        [Route("users")]
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
        [Route("users/birthday-role")]
        public IActionResult GetAllUserByDateOfBirthAndRole([FromQuery] int month, [FromQuery] Roles role)
        {
            try
            {
                var existedUsers = _userService.GetAllUserEnities();
                var filteredUsers = existedUsers.Where(x => x.DateOfBirth.Month == month && x.Role.Equals(role));
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
        [Route("update-user")]
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

                if (DateTime.Compare(user.DateOfBirth, today) >= 0 || user.PhoneNumber.Trim().Length != 10 || user.Balance < 0 || user.TotalProduct < 0)
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
        [Route("delete-user/{id}")]
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

















