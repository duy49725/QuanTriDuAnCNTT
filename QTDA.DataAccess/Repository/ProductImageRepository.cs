using QTDA.DataAccess.Data;
using QTDA.DataAccess.Repository.IRepository;
using QTDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTDA.DataAccess.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
	{
		private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
