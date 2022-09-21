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
using Microsoft.Data.SqlClient;
using Deal.Helpers;

namespace Deal.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class TransactionClearanceController : Controller
    {

        private readonly GTRDBContext _db;
        private readonly IUserService _userService;
        private readonly ISetupRepository _setupRepository;
        private readonly IPaymentRepository _paymentRepository;

        //private readonly IAccountRepository _accountRepository;
        //public IConfiguration _config { get; }

        //private readonly LinkGenerator _linkGenerator;
        public TransactionClearanceController(GTRDBContext db, IUserService userService, ISetupRepository setupRepository /*, LinkGenerator linkGenerator IConfiguration config,IAccountRepository accountRepository, */, IPaymentRepository paymentRepository)
        {
            _db = db;
            _userService = userService;
            _setupRepository = setupRepository;
            _paymentRepository = paymentRepository;
            //_accountRepository = accountRepository;
            // _config = config;
            // _linkGenerator = linkGenerator;
        }


        public IActionResult Index()
        {
            ViewBag.ClientList = _db.TransactionClearanceMaster
                .Include(a => a.ApplicationUserFor)
                .Include(a => a.Currency).Where(a=> !a.IsDelete).OrderByDescending(a=>a.PaymentDate).ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.CurrId = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name");
            ViewBag.UserIdFor = new SelectList(await _userService.GetUsersAsync(), "Id", "Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TransactionClearanceMaster model, bool isDelete)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.MasterId > 0)
                    {
                        if (isDelete)
                        {
                            var data = await _db.TransactionClearanceMaster.FindAsync(model.MasterId);
                            if (data != null)
                            {
                                /// go to archive data
                                data.IsDelete = true;
                                _db.Entry(data).State = EntityState.Modified;

                                var trans = await _db.TransactionClearanceDetails.Include(a=>a.PaymentTransaction).Where(a => a.MasterId == data.MasterId).ToListAsync();
                                trans.ForEach(a =>
                                {
                                    a.PaymentTransaction.IsClearTrans = false;
                                    _db.Entry(a.PaymentTransaction).State = EntityState.Modified;
                                });
                            }
                            else
                            {
                                return Json(new { IsSuccess = false, Message="Info not found!!" });
                            }
                        }
                        else
                        {
                            // get previous clear transaction
                            var preClearTrans =await _db.TransactionClearanceDetails.Where(a => a.MasterId == model.MasterId).ToListAsync();

                            //preClearTrans.Select()(a=> model.TransactionClearanceDetails.Contains(a.TransactionId))

                            foreach (var pre in preClearTrans)
                            {
                                var exist = model.TransactionClearanceDetails.FirstOrDefault(a => a.TransactionId == pre.TransactionId);
                                if (exist==null)
                                {
                                    var preTrans = _paymentRepository.GetTransactionById(pre.TransactionId);
                                    if (preTrans != null)
                                    {
                                        preTrans.IsClearTrans = false;
                                        _db.Entry(preTrans).State = EntityState.Modified; // for update isclear trans
                                    }
                                    _db.Entry(pre).State = EntityState.Deleted; // if not exist then delete
                                }
                                else
                                {
                                    model.TransactionClearanceDetails.Remove(exist); // remove from new data, beacause no need to change the data
                                }
                            }

                            await _db.AddRangeAsync(model.TransactionClearanceDetails);

                            _db.Entry(model).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        foreach (var item in model.TransactionClearanceDetails)
                        {
                            var trans = _paymentRepository.GetTransactionById(item.TransactionId);
                            if (trans == null)
                            {
                                return Json(new { IsSuccess = false, Message = "Transaction info not found!!" });
                            }

                            trans.IsClearTrans = true;
                            _db.Entry(trans).State = EntityState.Modified;
                        }
                        _db.Add(model);
                    }

                    await _db.SaveChangesAsync();
                    ModelState.Clear();
                    return Json(new { IsSuccess=true });
                }
            }
            catch (Exception ex)
            {

                throw ex;
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

            var trans =await _db.TransactionClearanceMaster.Include(a => a.Currency)
                .Include(a => a.TransactionClearanceDetails).FirstOrDefaultAsync(a=>a.MasterId==id);

            if (trans == null)
            {
                return NotFound();
            }

            ViewBag.PaymentClearance = await _db.TransactionClearanceDetails.Include(a=>a.PaymentTransaction) 
                .Where(a=>a.MasterId==id)
                .Select(a => new ClearanceViewModel
                {
                    MasterId=a.MasterId,
                    DetailsId=a.DetailsId,
                    TransactionId=a.TransactionId,
                    //Name="",
                    Amount=a.PaymentTransaction.Amount,
                    Order_Id=a.PaymentTransaction.Order_Id,
                    Email=a.PaymentTransaction.Email,
                    IsClearTrans=a.PaymentTransaction.IsClearTrans,
                }).ToListAsync();

            ViewBag.CurrId = new SelectList(_setupRepository.GetCurrencies(), "CurrId", "Name",trans.CurrId);
            ViewBag.UserIdFor = new SelectList(await _userService.GetUsersAsync(), "Id", "Name",trans.UserIdFor);
            return View("Create", trans);
        }

       

        [HttpGet]
        public async Task<IActionResult> GetPendingTransactionAsync(string userId,int currId, DateTime date)
        {
            var data =await _db.PaymentTransaction
                .Include(a=>a.PayType)
                .Where(a => a.UserIdFor == userId && a.CurrId==currId 
                && a.PaymentDate.Date <= date.Date && a.Status.ToLower()=="success" && !a.IsClearTrans )
                .Select(a => new ClearanceViewModel
                {
                    MasterId = 0,
                    DetailsId = 0,
                    TransactionId = a.TransactionId,
                    //Name="",
                    Amount = a.Amount,
                    Order_Id = a.Order_Id,
                    Email = a.Email,
                    IsClearTrans = a.IsClearTrans,
                })
                .ToListAsync();

            return Json(data);
        }


        //[HttpGet]
        public async Task<IActionResult> TransactionSuspendAsync(string userId, DateTime? FromDate, DateTime? ToDate)
        {
            ViewBag.UserId = new SelectList(await _userService.GetUsersAsync(), "Id", "Email");

            SqlParameter[] sqlParameter = new SqlParameter[3];           
            sqlParameter[0] = new SqlParameter("@useridFor", userId);
            sqlParameter[1] = new SqlParameter("@fromdate", FromDate);
            sqlParameter[2] = new SqlParameter("@todate", ToDate);           
            ViewBag.TransactionList = SQlHelper.ExecProcMapTList<PaymentTransactionDashboard>("SuccessfullPayment", sqlParameter);

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatusAsync(Guid transactionId,string status)
        {
            var trans = _paymentRepository.GetTransactionById(transactionId);

            if (trans==null)
            {
                return Json(new { isSuccess = false, message = "No transaction found!!" });
            }

            trans.Status = status; // for suspend transaction 

            _db.Entry(trans).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Json(new { isSuccess = true, message = "Success!!" });
        }

    }
}
