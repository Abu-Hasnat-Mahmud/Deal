using Deal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Models
{
    public class GTRDBContext : IdentityDbContext<ApplicationUser>
    {
        public GTRDBContext(DbContextOptions<GTRDBContext> options) : base(options){ }
        
        public DbSet<PayType> PayType { get; set; }
        public DbSet<UserPayTypeControl> UserPayTypeControl { get; set; }
        public DbSet<Currency> Currency { get; set; }

        public DbSet<Client> Client { get; set; }
        public DbSet<Product> Product { get; set; }

        public DbSet<PaymentLink> S_PaymentLink { get; set; }
        public DbSet<CheckoutMaster> S_CheckoutMaster { get; set; }
        public DbSet<CheckoutDetails> S_CheckoutDetails { get; set; }
        public DbSet<QRInfo> S_QRInfo { get; set; }
        

        /// <summary>
        /// transaction history
        /// </summary>
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
        public DbSet<TransactionClearanceMaster> TransactionClearanceMaster { get; set; }
        public DbSet<TransactionClearanceDetails> TransactionClearanceDetails { get; set; }
        public DbSet<CustomPayment> CustomPayment { get; set; }


    }
}
