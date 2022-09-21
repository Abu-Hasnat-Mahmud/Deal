using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Models
{
    public class CommonFeild
    {
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsDelete { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }

    public partial class PayType : CommonFeild
    {
        [Key]
        public int PayTypeId { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Pay Type")]
        public string PayTypeName { get; set; }

        [StringLength(100)]
        public string Icon { get; set; }

        public bool IsCustom { get; set; }
        public bool IsActive { get; set; }

    }
    
    public partial class PayTypeViewModel
    {
        [Key]
        public int PayTypeId { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Pay Type")]
        public string PayTypeName { get; set; }

        [StringLength(100)]
        public string Icon { get; set; }

        public bool IsCustom { get; set; }
        public bool IsActive { get; set; }
        public bool IsUsing { get; set; }

    }

    public class UserPayTypeControl
    {
        [Key]
        public int Id { get; set; }

        public int PayTypeId { get; set; }
        [ForeignKey("PayTypeId")]
        public virtual PayType PayType { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsUsing { get; set; }
    }

    public class Currency
    {
        [Key]
        public int CurrId { get; set; }

        [StringLength(20)]
        [DisplayName("Currency")]
        [Required]
        public string Name { get; set; }
    }

    public class Client : CommonFeild
    {
        [Key]
        public int ClientId { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(30)]
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Description { get; set; }
    }

    public class Product : CommonFeild
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public double Price { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [DisplayName("Currency")]
        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }

    public class PaymentLink : CommonFeild
    {
        [Key]
        public int LinkId { get; set; }

        [StringLength(50, ErrorMessage = "Title max length 50 charecter")]
        [Required]
        public string Title { get; set; }

        [Required]
        public double Amount { get; set; }

        [DisplayName("Currency")]
        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        public bool IsInactive { get; set; }

        [StringLength(200)]
        [DisplayName("Link")]
        public string CustomLink { get; set; }

        [StringLength(128)]
        //[Required]
        public string Key { get; set; }

        [DisplayName("Custom Payment")]
        public bool IsCustomPayment { get; set; }
    }

    public class CustomPayment
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string BankName { get; set; }

        [Required]
        public double AmountPaid { get; set; }

        [DisplayName("Currency")]
        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        //public bool IsDelete { get; set; }       
        public bool IsAccept { get; set; }
        public bool IsReject { get; set; }

        public DateTime PaymentDate { get; set; }

        public string FilePath { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }

        public string UserIdFor { get; set; }
        [ForeignKey("UserIdFor")]
        public ApplicationUser ApplicationUser { get; set; }

        public Guid TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public PaymentTransaction PaymentTransaction { get; set; }
    }




    public class CheckoutMaster : CommonFeild
    {
        [Key]
        public int MasterId { get; set; }

        [StringLength(50, ErrorMessage = "Title max length 50 charecter")]
        [Required]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "Address max length 200 charecter")]
        public string Address { get; set; }

        [StringLength(128)]
        //[Required]
        public string Key { get; set; }

        [DisplayName("Enable Custom Payment")]
        public bool IsCustomPayment { get; set; }

        [DisplayName("Enable Callback")]
        public bool IsCallbackURL { get; set; }

        [DisplayName("Callback URL")]
        [DataType(DataType.Url)]
        public string CallbackUrl { get; set; }

        //[DisplayName("Client")]
        //public int ClientId { get; set; }
        //[ForeignKey("ClientId")]
        //public Client Client { get; set; }

        public virtual List<CheckoutDetails> CheckoutDetailses { get; set; }
    }





    public class CheckoutDetails
    {
        [Key]
        public int DetaileId { get; set; }

        [DisplayName("Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public double Price { get; set; }

        [DisplayName("Qty")]
        public int Quantity { get; set; }

        public int MasterId { get; set; }
        [ForeignKey("MasterId")]
        public CheckoutMaster CheckoutMaster { get; set; }

        public bool IsDelete { get; set; }
    }







    public class PaymentReceive
    {
        [Required]
        [DisplayName("Key")]
        public string Key { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
    }
    public class CheckoutPaymentReceive : PaymentReceive
    {
        [Required]
        public string Address { get; set; }
    }

    public class QRInfo : CommonFeild
    {
        [Key]
        public int QRId { get; set; }

        public bool IsActive { get; set; }

        public Guid Key { get; set; }

        [NotMapped]
        public byte[] QRCode { get; set; }
    }




    public class QRPaymentReceive : PaymentReceive
    {
        [Required]
        [DisplayName("Amount")]
        [Range(1, Double.MaxValue, ErrorMessage = "Minimum amount 1")]
        public double Amount { get; set; }

        [DisplayName("Currency")]
        [Required]
        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        public string Address { get; set; }
    }

    public class LinkPaymentReceive : PaymentReceive
    {
        public string Address { get; set; }
    }



    public class PaymentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionId { get; set; }

        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }

        public DateTime PaymentDate { get; set; }
        public string Order_Id { get; set; }
        public string Payment_Ref_Id { get; set; }

        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        public int? PayTypeId { get; set; }
        [ForeignKey("PayTypeId")]
        public PayType PayType { get; set; }

        public int? PaymentLinkId { get; set; }
        [ForeignKey("PaymentLinkId")]
        public PaymentLink PaymentLink { get; set; }

        public int? CheckoutId { get; set; }
        [ForeignKey("CheckoutId")]
        public CheckoutMaster CheckoutMaster { get; set; }

        public int? QRId { get; set; }
        [ForeignKey("QRId")]
        public QRInfo QRInfo { get; set; }

        public bool IsCustomPayment { get; set; }
        public bool IsDelete { get; set; }
        public bool IsClientUsed { get; set; }
        public string UserIdFor { get; set; }
        public bool IsClearTrans { get; set; } // for clearance transaction

    }

    public class PaymentTransactionDashboard
    {
        
        public string PayTypeName { get; set; }
        public string Currency { get; set; }
        public string TransactionId { get; set; }

        public string UserEmailFor { get; set; } // for user email
        public string Email { get; set; } // for client email
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }

        public DateTime PaymentDate { get; set; }
        public string Order_Id { get; set; }
        public string Payment_Ref_Id { get; set; }

        public int CurrId { get; set; }
        

        public int? PayTypeId { get; set; }
       

        public int? PaymentLinkId { get; set; }
      
        public int? CheckoutId { get; set; }
       

        public int? QRId { get; set; }
       

        public bool IsCustomPayment { get; set; }
        public bool IsDelete { get; set; }
        public bool IsClientUsed { get; set; }
        public bool IsClearTrans { get; set; }

    }


    public class TransactionClearanceMaster : CommonFeild
    {
        [Key]
        public int MasterId { get; set; }

        public int CurrId { get; set; }
        [ForeignKey("CurrId")]
        public Currency Currency { get; set; }

        public string Title { get; set; }

        
        public string UserIdFor { get; set; }
        [ForeignKey("UserIdFor")]
        public ApplicationUser ApplicationUserFor { get; set; }

        public DateTime PaymentDate { get; set; }
        public virtual List<TransactionClearanceDetails> TransactionClearanceDetails { get; set; }
    }

    public  class TransactionClearanceDetails 
    {
        [Key]
        public int DetailsId { get; set; }       

        //public string UserIdFor { get; set; }

        public DateTime IssueDate { get; set; }

        public Guid TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public virtual PaymentTransaction PaymentTransaction { get; set; } 

        public int MasterId { get; set; }
        [ForeignKey("MasterId")]
        public virtual TransactionClearanceMaster TransactionClearanceMaster { get; set; }
    }

    public class ClearanceViewModel
    {
        public int MasterId { get; set; }
        public int DetailsId { get; set; }
        public Guid TransactionId { get; set; }
        public double Amount { get; set; }
        public string Order_Id { get; set; }
        public string Email { get; set; }
        public bool IsClearTrans { get; set; }
    }



    //public class CustomerPayment
    //{
    //    [Key]
    //    public int CustomerPaymentId { get; set; }

    //    [Required(ErrorMessage = "ইনপুট নিশ্চিত করুন ।")]
    //    //[Display(Name = "Serial No")]
    //    [Display(Name = "সিরিয়াল নাম্বার :")]
    //    //[StringLength(20, ErrorMessage = "The {0} অবশ্যই কমপক্ষে {10} অক্ষর দীর্ঘ হতে হবে |", MinimumLength = 10)]
    //    //[RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "ইংরেজি তে লিখুন । বিশেষ অক্ষর অনুমোদিত নয় |")]

    //    public string CustomerPaymentNo { get; set; }
    //    public string TrxID { get; set; }
    //    public string PaymentId { get; set; }
    //    public string transactionStatus { get; set; }
    //    public float Amount { get; set; }
    //    public string Currency { get; set; }
    //    public string merchantInvoiceNumber { get; set; }
    //    public int SoftwarePackageId { get; set; }
    //    public int UsageDuration { get; set; }




    //    //[Display(Name = "Entry Date")]
    //    [Display(Name = "এন্ট্রি তারিখ :")]

    //    [DataType(DataType.DateTime)]
    //    public DateTime PaymentDate { get; set; }


    //    [Display(Name = "এন্ট্রি তারিখ :")]

    //    [DataType(DataType.DateTime)]
    //    public DateTime ActiveFromDate { get; set; }


    //    [Display(Name = "এন্ট্রি তারিখ :")]

    //    [DataType(DataType.DateTime)]
    //    public DateTime ActiveToDate { get; set; }


    //    [Required]
    //    [StringLength(128)]
    //    public string userid { get; set; }

    //    [Required]
    //    [Phone]
    //    public string UserPhoneNo { get; set; }


    //    //public int ?comid { get; set; }



    //    [StringLength(300)]
    //    [Display(Name = "স্ট্যাটাস")]
    //    public string Status { get; set; }

    //    public bool ActiveYesNo { get; set; }


    //}



}
