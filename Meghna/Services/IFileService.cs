using Deal.Models;

namespace Deal.Services
{
    public interface IFileService
    {
        string ProductImageUpload(Product product);
        string CustomPaymentFile(CustomPayment payment);
    }
}