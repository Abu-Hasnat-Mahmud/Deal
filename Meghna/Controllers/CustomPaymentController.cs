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
    public class CustomPaymentController : Controller
    {

        private readonly DBContext _db;
        private readonly IUserService _userService;
        private IFileService _fileService;
        private ISetupRepository _setuprepository;
        private IPaymentRepository _paymentRepository;
        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public CustomPaymentController(DBContext db, ISetupRepository setupRepository, IUserService userService, IFileService fileService /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */, IPaymentRepository paymentRepository)
        {
            _db = db;
            _userService = userService;
            _fileService = fileService;
            _setuprepository = setupRepository;
            _paymentRepository = paymentRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.PendingCustomPayments =await _paymentRepository.GetPendingCustomPayments();
            return View();
        }

        [HttpGet]
        public IActionResult Create(Guid id)
        {
            var trans = _paymentRepository.GetTransactionById(id);

            if (trans == null) return NotFound();

            CustomPayment customPayment = new CustomPayment
            {
                CurrId = trans.CurrId,
                TransactionId = trans.TransactionId,
                AmountPaid = trans.Amount,                
            };

            ViewBag.Success = false;
            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name", trans.CurrId);

            return View(customPayment);
        } 
        
        
        [HttpPost]
        public async Task<IActionResult> Create(CustomPayment model)
        {
            if (ModelState.IsValid)
            {
                var trans = _paymentRepository.GetTransactionInfoById(model.TransactionId);
                if (model.Id > 0)
                {
                    
                }
                else
                {                   
                    model.PaymentDate = DateTime.Now;
                    model.UserIdFor = trans.CheckoutMaster != null ? trans.CheckoutMaster.UserId : trans.PaymentLink.UserId;
                    _db.CustomPayment.Add(model);
                }
                await _db.SaveChangesAsync();

                // upload file
                if (model.File != null)
                {
                    model.FilePath = _fileService.CustomPaymentFile(model);
                    _db.Update(model);                    
                }
                
                if (trans!=null)
                {
                    trans.PaymentDate = model.PaymentDate;
                    trans.Status = "Custom Payment";
                    _db.Update(trans);
                }

                await _db.SaveChangesAsync();

                ModelState.Clear();
                ViewBag.Success = true;
                return View();
            }
            ViewBag.Success = false;
            ViewBag.Currency = new SelectList(_setuprepository.GetCurrencies(), "CurrId", "Name",model.CurrId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptDocumentAsync(int id,bool accept)
        {
            try
            {
                var custom = await _paymentRepository.GetCustomPaymentById(id);

                if (custom != null)
                {
                    if (accept)
                    {
                        custom.IsReject = false;
                        custom.IsAccept = true;
                    }
                    else
                    {
                        custom.IsReject = true;
                        custom.IsAccept = false;
                    }

                    var trans = _paymentRepository.GetTransactionById(custom.TransactionId);
                    trans.Status = "Success";
                    trans.Amount = custom.AmountPaid;
                    trans.PaymentDate = custom.PaymentDate;

                    _db.Update(custom);
                    _db.Update(trans);

                    await _db.SaveChangesAsync();

                    return Json(new { success = true, message = "ok" });
                }
                else
                {
                    return Json(new { success = false, message = "Payment info not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            
        }


    }
}
