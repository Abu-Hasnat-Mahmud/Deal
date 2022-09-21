using Deal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deal.Repository
{
    public interface ISetupRepository
    {
        List<Currency> GetCurrencies();
        Client GetClientById(int clientId);
        List<Client> GetClients();
        Product GetProductById(int productId);
        List<Product> GetProducts();

        //Payment Types
        Task<PayType> GetPaymetTypeById(int id);
        Task<List<PayType>> GetAllPaymetTypes();
        Task<List<PayType>> GetActivePaymetTypes();
    }
}