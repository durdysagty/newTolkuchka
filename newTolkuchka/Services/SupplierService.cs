using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SupplierService : ServiceNoFile<Supplier>, ISupplier
    {
        public SupplierService(AppDbContext con) : base(con)
        {
        }

        public IEnumerable<AdminSupplier> GetAdminSuppliers()
        {
            IEnumerable<AdminSupplier> suppliers = GetModels().Select(x => new AdminSupplier
            {
                Id = x.Id,
                Name = x.Name,
                PhoneMain = x.PhoneMain,
                PurchaseInvoices = x.PurchaseInvoices.Count
            });
            return suppliers;
        }
    }
}