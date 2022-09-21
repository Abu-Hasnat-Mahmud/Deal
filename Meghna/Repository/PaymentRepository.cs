using Deal.Models;
using Deal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Repository
{
    class PaymentRepository : IPaymentRepository
    {
        private GTRDBContext _gTRDBContext;
       
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PaymentRepository(IUserService userService, IEmailService emailService, IConfiguration configuration, GTRDBContext gTRDBContext)
        {
            _userService = userService;
            _gTRDBContext = gTRDBContext;
            _emailService = emailService;
            _configuration = configuration;
        }


        // Get PaymentLink list
        public List<PaymentLink> GetPaymentLinks() =>
            _gTRDBContext.S_PaymentLink.Include(a => a.Currency).Where(a => a.IsDelete == false && a.UserId == _userService.GetUserId())
            .ToList();

        // Get PaymentLink list from archive
        public List<PaymentLink> GetPaymentLinksArchive() =>
            _gTRDBContext.S_PaymentLink.Include(a => a.Currency).Where(a => a.IsDelete == true && a.UserId == _userService.GetUserId())
            .ToList();


        // Get PaymentLink by id
        public PaymentLink GetPaymentLinkById(int id) =>
            _gTRDBContext.S_PaymentLink.Where(a => a.LinkId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

        // Get PaymentLink by key
        public PaymentLink GetPaymentLinkByKey(string key) =>
            _gTRDBContext.S_PaymentLink.Include(a=>a.Currency).Where(a => a.Key == key && a.IsInactive==false).FirstOrDefault();


        public PaymentTransaction CreateLinkTransaction(LinkPaymentReceive linkPaymentReceive, PaymentLink model, string status)
        {
            PaymentTransaction paymentTransaction = new PaymentTransaction
            {
                PaymentLinkId = model.LinkId,
                Amount = model.Amount,
                Status = status,
                CurrId = model.CurrId,
                IsCustomPayment = model.IsCustomPayment,
                Address = linkPaymentReceive.Address,
                Email = linkPaymentReceive.UserEmail,
                PaymentDate = DateTime.Now,
                UserIdFor = model.UserId,
            };

            return CreateNewTransaction(paymentTransaction);
        }


        // Get Checkout list
        public List<CheckoutMaster> GetCheckoutList() =>
            _gTRDBContext.S_CheckoutMaster
            .Include(a => a.CheckoutDetailses.Where(b => b.IsDelete == false))
            .ThenInclude(a => a.Product)
            .ThenInclude(a => a.Currency)
            .Where(a => a.IsDelete == false && a.UserId == _userService.GetUserId())
            .ToList();

        // Get Checkout list from archive
        public List<CheckoutMaster> GetCheckoutListArchive() =>
            _gTRDBContext.S_CheckoutMaster
            .Include(a => a.CheckoutDetailses.Where(b => b.IsDelete == false))
            .ThenInclude(a => a.Product)
            .ThenInclude(a => a.Currency)
            .Where(a => a.IsDelete == true && a.UserId == _userService.GetUserId())
            .ToList();

        // Get Checkout by id
        public CheckoutMaster GetCheckoutById(int id) =>
            _gTRDBContext.S_CheckoutMaster
            .Include(a => a.CheckoutDetailses.Where(b => b.IsDelete == false))
            .ThenInclude(a => a.Product)
            .ThenInclude(a => a.Currency)
            .Where(a => a.MasterId == id && a.UserId == _userService.GetUserId()).FirstOrDefault();

        // Get Checkout by key
        public CheckoutMaster GetCheckoutByKey(string key) =>
            _gTRDBContext.S_CheckoutMaster
            .Include(a => a.CheckoutDetailses.Where(b => b.IsDelete == false))
            .ThenInclude(a => a.Product)
            .ThenInclude(a => a.Currency)
            .Where(a => a.Key == key).FirstOrDefault();


        // Create Checkout transaction
        public PaymentTransaction CreateCheckoutTransaction(CheckoutPaymentReceive checkoutPaymentReceive, CheckoutMaster model, string status)
        {
            PaymentTransaction paymentTransaction = new PaymentTransaction
            {
                CheckoutId = model.MasterId,
                Amount = model.CheckoutDetailses.Where(a => a.IsDelete == false).Sum(a => a.Price),
                Status = status,
                IsCustomPayment = model.IsCustomPayment,
                CurrId = model.CheckoutDetailses.Where(a => a.IsDelete == false).FirstOrDefault().Product.CurrId,
                Address = checkoutPaymentReceive.Address,
                Email = checkoutPaymentReceive.UserEmail,
                PaymentDate = DateTime.Now,
                UserIdFor=model.UserId,
            };

            return CreateNewTransaction(paymentTransaction);
        }

        // Get transaction by id
        public PaymentTransaction GetTransactionById(Guid transactionId)
        {
            return _gTRDBContext.PaymentTransaction.FirstOrDefault(a => a.TransactionId == transactionId);
        }

        // Get transaction by id
        public PaymentTransaction GetTransactionInfoById(Guid transactionId)
        {
            return _gTRDBContext.PaymentTransaction
                .Include(a=>a.Currency)
                .Include(a=>a.CheckoutMaster)
                .Include(a=>a.PaymentLink)
                .FirstOrDefault(a => a.TransactionId == transactionId);
        }

        // Get Checkout by key
        PaymentTransaction CreateNewTransaction(PaymentTransaction model)
        {
            model.TransactionId = Guid.NewGuid();

            _gTRDBContext.Add(model);
            _gTRDBContext.SaveChanges();
            return model;
        }



        // Set callback transaction
        public PaymentTransaction MerchantCallBack(PaymentTransaction model)
        {
            _gTRDBContext.Update(model);
            _gTRDBContext.SaveChanges();
            return model;
        }


        public async Task TransactionEmailSend(PaymentTransaction transaction)
        {          
            if (transaction.CheckoutId != null)
            {
                var trans = _gTRDBContext.PaymentTransaction
                    .Include(a => a.CheckoutMaster)
                        .ThenInclude(a => a.CheckoutDetailses).ThenInclude(a => a.Product)
                    .Include(a => a.Currency)
                    .FirstOrDefault(a => a.TransactionId == transaction.TransactionId);

                if (trans != null)
                {
                    string productName = trans.CheckoutMaster.CheckoutDetailses.FirstOrDefault().Product.Name;
                    string currency = trans.Currency.Name;
                    double amount = trans.Amount;
                   

                    string prodcutInfo = "Product: " + productName + "    " + currency + " - " + amount;

                    UserEmailOptions clientEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { trans.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                           // new KeyValuePair<string, string>("{{username}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailCheckoutClient(clientEmailOptions);


                    ApplicationUser user =await _userService.GetUserByIdAsync(transaction.CheckoutMaster.UserId);

                    UserEmailOptions userEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { user.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{username}}",user.Name),
                            new KeyValuePair<string, string>("{{clientname}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailCheckoutSeller(userEmailOptions);


                }

            }
            else if (transaction.PaymentLinkId != null)
            {
                var trans = _gTRDBContext.PaymentTransaction
                    .Include(a => a.PaymentLink)                        
                    .Include(a => a.Currency)
                    .FirstOrDefault(a => a.TransactionId == transaction.TransactionId);

                if (trans != null)
                {
                    string productName = trans.PaymentLink.Title;
                    string currency = trans.Currency.Name;
                    double amount = trans.Amount;

                    string prodcutInfo = "Link: " + productName + "    " + currency + " - " + amount;

                    UserEmailOptions clientEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { trans.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                           // new KeyValuePair<string, string>("{{username}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailLinkClient(clientEmailOptions);


                    ApplicationUser user = await _userService.GetUserByIdAsync(transaction.PaymentLink.UserId);

                    UserEmailOptions userEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { user.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{username}}",user.Name),
                            new KeyValuePair<string, string>("{{clientname}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailLinkAuthor(userEmailOptions);
                }
            }
            else if (transaction.QRId != null)
            {
                var trans = _gTRDBContext.PaymentTransaction
                    .Include(a => a.QRInfo)                        
                    .Include(a => a.Currency)
                    .FirstOrDefault(a => a.TransactionId == transaction.TransactionId);

                if (trans != null)
                {
                    ApplicationUser user = await _userService.GetUserByIdAsync(transaction.QRInfo.UserId);

                    string productName = user.Name; //trans.QRInfo.Title;
                    string currency = trans.Currency.Name;
                    double amount = trans.Amount;

                    string prodcutInfo = "User: " + productName + "    " + currency + " - " + amount;

                    UserEmailOptions clientEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { trans.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                           // new KeyValuePair<string, string>("{{username}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailQRClient(clientEmailOptions);


                    
                    UserEmailOptions userEmailOptions = new UserEmailOptions
                    {
                        ToEmails = new List<string>() { user.Email },
                        PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{username}}",user.Name),
                            new KeyValuePair<string, string>("{{clientname}}",trans.Email),
                            new KeyValuePair<string, string>("{{invoiceno}}",trans.Order_Id),
                            new KeyValuePair<string, string>("{{productinfo}}",prodcutInfo)
                        }
                    };

                    await _emailService.SendEmailQRAuthor(userEmailOptions);
                }
            }

        }

        public async Task<QRInfo> CreateQRInfo()
        {
            try
            {
                QRInfo qRInfo = new QRInfo
                {
                    IsActive = true,
                    Key = Guid.NewGuid(),
                    UserId = _userService.GetUserId(),
                    DateAdded = DateTime.Now
                };

                _gTRDBContext.Add(qRInfo);
                await _gTRDBContext.SaveChangesAsync();
                return qRInfo;
            }
            catch (Exception)
            {
                 throw;
            }
           

        }

        public QRInfo GetQRInfoByKey(Guid key)
        {
            return _gTRDBContext.S_QRInfo.FirstOrDefault(a=>a.Key == key);
        }

        // Create Checkout transaction
        public PaymentTransaction CreateQRTransaction(QRPaymentReceive qrPaymentReceive, QRInfo model, string status)
        {
            PaymentTransaction paymentTransaction = new PaymentTransaction
            {
                QRId = model.QRId,
                Amount = qrPaymentReceive.Amount,
                Status = status,
                CurrId = qrPaymentReceive.CurrId,
                Address = qrPaymentReceive.Address,
                Email = qrPaymentReceive.UserEmail,
                PaymentDate = DateTime.Now,
                UserIdFor = model.UserId,
            };

            return CreateNewTransaction(paymentTransaction);
        }


        // get pending custom payment
        public async Task<List<CustomPayment>> GetPendingCustomPayments()
        {
            var payments = await _gTRDBContext.CustomPayment
                .Include(a=>a.Currency)
                .Include(a=>a.PaymentTransaction).ThenInclude(a=>a.PaymentLink)
                .Include(a=>a.PaymentTransaction).ThenInclude(a=>a.CheckoutMaster)
                .Where(a => a.UserIdFor == _userService.GetUserId() && !a.IsReject && !a.IsAccept).ToListAsync();
            return payments;
        }

        public async Task<CustomPayment> GetCustomPaymentById(int id)
        {
            var payment = await _gTRDBContext.CustomPayment.Where(a => a.UserIdFor == _userService.GetUserId() && a.Id==id).FirstOrDefaultAsync();
            return payment;
        }



    }
}
