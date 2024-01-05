using Microsoft.AspNetCore.Mvc;

namespace DocnetCorePractice.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
