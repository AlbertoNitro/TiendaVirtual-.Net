using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practica.Models.Binders
{
    public class CarroCompraModelBinder : System.Web.Mvc.IModelBinder
    {
        private const string ID_SESSION_CARRO = "ID_CARRITO";

        public Object BindModel(ControllerContext mbctx, System.Web.Mvc.ModelBindingContext bctx)
        {
            CarroCompra carroCompra = (CarroCompra)mbctx.HttpContext.Session[ID_SESSION_CARRO];
            if (carroCompra == null)
            {
                carroCompra = new CarroCompra();
                mbctx.HttpContext.Session[ID_SESSION_CARRO] = carroCompra;
            }
            return carroCompra;
        }
    }
}