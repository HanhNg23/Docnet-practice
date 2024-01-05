using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;
using DocnetCorePractice.Service;
using DocnetCorePractice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.ComponentModel.DataAnnotations;
using ILogger = Serilog.ILogger;

namespace DocnetCorePractice.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CaffeController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        public readonly IUserService _userService;
        public readonly ICaffeService _caffeService;
        private readonly ILogger _logger;
        public CaffeController(IServiceProvider serviceProvider)
        {
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _caffeService = serviceProvider.GetRequiredService<ICaffeService>();
            _logger = Log.Logger;
        }


        // 1. Viết API insert thêm caffe mới vào menu với input là CaffeModel, kiểm tra điều kiện:
        //      - Name và Id không tồn tại trong CaffeEntity (nếu không thỏa return code 400)
        //      - Price hoặc discount >= 0 (nếu không thỏa return code 400)
        //   Nếu có điều kiện nào vi phạm thì không insert và return failed.

        [HttpPost]
        [Route("new-caffe")]
        public IActionResult AddNewCaffe([FromBody] CaffeModel caffe)
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
                Name = caffe.Name,
                Price = caffe.Price,
                Discount = caffe.Discount,
                CreateTimes = DateTime.Now,
                CreateUser = "HA",
                IsActive = true,
                Type = caffe.Type,
            });
           

            if (addedCaffe == null)
            {
                return BadRequest("Added Failed");
            }

            return Ok(addedCaffe);
        }

        // 2. Viết API get all caffe có IsActive = true theo CaffeModel.
        // nếu không có caffe nào thì return code 204
        [HttpGet]
        [Route("active-caffe")]
        public IActionResult GetAllActiveCaffe()
        {
            var activeCaffe = _caffeService.GetAllCaffeEntities().Where(x => x.IsActive);
            if (activeCaffe.Count() <= 0)
            {
                return NoContent();
            }
            return Ok(activeCaffe);
        }

        // 3. Viết API get detail caffe có input là Id với điều kiện isActive bằng true.
        // Nếu không có user nào thì return code 204

        [HttpGet]
        [Route("caffe/{id}")]
        public IActionResult GetActiveCaffeById([FromRoute] string id)
        {
            try
            {
                var activeCaffe = _caffeService.GetAllCaffeEntities().Where(x => x.Id.Equals(id) && x.IsActive && x.CreateUser != null);
                if (activeCaffe.Count() <= 0)
                {
                    return NoContent();
                }
                return Ok(activeCaffe);
            }
            catch (Exception ex)
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
        [Route("update-caffe/{id}")]
        public IActionResult UpdateCaffeById([FromRoute] string id, [FromBody] RequestUpdateCaffe caffe)
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
            catch (Exception ex)
            {
                _logger.Debug(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        // 5. Viết API Delete caffe với input là Id. Caffe sẽ được delete nếu thỏa điều kiện sau:
        //  - Id tồn tại trong CaffeEntity (nếu không thỏa return code 400)

        [HttpDelete]
        [Route("delete-caffe/{id}")]
        public IActionResult DeleteCaffeById([FromRoute] string id)
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
    }
}
