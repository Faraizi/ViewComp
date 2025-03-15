using BankCoreCRUD.Models;
using BankCoreCRUD.ViewComponents.Component;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankCoreCRUD.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AccountController(AccountContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Account.Include(c => c.Customers!).ThenInclude(p => p.Transaction).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["Transactions"] = new SelectList(_context.Transaction, "TranID", "TranName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccID,AccType,Customers")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.AcctID == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account != null)
            {
                _context.Account.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> PostForViewComponent(Account account, Customer customer)
        {
            ViewData["TranID"] = new SelectList(_context.Transaction, "TranID", "TranName", customer.CustID);
            Transaction? transaction = await _context.Transaction.FindAsync(customer.TranID);
            if (account is null)
            {
                return ViewComponent(typeof(AccountWithCustomers), new object[] { new Account(), new Customer() });
            }
            else
            {
                if (account.Customers is not null)
                {
                    foreach (Customer p in account.Customers!)
                    {
                        if (p.TranID > 0)
                        {
                            Transaction aModel = await _context.Transaction.SingleAsync(m => m.TranID == p.TranID);
                            p.Transaction = aModel;
                        }
                    }
                }
                if (customer is null)
                {
                    return ViewComponent(typeof(AccountWithCustomers), new object[] { account, new Customer() });
                }
                else
                {
                    customer.Transaction = transaction;
                    return ViewComponent(typeof(AccountWithCustomers), new { account, transaction });
                }
            }
        }

        [HttpPost]
        public JsonResult UploadImage(IFormFile file)
        {
            if (file != null)
            {
                string fileName = Path.Combine(_hostingEnvironment.WebRootPath, "images", file.FileName);
                file.CopyTo(new FileStream(fileName, FileMode.Create));
            }
            string url = HttpContext.Request.GetEncodedUrl();
            return Json("https://localhost:7011/images/" + file!.FileName);
        }

        private bool CategoryExists(int id)
        {
            return _context.Account.Any(e => e.AcctID == id);
        }
    }
}
