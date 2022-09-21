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
    
    public class MeghnaAPIController : Controller
    {
        private readonly GTRDBContext _db;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentRepository _paymentRepository;
        public MeghnaAPIController(GTRDBContext db, IUserService userService, IConfiguration configuration, IPaymentRepository paymentRepository)
        {
            _db = db;
            _userService = userService;
            _configuration = configuration;
            _paymentRepository = paymentRepository;
        }


        [HttpGet]
        public IActionResult TransacationCheck(string id)
        {
            //if(String.IsNullOrEmpty(id))
            //{
            //    return new JsonResult(new { message = "No content" }) { StatusCode = StatusCodes.Status204NoContent };
            //}

            var trans = _paymentRepository.GetTransactionInfoById(Guid.Parse(id));

            if (trans == null)
            {
                return new JsonResult(new { message = "No content" }) { StatusCode = StatusCodes.Status204NoContent };
            }
            else
            {
                var returnObject = new JsonResult(new
                                {
                                    Currency = trans.Currency.Name,
                                    trans.Amount,
                                    trans.PaymentDate,
                                    trans.Email,
                                    Title = trans.CheckoutMaster != null ? trans.CheckoutMaster.Title : trans.PaymentLink.Title,
                                    trans.Status,
                                    trans.IsClientUsed,
                                }){ StatusCode = StatusCodes.Status200OK };

                if (!trans.IsClientUsed)
                {
                    trans.IsClientUsed = true;
                    _db.Update(trans);
                    _db.SaveChanges();
                }

                return returnObject;
            }

           
        }

 











    }
}
