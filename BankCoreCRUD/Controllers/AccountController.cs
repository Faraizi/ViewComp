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
            ViewData["Models"] = new SelectList(_context.Transaction, "TranID", "AccType");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AcctID,Branch,Customers")] Account account)
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
            ViewData["TranID"] = new SelectList(_context.Transaction, "TranID", "AccType", customer.CustID);
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
            return Json("http://localhost:28962//images/" + file!.FileName);
        }
        private bool CategoryExists(int id)
        {
            return _context.Account.Any(e => e.AcctID == id);
        }
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(c => c.Customers)
                    .ThenInclude(p => p.Transaction)
                .FirstOrDefaultAsync(m => m.AcctID == id);

            if (account == null)
            {
                return NotFound();
            }

            ViewData["Models"] = new SelectList(_context.Transaction, "TranID", "AccType");
            return View(account);
        }

        [HttpGet]
        public IActionResult GetProductForEdit(int custID)
        {
            var customer = _context.Customer
                .Include(p => p.Transaction)
                .FirstOrDefault(p => p.CustID == custID);

            if (customer == null)
            {
                return NotFound();
            }

            // Prepare models dropdown for the form
            ViewBag.Models = _context.Transaction.Select(m => new SelectListItem
            {
                Value = m.TranID.ToString(),
                Text = m.AccType
            }).ToList();

            return PartialView("_EditCustomer", customer);
        }
        [HttpPost]
        public IActionResult UpdateProduct(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = _context.Customer.Find(customer.CustID);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update product properties
                    existingProduct.CustName = customer.CustName;
                    existingProduct.Balance = customer.Balance;
                    existingProduct.DOB = customer.DOB;
                    existingProduct.IsActive = customer.IsActive;
                    existingProduct.ImageUrl = customer.ImageUrl;
                    existingProduct.TranID = customer.TranID;

                    _context.Update(existingProduct);
                    _context.SaveChanges();

                    // Get updated category with products
                    var account = _context.Account
                        .Include(c => c.Customers)
                        .ThenInclude(p => p.Transaction)
                        .FirstOrDefault(c => c.AcctID == existingProduct.AcctID);

                    return ViewComponent("CategoryWithProducts", new { account = account, customer = new Customer() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to update product: " + ex.Message);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
