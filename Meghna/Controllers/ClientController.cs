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

namespace Deal.Controllers
{
    public class ClientController : Controller
    {

        private readonly DBContext _db;
        private readonly IUserService _userService;
        private readonly ISetupRepository _setupRepository;

        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public ClientController(DBContext db, IUserService userService, ISetupRepository setupRepository /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */)
        {
            _db = db;
            _userService = userService;
            _setupRepository = setupRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }


        public IActionResult Index()
        {           
            ViewBag.ClientList = _setupRepository.GetClients();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client model, bool isDelete)
        {
            if (ModelState.IsValid)
            {
                if (model.ClientId > 0)
                {
                    if (isDelete)
                    {
                        var data = _setupRepository.GetClientById(model.ClientId);
                        if (data!=null)
                        {
                            /// go to archive data
                            data.IsDelete = true;
                            _db.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            ModelState.AddModelError("", model.Name + " client not found or already deleted!!");
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
                    _db.Client.Add(model);
                }
                await _db.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var client = _setupRepository.GetClientById(id);  // _db.Client.Where(a => a.ClientId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

            if (client == null)
            {
                return NotFound();
            }

            return View("Create", client);
        }

       

    }
}
