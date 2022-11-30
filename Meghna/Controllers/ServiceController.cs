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
using System.Drawing;
using QRCoder;
using System.IO;

namespace Deal.Controllers
{
    public class ServiceController : Controller
    {

        private readonly DBContext _db;
        private readonly IUserService _userService;
        private readonly ISetupRepository _setupRepository;
        private readonly IPaymentRepository _paymentRepository;
        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public ServiceController(DBContext db, IUserService userService, ISetupRepository setupRepository, IPaymentRepository paymentRepository /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */)
        {
            _db = db;
            _userService = userService;
            _setupRepository = setupRepository;
            _paymentRepository = paymentRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }

        /// <summary>
        /////////////////////   Payment link region
        /// </summary>
        /// <returns></returns>
        #region Paymentlink

        public IActionResult PaymentLink()
        {
            var data = _paymentRepository.GetPaymentLinks();
            
            return View(data);
        }


        [HttpGet]
        public IActionResult PaymentLinkCreate()
        {
            ViewBag.Currency = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PaymentLinkCreate(PaymentLink model, bool isDelete)
        {
            if (ModelState.IsValid)
            {
                if (model.LinkId > 0)
                {
                    if (isDelete)
                    {
                        var data = _db.S_PaymentLink.Where(a => a.UserId == _userService.GetUserId() && a.LinkId == model.LinkId).FirstOrDefault();
                        if (data != null)
                        {
                            /// go to archive data
                            data.IsDelete = true;
                            _db.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            ModelState.AddModelError("", model.Title + " link not found or already deleted!!");
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
                    if (String.IsNullOrEmpty(model.CustomLink))
                    {
                        string key = Guid.NewGuid().ToString();
                        var link = Url.ActionLink("PayWithLink", "Service",new {id=key });

                        model.Key = key;
                        model.CustomLink = link;
                    }
                    _db.S_PaymentLink.Add(model);
                }
                await _db.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction("PaymentLink");
            }
            else
            {
                ViewBag.Currency = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name",model.CurrId);
                return View(model);
            }            
        }


        [HttpGet]
        public IActionResult PaymentLinkEdit(int id)
        {
            if (id == 0) return NotFound(); 

            var link = _paymentRepository.GetPaymentLinkById(id); // _db.S_PaymentLink.Where(a => a.LinkId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

            if (link == null) return NotFound(); 

            ViewBag.Currency = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name", link.CurrId);
            return View("PaymentLinkCreate", link);
        }


        public IActionResult LinkGenerate()
        {
            var key = Guid.NewGuid().ToString();

            var link = Url.ActionLink("PayWithLink", "Service",new {id=key });

            return Json(new { url = link, key });
        }
       

        [AllowAnonymous]
        public IActionResult PayWithLink()
        {
            var key = Request.RouteValues["id"].ToString();

            if (String.IsNullOrEmpty(key))
            {
                return NoContent();
            }

            var keyInfo = _paymentRepository.GetPaymentLinkByKey(key);

            if (keyInfo == null)
            {
                return NotFound();
            }

            ViewBag.Key = key;
            return View(keyInfo);
        }
        #endregion payment lind end----------------------

        /// <summary>
        ///             Checkout setup and checkout payment
        /// </summary>
        /// <returns></returns>
        #region Checkout payment
        [HttpGet]
        public IActionResult Checkout()
        {
            var data = _paymentRepository.GetCheckoutList();
            return View(data);
        }

        [HttpGet]
        public IActionResult CheckoutCreate()
        {
            //ViewBag.Clients = new SelectList(_setupRepository.GetClients(), "ClientId", "Name");
            ViewBag.Products = new SelectList(_setupRepository.GetProducts(), "ProductId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutCreate(CheckoutMaster model)
        {
            try
            {
               // var errors = ModelState.Where(x => x.Value.Errors.Any())
               //.Select(x => new { x.Key, x.Value.Errors });
                if (ModelState.IsValid)
                {
                    if (model.MasterId > 0)
                    {
                        model.CheckoutDetailses.ForEach((item) =>
                        {
                            if (item.DetaileId > 0)
                                _db.Entry(item).State = EntityState.Modified;  /// for update details                        
                            else
                                _db.Add(item); // for add new details
                        });
                        _db.Entry(model).State = EntityState.Modified;  /// for update master        
                    }
                    else
                    {
                        model.Key = Guid.NewGuid().ToString();
                        _db.S_CheckoutMaster.Add(model);
                    }
                    await _db.SaveChangesAsync();

                    return Json(new { Success = 1 });
                }

                return Json(new { Success = 2, Message = "Model state not valid" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = 3, Message = ex.Message });
            }

        }


        [HttpGet]
        public IActionResult CheckoutEdit(int id)
        {
            if (id == 0) return NotFound();

            var checkout = _paymentRepository.GetCheckoutById(id); // _db.S_PaymentLink.Where(a => a.LinkId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

            if (checkout == null) return NotFound();

            ViewBag.Products = new SelectList(_setupRepository.GetProducts(), "ProductId", "Name");
            return View("CheckoutCreate", checkout);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PayWithCheckout()
        {
            var key = Request.RouteValues["id"].ToString();

            if (String.IsNullOrEmpty(key)) return NoContent();

            var keyInfo = _paymentRepository.GetCheckoutByKey(key);

            if (keyInfo == null) return NotFound();

            HttpContext.Session.SetString("key",key);

            return View(keyInfo);
        }

        #endregion

        
        #region QRCode payment

        public async Task<IActionResult> QRPayment()
        {            
            var getKey =await _db.S_QRInfo.FirstOrDefaultAsync(a => a.UserId == _userService.GetUserId());
            if (getKey == null)
            {
                getKey = await _paymentRepository.CreateQRInfo();                
            }

            var qrUrl = Url.ActionLink("PayWithQR", "Service", new {id= getKey.Key });
            ViewBag.QRUrl = qrUrl;
            // qr generator
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            getKey.QRCode = BitmapToBytesCode(qrCodeImage);

            return View(getKey);
        }

        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult PayWithQR()
        {
            var key = Request.RouteValues["id"].ToString();

            if (String.IsNullOrEmpty(key)) return NoContent();

            //ViewBag.Currency = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name");
            ViewBag.Currency = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name");
            //var keyInfo = _paymentRepository.GetCheckoutByKey(key);

            //if (keyInfo == null) return NotFound();

            // HttpContext.Session.SetString("key", key);
            ViewBag.Key = key;
            return View();
        }

        #endregion


        #region   custom payment

        public IActionResult CustomPayment()
        {
           
            return View();
        }

        #endregion

    }
}
