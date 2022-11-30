using Deal.Helpers;
using Deal.Models;
using Deal.Repository;
using Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DBContext _db;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentRepository _paymentRepository;
        public DashboardController(DBContext db, IUserService userService, IConfiguration configuration, IPaymentRepository paymentRepository)
        {
            _db = db;
            _userService = userService;
            _configuration = configuration;
            _paymentRepository = paymentRepository;
        }


        //[HttpGet]
        public IActionResult Index(DateTime? FromDate, DateTime? ToDate)
        {
            //if (FromDate == null || ToDate==null)
            //{
            //    DateTime date = DateTime.Now;
            //    FromDate = new DateTime(date.Year, date.Month, 1);
            //    ToDate= FromDate?.AddMonths(1).AddDays(-1);
            //}

            SqlParameter[] sqlParameter = new SqlParameter[3];
            sqlParameter[0] = new SqlParameter("@userid", _userService.GetUserId());
            sqlParameter[1] = new SqlParameter("@fromdate", FromDate);
            sqlParameter[2] = new SqlParameter("@todate", ToDate);

            ViewBag.TransactionList = SQlHelper.ExecProcMapTList<PaymentTransactionDashboard>("TransactionSummary", sqlParameter);

            return View();
        }













    }
}
