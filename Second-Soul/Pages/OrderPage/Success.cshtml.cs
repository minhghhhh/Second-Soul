using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
    public class SuccessModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IOrderBusiness _orderBusiness;
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        public SuccessModel(IUserBusiness userBusiness, IOrderBusiness orderBusiness, IShoppingCartBusiness shoppingCartBusiness, IProductBusiness productBusiness, IOrderDetailBusiness orderDetailBusiness)
        {
            _userBusiness = userBusiness;
            _orderBusiness = orderBusiness;
            _productBusiness = productBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
            _orderDetailBusiness = orderDetailBusiness;
        }
        public async Task<IActionResult> OnGet(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage("/Index");
            }
            var user = await _userBusiness.GetUserByToken(token);
            if (user == null) 
            {
                return RedirectToPage("/Index");
            }
            var result = await _orderBusiness.GetSinglePendingOrder(user.UserId);
            if (result == null || !(result.Status > 0) || result.Data == null) 
            {
                return RedirectToPage("/Index");
            }
            var order = (Order)result.Data;
            order.Status = "Completed";
            result = await _orderBusiness.Update(order);
            if (!(result.Status > 0))
            {
                return RedirectToPage("/Index");
            }
            var details = await _orderDetailBusiness.GetDetailsByOrderId(order.OrderId);
            foreach (var item in details) { 
                result = await _productBusiness.GetById(item.ProductId);
                var product = (Product)result.Data;
                product.IsAvailable = false;
                await _productBusiness.Update(product);
                await _shoppingCartBusiness.RemoveFromCart(user.UserId, product.ProductId);
            }
            double balance = (order.TotalAmount - 30000) *80 / 100;
            user.Wallet = (int)Math.Ceiling(balance);
            await _userBusiness.Update(user);
            return RedirectToPage("Index");
        }
    }
}
