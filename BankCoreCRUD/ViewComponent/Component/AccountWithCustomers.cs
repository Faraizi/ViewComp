using Microsoft.AspNetCore.Mvc;
using BankCoreCRUD.Models;

namespace BankCoreCRUD.ViewComponents.Component
{
    public class AccountWithCustomers : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Account? account, Customer? customer)
        {
            if (account != null && customer != null)
            {
                if (account.Customers is null)
                {
                    account.Customers = new List<Customer>() { customer };
                }
                else
                {
                    account.Customers.Add(customer);
                }
                return View(account);
            }
            else
            {
                return View(new Account());
            }
        }
    }
}
