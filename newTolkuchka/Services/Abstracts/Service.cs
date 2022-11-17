using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    //public enum LanVersion { Ru, En, Tm }
    public abstract class Service<T> : IAction<T> where T : class
    {
        private protected readonly AppDbContext _con;
        public Service(AppDbContext con)
        {
            _con = con;
        }

        public async Task<T> GetModelAsync(int id)
        {
            T model = (T)await _con.FindAsync(typeof(T), id);
            return model;
        }

        public IQueryable<T> GetModels()
        {
            IQueryable<T> models = _con.Set<T>();
            return models;
        }

        public virtual bool IsExist(T model, IEnumerable<T> list)
        {
            bool exist = false;
            if (!list.Any())
                return exist;
            Type type = typeof(T);
            IEnumerable<PropertyInfo> nameProperties = type.GetProperties().Where(x => x.Name.Contains("Name"));
            foreach (PropertyInfo p in nameProperties)
            {
                string value = p.GetValue(model).ToString();
                exist = list.Where(x => p.GetValue(x).ToString() == value).Any();
                if (exist)
                    return exist;
            }
            return exist;
        }

        public async Task<bool> IsBinded(int id)
        {
            string typeName = typeof(T).Name;
            switch (typeName)
            {
                case "Brand":                    
                    if (await _con.Products.Where(p => p.BrandId == id).AnyAsync())
                        return true;
                    if (await _con.Lines.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    break;
                case "Category":
                    if (await _con.Categories.Where(p => p.ParentId == id).AnyAsync())
                        return true;
                    if (await _con.Products.Where(p => p.CategoryId == id).AnyAsync())
                        return true;
                    break;
                case "Line":
                    if (await _con.Products.Where(p => p.LineId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(p => p.LineId == id).AnyAsync())
                        return true;
                    break;
                case "Model":
                    return await _con.Products.Where(p => p.ModelId == id).AnyAsync();
                case "Position":
                    return await _con.Employees.Where(p => p.PositionId == id).AnyAsync();
                case "Product":                    
                    if (await _con.Purchases.Where(p => p.ProductId == id).AnyAsync())
                        return true;
                    if (await _con.Orders.Where(p => p.ProductId == id).AnyAsync())
                        return true;
                    break;
                case "Spec":
                    return await _con.SpecsValues.Where(x => x.SpecId == id).AnyAsync();
                case "SpecsValue":
                    return await _con.ProductSpecsValues.Where(x => x.SpecsValueId == id).AnyAsync();
                case "SpecsValueMod":
                    return await _con.ProductSpecsValueMods.Where(x => x.SpecsValueModId == id).AnyAsync();
                case "Type":
                    return await _con.Products.Where(x => x.TypeId == id).AnyAsync();
                case "Warranty":
                    return await _con.Products.Where(x => x.WarrantyId == id).AnyAsync();
                case "Supplier":
                    return await _con.PurchaseInvoices.Where(x => x.SupplierId == id).AnyAsync();
                case "PurchaseInvoice":
                    return await _con.Purchases.Where(x => x.PurchaseInvoiceId == id).AnyAsync();
                case "Purchase":
                    return await _con.Orders.Where(x => x.PurchaseId == id).AnyAsync();
                case "Invoice":
                    return await _con.Orders.Where(x => x.InvoiceId == id).AnyAsync();
                case "Currency":
                    // to be corrected
                    return true;
                default:
                    return false;
            }
            return false;
        }
        public Type GetModelType()
        {
            Type type = typeof(T);
            return type;
        }
        public int GetModelId(Type type, T model)
        {
            return (int)type.GetProperty("Id").GetValue(model);
        }
    }
}
