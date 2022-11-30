using Deal.Models;
using Deal.Repository;
using Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Controllers
{
    //[Authorize]
    public class PaymentController : Controller
    {
        private readonly DBContext _db;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ISetupRepository _setupRepository;
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(DBContext db, IUserService userService, IConfiguration configuration, IPaymentRepository paymentRepository, ISetupRepository setupRepository)
        {
            _db = db;
            _userService = userService;
            _configuration = configuration;
            _paymentRepository = paymentRepository;
            _setupRepository = setupRepository;
        }


        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var key = Request.RouteValues["id"].ToString();

            if (String.IsNullOrEmpty(key))
            {
                return NoContent();
            }

            ViewBag.TransactionId = key;

            var data = _paymentRepository.GetTransactionById(Guid.Parse(key));

            if (data != null)
            {
                var payTypes = await _setupRepository.GetActivePaymetTypes();

                var specificUser = await _db.UserPayTypeControl.Where(a=>a.UserId == data.UserIdFor && a.IsUsing).ToListAsync();

                payTypes = (from p in payTypes
                          join sp in specificUser on p.PayTypeId equals sp.PayTypeId
                          select p).Distinct().ToList();

                if (data.IsCustomPayment)
                {
                    ViewBag.PayTypes = payTypes.Where(a => a.IsActive == true || a.IsCustom == data.IsCustomPayment).ToList();
                }
                else
                {
                    ViewBag.PayTypes = payTypes.Where(a => a.IsActive == true && a.IsCustom == data.IsCustomPayment).ToList();
                }
                
            }

            return View();
        }

        [HttpGet]
        public IActionResult PaymentGateway(int? payTypeId, Guid transactionId)
        {
            if (payTypeId == null || String.IsNullOrEmpty(transactionId.ToString()))
            {
                return Json(new { success = false, message = "Invalide info" });
            }
            var paymentType = _db.PayType.FirstOrDefault(a => a.PayTypeId == payTypeId);
            var transcation = _db.PaymentTransaction.FirstOrDefault(a => a.TransactionId == transactionId);

            if (transcation == null)
            {
                return Json(new { success = false, message = "Your transaction info not found!!" });
            }
            else
            {
                transcation.PayTypeId = payTypeId;
                transcation.IsCustomPayment = paymentType.IsCustom;
                transcation.Status = "Type Selected";

                _db.Update(transcation);
                _db.SaveChanges();

                
                var link = "";
                if (paymentType.IsCustom)
                {
                    link = Url.ActionLink("Create", "CustomPayment", new {  id=transactionId });
                    return Json(new { success = true,ajax=false, link, message = "Go to pay" });
                }
                else
                {
                    link = Url.ActionLink("PayWithNagad", "Nagad", new { amount = transcation.Amount, transactionId, isSandbox = true });
                    return Json(new { success = true, ajax = true, link, message = "Go to pay" });
                }                
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult LinkPaymentReceive(LinkPaymentReceive model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var keyCheck = _paymentRepository.GetPaymentLinkByKey(model.Key);

                    if (keyCheck == null)
                    {
                        return Json(new { success = false, message = "Payment info not found!!" });
                    }

                    var transaction = _paymentRepository.CreateLinkTransaction(model, keyCheck, "On Create");

                    if (transaction != null)
                    {
                        string link = Url.ActionLink("Index", "Payment", new { id = transaction.TransactionId });

                        return Json(new { success = true, link, message = "Go to payment" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Payment can not created" });
                    }
                }
                return Json(new { success = false, message = "Info invalid!!" });

            }
            catch (Exception ex)
            {
                //throw ex;
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ChekoutPaymentReceive(CheckoutPaymentReceive model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var key = HttpContext.Session.GetString("key");
                    //if (key != model.Key) return NoContent();

                    //var keyCheck = _db.S_CheckoutMaster.FirstOrDefault(c => c.Key == model.Key);
                    var keyCheck = _paymentRepository.GetCheckoutByKey(model.Key);

                    if (keyCheck == null)
                    {
                        return Json(new { success = false, message = "Checkout info not found" });
                    }

                    var transaction = _paymentRepository.CreateCheckoutTransaction(model, keyCheck, "On Create");

                    if (transaction != null)
                    {
                        string link = Url.ActionLink("Index", "Payment", new { id = transaction.TransactionId });

                        return Json(new { success = true, link = link, message = "Go to payment" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Payment can not create" });
                    }
                }
                return Json(new { success = false, message = "Info invalid!!" });

            }
            catch (Exception ex)
            {
                //throw ex;
                return Json(new { success = false, message = ex.Message });
            }
        }


        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult MerchantCallBack(CheckoutPaymentReceive model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //var key = HttpContext.Session.GetString("key");
        //            //if (key != model.Key) return NoContent();

        //            //var keyCheck = _db.S_CheckoutMaster.FirstOrDefault(c => c.Key == model.Key);
        //            var keyCheck = _paymentRepository.GetCheckoutByKey(model.Key);

        //            if (keyCheck == null)
        //            {
        //                return Json(new { success = false, message = "Checkout info not found" });
        //            }

        //            var transaction = _paymentRepository.CreateCheckoutTransaction(model, keyCheck,"On Create");

        //            if (transaction!=null)
        //            {
        //                string link = Url.ActionLink("Index", "Payment", new { id = transaction.TransactionId });

        //                return Json(new { success = true, link = link, message = "Go to payment" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "Payment can not create" });
        //            }
        //        }
        //        return Json(new { success = false, message = "Info invalid!!" });

        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //        return Json(new { success = false, message = ex.Message });
        //    }           
        //}


        [AllowAnonymous]
        [HttpPost]
        public IActionResult QRPaymentReceive(QRPaymentReceive model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var keyCheck = _paymentRepository.GetQRInfoByKey(Guid.Parse(model.Key));

                    if (keyCheck == null)
                    {
                        return Json(new { success = false, message = "QR info not found!!" });
                    }

                    var transaction = _paymentRepository.CreateQRTransaction(model, keyCheck, "On Create");

                    if (transaction != null)
                    {
                        string link = Url.ActionLink("Index", "Payment", new { id = transaction.TransactionId });

                        return Json(new { success = true, link, message = "Go to payment" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Payment can not created" });
                    }
                }
                return Json(new { success = false, message = "Info invalid!!" });

            }
            catch (Exception ex)
            {
                //throw ex;
                return Json(new { success = false, message = ex.Message });
            }
        }











    }
}
