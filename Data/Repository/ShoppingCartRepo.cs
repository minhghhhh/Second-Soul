using Data.Base;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
	public class ShoppingCartRepo : GenericRepo<ShoppingCart>
	{
		public ShoppingCartRepo(SecondSoulShopContext context) : base(context)
		{
		}
	}
}
