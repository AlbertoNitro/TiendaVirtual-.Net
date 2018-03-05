using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Practica.Models;
using PracticaAlberto;

namespace PracticaAlberto.Controllers
{
    public class CarrosCompraController : Controller
    {
        private TiendaAlbertoEntities db = new TiendaAlbertoEntities();

        public ActionResult NumProductosCarroCompra(CarroCompra carroCompra)
        {
            return PartialView(carroCompra);
        }

        // GET: CarroCompra
        public ActionResult Index(CarroCompra carroCompra)
        {

            ViewData["importeTotal"] = getImporteTotal(carroCompra) ;
            return View(carroCompra);
        }

        private double getImporteTotal(List<Producto> listaProductos) {
            double importeTotal = 0;
            foreach (Producto producto in listaProductos)
            {
                importeTotal += producto.Precio;
            }
            return importeTotal;
        }


        public ActionResult AddProduct(int id, CarroCompra carroCompra)
        {
            if (ModelState.IsValid)
            {
                Producto producto = db.Productoes.Find(id);
                carroCompra.Add(producto);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (User.Identity.IsAuthenticated)
            {
                string loginCliente = User.Identity.Name;
                if (loginCliente == "admin@admin.es")
                {
                    return RedirectToAction("Index", "Productos");
                } else
                {
                    return RedirectToAction("IndexUsers", "Productos");
                }
            } else {
                return RedirectToAction("IndexUsers", "Productos");
            }
                
        }

        // GET: CarroCompra/Details/5
        public ActionResult DetailsProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productoes.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }


        //  GET: CarroCompra/Delete/5
        public ActionResult DeleteProduct(int id, CarroCompra carroCompra)
        {
            if (ModelState.IsValid)
            {
                Producto producto = carroCompra.Find(x => x.Id == id);
                carroCompra.Remove(producto);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "CarrosCompra");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
