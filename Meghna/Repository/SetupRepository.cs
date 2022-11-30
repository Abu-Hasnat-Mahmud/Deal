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
    class SetupRepository : ISetupRepository
    {
        private DBContext _gTRDBContext;
        private readonly IUserService _userService;

        public SetupRepository(IUserService userService, DBContext gTRDBContext)
        {
            _userService = userService;
            _gTRDBContext = gTRDBContext;
        }

        // Get Currency list
        public List<Currency> GetCurrencies() => _gTRDBContext.Currency.ToList();

        // Get Client list
        public List<Client> GetClients() =>
            _gTRDBContext.Client.Where(a => a.IsDelete == false && a.UserId == _userService.GetUserId())
            .ToList();



        // Get client by client id
        public Client GetClientById(int clientId) =>
           _gTRDBContext.Client.Where(a => a.ClientId == clientId && a.UserId == _userService.GetUserId())
           .FirstOrDefault();


        // Get Product list
        public List<Product> GetProducts() =>
            _gTRDBContext.Product.Include(a => a.Currency).Where(a => a.IsDelete == false && a.UserId == _userService.GetUserId())
            .ToList();

        // Get Product by Product id
        public Product GetProductById(int productId) =>
            _gTRDBContext.Product.Include(a=>a.Currency).Where(a => a.ProductId == productId && a.UserId == _userService.GetUserId())
            .FirstOrDefault();


        // Get PayType list
        public async Task<List<PayType>> GetAllPaymetTypes() => await _gTRDBContext.PayType.ToListAsync();

         // Get active PayType list
        public async Task<List<PayType>> GetActivePaymetTypes() => 
            await _gTRDBContext.PayType.Where(a=>a.IsActive).ToListAsync();

         // Get active PayType list
        public async Task<PayType> GetPaymetTypeById(int id) => 
            await _gTRDBContext.PayType.FirstOrDefaultAsync(a=>a.PayTypeId==id);



    }
}
