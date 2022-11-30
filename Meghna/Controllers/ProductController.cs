using Deal.Models;
using Deal.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Deal.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Deal.Controllers
{
    public class ProductController : Controller
    {

        private readonly DBContext _db;
        private readonly IUserService _userService;
        private IFileService _fileService;
        private ISetupRepository _setuprepository;
        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public ProductController(DBContext db, ISetupRepository setupRepository, IUserService userService, IFileService fileService /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */)
        {
            _db = db;
            _userService = userService;
            _fileService = fileService;
            _setuprepository = setupRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }


        public IActionResult Index()
        {
            var data = _setuprepository.GetProducts();// _db.Product.Include(a => a.Currency).Where(a => a.UserId == _userService.GetUserId()).ToList();
            ViewBag.ProductList = data;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, bool isDelete)
        {
            if (ModelState.IsValid)
            {
               

                if (model.ProductId > 0)
                {
                    if (isDelete)
                    {
                        var data = _db.Product.Where(a => a.UserId == _userService.GetUserId() && a.ProductId == model.ProductId).FirstOrDefault();
                        if (data != null)
                        {
                            /// go to archive data
                            data.IsDelete = true;
                            _db.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name", model.CurrId);
                            ModelState.AddModelError("", model.Name + " product not found or already deleted!!");
                            return View(model);
                        }
                    }
                    else
                    {
                        _db.Entry(model).State = EntityState.Modified;
                    }
                }
                else
                {
                    _db.Product.Add(model);
                }
                await _db.SaveChangesAsync();

                // upload image
                if (model.ImageFile != null)
                {
                    model.Image = _fileService.ProductImageUpload(model);
                    _db.Update(model);
                    await _db.SaveChangesAsync();
                }
                
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name");
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Product = _setuprepository.GetProductById(id); //_db.Product.Where(a => a.ProductId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

            if (Product == null)
            {
                return NotFound();
            }
            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name", Product.CurrId);
            return View("Create", Product);
        }

        // Get single product id by ajax call
        [HttpGet]
        public IActionResult GetSingleProductAJAX(int productId)
        {
            var product = _setuprepository.GetProductById(productId);
            return product == null ? Json(null) : Json(product);
        }



    }
}
