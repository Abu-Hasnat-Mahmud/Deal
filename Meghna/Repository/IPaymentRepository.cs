using Deal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deal.Repository
{
    public interface IPaymentRepository
    {
        // payment link part
        PaymentLink GetPaymentLinkById(int id);
        List<PaymentLink> GetPaymentLinks();
        List<PaymentLink> GetPaymentLinksArchive();
        PaymentLink GetPaymentLinkByKey(string key);
        PaymentTransaction CreateLinkTransaction(LinkPaymentReceive linkPaymentReceive, PaymentLink model, string status);


        //checkout part
        List<CheckoutMaster> GetCheckoutList();
        List<CheckoutMaster> GetCheckoutListArchive();
        CheckoutMaster GetCheckoutById(int id);
        CheckoutMaster GetCheckoutByKey(string key);
        PaymentTransaction CreateCheckoutTransaction(CheckoutPaymentReceive chekoutPaymentReceive, CheckoutMaster model, string status);


        // QR part
        Task<QRInfo> CreateQRInfo();
        QRInfo GetQRInfoByKey(Guid key);

        PaymentTransaction CreateQRTransaction(QRPaymentReceive qrPaymentReceive, QRInfo model, string status);


        //Transaction part
        PaymentTransaction GetTransactionById(Guid transactionId);
        PaymentTransaction GetTransactionInfoById(Guid transactionId);
        PaymentTransaction MerchantCallBack(PaymentTransaction model);

        Task TransactionEmailSend(PaymentTransaction transaction);

        // Custom payment
        Task<List<CustomPayment>> GetPendingCustomPayments();
        Task<CustomPayment> GetCustomPaymentById(int id);
    }
}