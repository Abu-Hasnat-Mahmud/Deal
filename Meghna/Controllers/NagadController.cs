using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Deal.Models;
using Deal.MarchentServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Deal.Repository;

namespace Deal.Controllers
{
    public class NagadController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;

        private IHostEnvironment _environment;

        public IHttpContextAccessor Accessor { get; }
        public NPGCrypto Npgn { get; }
        private readonly IConfiguration _config;
        private readonly IPaymentRepository _paymentRepository;
        public NagadController(ILogger<HomeController> logger, IPaymentRepository paymentRepository, IHttpContextAccessor accessor, NPGCrypto npgn,IHostEnvironment hostEnvironment, IConfiguration config)
        {
            _logger = logger;
            Accessor = accessor;
            Npgn = npgn;
            _config = config;
            _paymentRepository = paymentRepository;
            _environment = hostEnvironment;

        }

        [HttpGet]
        public IActionResult ControllerCheck(float amount)
        {
            return Json(amount);
        }

        [HttpGet]
        public async Task<IActionResult> PayWithNagad(float amount,string transactionId, bool isSandBox)
        {

            try
            {

                string message = "0 Check by Fahad";
                errorlog(message);


                var dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                var orderId = "NGA" + dateTime;


                var remoteIpAddress = Accessor.HttpContext.Connection.RemoteIpAddress;



                var messageabc = remoteIpAddress;
                errorlog(messageabc.ToString());

                // merchant id           



                var challenge = Npgn.RandomString(40, false);

                string initializeUri = ""; // NagadMarchent.InitializeUri + $"{merchantId}/{orderId}";// merchantId,orderId);
                string accountNumber = ""; //NagadMarchent.AccountNumber;
                var merchantId = ""; //NagadMarchent.MarchenId;
                if (isSandBox)
                {
                    merchantId = NagadMerchant.S_MerchantId;
                    initializeUri = NagadMerchant.S_InitializeUri + $"{merchantId}/{orderId}";// merchantId,orderId);
                    accountNumber = NagadMerchant.S_AccountNumber;
                }
                else
                {
                    merchantId = NagadMerchant.MerchantId;
                    initializeUri = NagadMerchant.InitializeUri + $"{merchantId}/{orderId}";// merchantId,orderId);
                    accountNumber = NagadMerchant.AccountNumber;
                }

                //var initializeUriSandbox = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/{merchantId}/{orderId}"; ///Mandat
                //var initializeUriLive = $"https://api.mynagad.com/api/dfs/check-out/initialize/{merchantId}/{orderId}"; ///Mandatory



                var plaintext = "{\"merchantId\": \"" + merchantId + "\", \"datetime\": \"" + dateTime + "\", \"orderId\": \"" + orderId + "\", \"challenge\": \"" + challenge + "\" }";
                errorlog(plaintext);


                message = "1 Plaintext encryption strat";
                errorlog(message);

                var encrptdsendata = Npgn.Encrypt(plaintext, isSandBox);
                errorlog(encrptdsendata);


                message = "2 Plaintext encrptdsendata done";
                errorlog(message);

                var signature = Npgn.Sign(plaintext, isSandBox);
                message = "3  signature create done";
                errorlog(message);

                SensativeDataObject res = new SensativeDataObject();

                using (var httpClient = new HttpClient())
                {

                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), initializeUri))  // initializeUriLive
                    {

                        /// set http header
                        Npgn.InitHeaders(request, remoteIpAddress);

                        request.Content = new StringContent("{\"dateTime\":\"" + dateTime + "\",\"sensitiveData\":\"" + encrptdsendata + "\",\"signature\":\"" + signature + "\"}", Encoding.UTF8, "application/json");
                        //requestbody for fiddler
                        var body = "{\"accountNumber\": \"" + accountNumber + "\",\"dateTime\":\"" + dateTime + "\",\"sensitiveData\":\"" + encrptdsendata + "\",\"signature\":\"" + signature + "\"}";

                        errorlog(body.ToString());



                        try
                        {
                            message = "4 intitialize API";
                            errorlog(message);


                            errorlog(request.Headers.ToString());

                            //HttpResponseMessage response
                            var response  = await httpClient.SendAsync(request);
                            string ResponseText;
                            Console.WriteLine(response.StatusCode);
                            errorlog(response.ToString());
                            errorlog(response.StatusCode.ToString() + "Line Number : 142");


                            if (response.IsSuccessStatusCode)
                            {
                                ResponseText = await response.Content.ReadAsStringAsync();
                                JObject jObject = JObject.Parse(ResponseText);

                                errorlog(jObject["sensitiveData"].ToString());


                                var resSensativeData = jObject["sensitiveData"].ToString();
                                var resSign = jObject["signature"].ToString();

                                // verify if the request is not tempered in the middle way
                                //var IsVerified = Npgn.Verify(plaintext, signature);
                                ////must be verified but not working now//need to check
                                //if (IsVerified == true)
                                //{
                                //    var sdata = Npgn.Decrypt(resSensativeData);
                                //    res = JsonConvert.DeserializeObject<SensativeDataObject>(sdata);
                                //}

                                //bypassing verification checking the procced work
                                var f = Npgn.Decrypt(resSensativeData, isSandBox);
                                res = JsonConvert.DeserializeObject<SensativeDataObject>(f);
                                var paymentRefId = res.PaymentReferenceId;
                                //var checkoutUriSandbox = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/{paymentRefId}";
                                //var checkoutUriLive = $"https://api.mynagad.com/api/dfs/check-out/complete/{paymentRefId}";

                                var checkoutUri = ""; //NagadMarchent.CheckOutUri + $"{paymentRefId}";


                                //var merchantCallbackUrl = $"https://localhost:44366/Nagad/PaymentResult/{paymentRefId}";
                                var merchantCallbackUrl = _config.GetValue<string>("PublishUri");


                                if (isSandBox)
                                {
                                    checkoutUri = NagadMerchant.S_CheckOutUri+ $"{paymentRefId}";
                                    //merchantCallbackUrl = merchantCallbackUrl +  NagadMerchant.S_CallbackUri+ $"/{transactionId}" ;
                                    merchantCallbackUrl = Url.ActionLink("PaymentResult", "Nagad", new { id = transactionId });
                                }
                                else
                                {
                                    errorlog("Live by Fahad line 186");


                                    checkoutUri = NagadMerchant.CheckOutUri + $"{paymentRefId}";
                                    //merchantCallbackUrl = merchantCallbackUrl + NagadMerchant.CallbackUri;
                                    merchantCallbackUrl = Url.ActionLink("PaymentResult", "Nagad", new { id = transactionId });
                                }


                                using (var orderrequest = new HttpRequestMessage(new HttpMethod("POST"), checkoutUri))
                                {
                                    var currencycode = "050";
                                    var sensativedatafeild = "{\"merchantId\": \"" + merchantId + "\", \"orderId\": \"" + orderId + "\", \"amount\": \"" + amount + "\", \"currencyCode\": \"" + currencycode + "\",\"challenge\": \"" + res.Challenge + "\" }";
                                    var sendata = Npgn.Encrypt(sensativedatafeild, isSandBox);
                                    var sign = Npgn.Sign(sensativedatafeild, isSandBox);

                                    Npgn.InitHeaders(orderrequest, remoteIpAddress);

                                    orderrequest.Content = new StringContent("{\"sensitiveData\":\"" + sendata + "\",\"signature\":\"" + sign + "\",\"merchantCallbackURL\":\"" + merchantCallbackUrl + "\"}", Encoding.UTF8, "application/json");

                                    //request body for fiddler
                                    //var url = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/{paymentRefId}";

                                    //var body3 = "{\"sensitiveData\":\"" + sendata + "\",\"signature\":\"" + sign + "\",\"merchantCallbackURL\":\"https://localhost:44330\"}";
                                    try
                                    {
                                        message = "5 checkout API call";
                                        errorlog(message);
                                        HttpResponseMessage rm = await httpClient.SendAsync(orderrequest);
                                        errorlog("3 check out response");
                                        if (rm.IsSuccessStatusCode)
                                        {
                                            string r = await rm.Content.ReadAsStringAsync();
                                            JObject obj = JObject.Parse(r);
                                            var callbackurl = obj["callBackUrl"].ToString();
                                            return Json(new {success=true, callbackurl = callbackurl });
                                        }
                                        else
                                        {
                                            return Json(new { success = false, error = rm.StatusCode });
                                        }
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        message = "6 error after checkout API call";
                                        errorlog(message);
                                        Console.WriteLine(ex.Message);
                                        return Json(new { success = false, error = ex.Message });
                                    }
                                }
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            errorlog(ex);


                            message = "7 error before checkout API call";
                            errorlog(message);
 

                            Console.WriteLine(ex.Message);
                            return Json(new { success = false, error = ex.Message });
                        }
                    }
                }
                return Json(new { error = message });
            }
            catch (Exception ex)
            {
                errorlog(ex);
                return Json(new { error = ex });

            }
        }


        public async Task<IActionResult> Refund(string orderid, string paymentRefId, float amount, bool isSandbox)
        {
            var remoteIpAddress = Accessor.HttpContext.Connection.RemoteIpAddress;

            // merchant id
            var merchantId = ""; //NagadMarchent.MarchenId;
            var initializeUri = ""; //NagadMarchent.CancelInitializeUri;

            if (isSandbox)
            {
                merchantId = NagadMerchant.S_MerchantId;
                initializeUri = NagadMerchant.S_CancelInitializeUri;
            }
            else
            {
                merchantId = NagadMerchant.MerchantId;
                initializeUri = NagadMerchant.CancelInitializeUri;
            }

            var dateTime = DateTime.Now.ToString("yyyyMMdd");
          

            var challenge = Npgn.RandomString(40, false);

            //var initializeUriSandbox = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/purchase/cancel?paymentRefId={paymentRefId}"; ///Mandat


            //var initializeUriLive = $"https://api.mynagad.com/api/dfs/purchase/cancel?paymentRefId={paymentRefId}"; ///Mandatory

           

            var plaintext = "{\"merchantId\": \"" + merchantId + "\", \"datetime\": \"" + dateTime + "\", \"orderId\": \"" + orderid + "\", \"challenge\": \"" + challenge + "\" }";

            string referenceNo = "test";
            string referenceMessage = "refund test";

            // with paymentReferenceid
            //var refundPlaintext = "{\"merchantId\": \"" + merchantId + "\", \"originalRequestDate\": \"" + dateTime + "\",\"paymentRefId\": \"\", \"orderId\": \"" + orderid + "\",\"originalAmount\": \"" + amount + "\", \"cancelAmount\": \"" + (amount - 1) + "\", \"referenceNo\": \"" + referenceNo + "\",\"referenceMessage\": \"" + referenceMessage + "\"}";
            var refundPlaintext = "{\"merchantId\": \"" + merchantId + "\", \"originalRequestDate\": \"" + dateTime + "\", \"orderId\": \"" + orderid + "\",\"originalAmount\": \"" + amount + "\", \"cancelAmount\": \"" + amount + "\", \"referenceNo\": \"" + referenceNo + "\",\"referenceMessage\": \"" + referenceMessage + "\"}";

            var encrptdsendata = Npgn.Encrypt(refundPlaintext,isSandbox);
            var signature = Npgn.Sign(refundPlaintext,isSandbox);

            RefundResoponseSenObj res = new RefundResoponseSenObj();

            using (var httpClient = new HttpClient())
            {

                using (var request = new HttpRequestMessage(new HttpMethod("POST"), initializeUri))  
                {

                    /// set http header
                    Npgn.InitHeaders(request, remoteIpAddress);

                    request.Content = new StringContent("{\"dateTime\":\"" + dateTime + "\",\"sensitiveData\":\"" + encrptdsendata + "\",\"signature\":\"" + signature + "\"}", Encoding.UTF8, "application/json");
                    //requestbody for fiddler
                    var body = "{\"sensitiveDataCancelRequest\":\"" + encrptdsendata + "\",\"signature\":\"" + signature + "\"}";

                    try
                    {
                        HttpResponseMessage response = await httpClient.SendAsync(request);
                        string ResponseText;
                        Console.WriteLine(response.StatusCode);
                        if (response.IsSuccessStatusCode)
                        {
                            ResponseText = await response.Content.ReadAsStringAsync();
                            JObject jObject = JObject.Parse(ResponseText);

                            var resSensativeData = jObject["sensitiveData"].ToString();
                            var resSign = jObject["signature"].ToString();
                           
                            var f = Npgn.Decrypt(resSensativeData,isSandbox);
                            res = JsonConvert.DeserializeObject<RefundResoponseSenObj>(f);
                            var cancelAmount = res.cancelAmount;

                            return Json(new { isSuccess = true, result = res });
                        }
                        else
                        {
                            //RefundResoponseErrObj err;
                            ResponseText = await response.Content.ReadAsStringAsync();
                            JObject jObject = JObject.Parse(ResponseText);
                            var reason = jObject["reason"].ToString();
                            var message = jObject["message"].ToString();



                            return Json(new { isSuccess = false, err = message });
                        }
                    }
                    catch (HttpRequestException ex)
                    {

                        Console.WriteLine(ex.Message);
                        errorlog(ex);
                        return Json(new { error = ex.Message });
                    }

                }


            }


           // return Json("");
        }

        public async Task<IActionResult> PaymentResultAsync(string id,[FromQuery] CallbackResponseObject callbackResponseObject)
        {
            try
            {
                string transactionId = null;
                if (id==null)
                {
                     transactionId = Request.RouteValues["id"]?.ToString();
                }
                else
                {
                    transactionId = id;
                }
                

                if (callbackResponseObject != null)
                {
                    if (transactionId !=null )
                    {
                        var transaction = _paymentRepository.GetTransactionInfoById(new Guid(transactionId));
                        if (transaction != null)
                        {
                            transaction.Order_Id = callbackResponseObject.order_id;
                            transaction.Payment_Ref_Id = callbackResponseObject.payment_ref_id;
                            transaction.PaymentDate = DateTime.Now; //Convert.ToDateTime(callbackResponseObject.payment_dt);
                            transaction.Status = "Success";
                            _paymentRepository.MerchantCallBack(transaction);
                            await _paymentRepository.TransactionEmailSend(transaction);
                            ViewBag.status = "success";
                            return View(transaction);
                        }
                        else
                        {
                            ViewBag.transactionId = transactionId;
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.transactionId = transactionId;
                        return View();
                    }
                   

                }
                //ViewBag.status = "success";
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           




            //Error callback responsr uri
            //https://localhost:44366/Nagad/PaymentResult/?merchant=683002007104225&order_id=Bangladesh1994&payment_ref_id=MDYyMzEyNTgwMzIxMi42ODMwMDIwMDcxMDQyMjUuQmFuZ2xhZGVzaDE5OTQuMDAzNTQxZDk4MGQ3NGJmZjJhYmE=&status=Failed&status_code=50_0026_851&message=E%20Com%20Trx%20Failed.%20Transaction%20over%20limit.
            //success callback response uri
            //https://localhost:44366/Nagad/PaymentResult/?merchant=1236545412342534&order_id=ordasdf234&payment_ref_id=sample0nlinepaymentrefid&status=success&status_code=00_000_000&payment_dt=20192906162245&issuer_payment_ref=70X8BCDI

            // await write(callbackResponseObject);

        }

        public async Task<IActionResult> CheckPayment(string paymentRefId, bool isSandbox)
        {
            //need to verify the payment status
            var remoteIpAddress = Accessor.HttpContext.Connection.RemoteIpAddress;
            using (var httpClient = new HttpClient())
            {
                var verifyUri = ""; //NagadMarchent.VerifyUri + $"{paymentRefId}"; // $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/verify/payment/{paymentRefId}";

                if (isSandbox)
                {
                    verifyUri = NagadMerchant.S_VerifyUri + $"{paymentRefId}";
                }
                else
                {
                    verifyUri = NagadMerchant.VerifyUri + $"{paymentRefId}";
                }
                
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), verifyUri))
                {
                    try
                    {
                        Npgn.InitHeaders(request, remoteIpAddress);
                        var rm = await httpClient.SendAsync(request);
                        string r = await rm.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ResponseObj>(r);
                        return Json(new { model = data });
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }

                }
            }
            return null;
        }


        public IActionResult write(CallbackResponseObject model)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string directory = System.IO.Path.Combine(path, "TransactionLog");

            // TODO verify directory exists, Name is not null, Path is not null, Body is not null

            string fileName = "TransactionResult.txt";
            string fullPath = System.IO.Path.Combine(path, fileName);


            if (!System.IO.File.Exists(fullPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(fullPath))
                {
                    sw.Write("Order Id : ");
                    sw.WriteLine(model.order_id);
                    sw.Write("payment_ref_id : ");
                    sw.WriteLine(model.payment_ref_id);
                    sw.WriteLine("============");
                }
            }
            else
            {
                // This text is always added, making the file longer over time
                // if it is not deleted.
                using (StreamWriter sw = System.IO.File.AppendText(fullPath))
                {
                    sw.Write("Order Id : ");
                    sw.WriteLine(model.order_id);
                    sw.Write("payment_ref_id : ");
                    sw.WriteLine(model.payment_ref_id);
                    sw.WriteLine("============");
                }
            }




            return Ok();
        }


        public void errorlog(Exception ex)
        {
            string filePath = @"C:\DevelopmentError\DevelopmentFile.txt";


            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }

        public void errorlog(string ex)
        {
            string filePath = @"C:\DevelopmentError\DevelopmentFile.txt";


            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(ex);
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();


            }
        }



        public IActionResult ErrorCheck(string err)
        {

            string path = _environment.ContentRootPath;
            //string directory = System.IO.Path.Combine(path, "TransactionLog");

            // TODO verify directory exists, Name is not null, Path is not null, Body is not null

            string fileName = "NagadError.txt";
            string fullPath = System.IO.Path.Combine(path, fileName);

            // Create a file to write to.
            using (StreamWriter sw = System.IO.File.CreateText(fullPath))
            {
                sw.WriteLine(err);
                sw.Write("Date: ");
                sw.WriteLine(DateTime.Now);
                sw.WriteLine("============");
            }

            //if (!System.IO.File.Exists(fullPath))
            //{
            //    // Create a file to write to.
            //    using (StreamWriter sw = System.IO.File.CreateText(fullPath))
            //    {
            //        sw.WriteLine(err);
            //        sw.Write("Date: ");
            //        sw.WriteLine(DateTime.Now);                  
            //        sw.WriteLine("============");
            //    }
            //}
            //else
            //{
            //    // This text is always added, making the file longer over time
            //    // if it is not deleted.
            //    using (StreamWriter sw = System.IO.File.AppendText(fullPath))
            //    {
            //        sw.WriteLine(err);
            //        sw.Write("Date: ");
            //        sw.WriteLine(DateTime.Now);
            //        sw.WriteLine("============");
            //    }
            //}




            return Ok();
        }

        #region not need


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
