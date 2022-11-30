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
    public class PaymentTypeController : Controller
    {

        private readonly DBContext _db;
        private readonly IUserService _userService;
        private readonly ISetupRepository _setupRepository;

        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public PaymentTypeController(DBContext db, IUserService userService, ISetupRepository setupRepository /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */)
        {
            _db = db;
            _userService = userService;
            _setupRepository = setupRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }


        public async Task<IActionResult> IndexAsync()
        {           
            ViewBag.PayTypes =await _setupRepository.GetAllPaymetTypes();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PayType model, bool isDelete)
        {
            if (ModelState.IsValid)
            {
                if (model.PayTypeId > 0)
                {
                    if (isDelete)
                    {
                        var data =await _setupRepository.GetPaymetTypeById(model.PayTypeId);
                        if (data!=null)
                        {
                            /// go to archive data
                            data.IsDelete = true;
                            _db.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            ModelState.AddModelError("", model.PayTypeName + " client not found or already deleted!!");
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
                    _db.Add(model);
                }
                await _db.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var payType = await _setupRepository.GetPaymetTypeById(id);  

            if (payType == null)
            {
                return NotFound();
            }

            return View("Create", payType);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentTypeActivateAsync()
        {
            string userId = _userService.GetUserId();
            var payTypes = await _setupRepository.GetAllPaymetTypes();

            var checkPayment= payTypes
                .Select(a => new PayTypeViewModel
                {
                    PayTypeId = a.PayTypeId,
                    PayTypeName=a.PayTypeName,
                    Icon=a.Icon,
                    IsCustom=a.IsCustom,
                    IsActive=a.IsActive,
                    IsUsing =  _db.UserPayTypeControl.Any(b=>b.UserId == userId && b.PayTypeId == a.PayTypeId && b.IsUsing),
                })
                .ToList();
                
            return View(checkPayment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentTypeControlAsync(bool isUsing, int payTypeId)
        {
            try
            {
                var userid = _userService.GetUserId();
                var exist = await _db.UserPayTypeControl.FirstOrDefaultAsync(a => a.PayTypeId == payTypeId && a.UserId == userid);

                if (exist != null)
                {
                    exist.IsUsing = isUsing;
                    _db.Entry(exist).State = EntityState.Modified;
                }
                else
                {
                    var createNew = new UserPayTypeControl
                    {
                        UserId = userid,
                        PayTypeId = payTypeId,
                        IsUsing = isUsing,
                    };

                    _db.Add(createNew);
                }

                await _db.SaveChangesAsync();

                return Json(new { isSuccess=true});
            }
            catch (Exception ex)
            {

                return Json(new { isSuccess = true, message=ex.Message });
            }
           


        }

    }
}
