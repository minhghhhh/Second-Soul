using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
    public class CancelPaymentModel : PageModel
    {
        public int id {  get; set; } = 0;
        public void OnGet(int oid)
        {
            id = oid;
        }
    }
}
