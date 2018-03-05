using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Practica.Models;
using PracticaAlberto;

namespace PracticaAlberto.Controllers
{
    public class PedidosController : Controller
    {
        private TiendaAlbertoEntities db = new TiendaAlbertoEntities();

        [Authorize(Users = "admin@admin.es")]
        // GET: Pedidos
        public ActionResult Index()
        {  
            List<Pedido> listaPedidos = (from p in this.db.Pedidoes
                                         orderby p.Fecha descending
                                         select p).ToList();
            return View(listaPedidos);
        }

        private String generateUniqueToken()
        {
            int longitud = 32; //maximum: 32
            return Guid.NewGuid().ToString("N").Substring(0, longitud);
        }

        // GET: Pedidos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            } else
            {
                var listaIdsProductos = from p in this.db.INT_ProductoPedido
                                        where p.FK_INT_Pedido == id
                                        select p.Producto;
                ViewData["importeTotal"] = getImporteTotal(listaIdsProductos.ToList());
                return View(listaIdsProductos);
            }
        }

        private double getImporteTotal(List<Producto> listaProductos)
        {
            double importeTotal = 0;
            foreach (Producto producto in listaProductos)
            {
                importeTotal += producto.Precio;
            }
            return importeTotal;
        }

        // GET: Pedidos/Create
        public ActionResult Create(CarroCompra carroCompra)
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginCliente = User.Identity.Name;
                Cliente cliente = db.Clientes.First(c => c.Login == loginCliente);
                Pedido newPedido = new Pedido();
                newPedido.Id = generateUniqueToken();
                newPedido.Cliente = cliente;
                newPedido.ImporteTotal = (float) getImporteTotal(carroCompra);
                newPedido.Fecha = DateTime.Now;
                db.Pedidoes.Add(newPedido);
                db.SaveChanges();
                foreach (Producto producto in carroCompra)
                {
                    INT_ProductoPedido productoPedido = new INT_ProductoPedido();
                    productoPedido.FK_INT_Pedido = newPedido.Id;
                    productoPedido.FK_INT_Producto = producto.Id;
                    db.INT_ProductoPedido.Add(productoPedido);
                }
                db.SaveChanges();
                this.DecreaseStocks(carroCompra);
                carroCompra.RemoveAll(p => true);
                return RedirectToAction("List", "Pedidos");
            } else
            {
                return RedirectToAction("Login", "Account", routeValues: null);
            }
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FK_Id_Cliente")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Pedidoes.Add(pedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_Id_Cliente = new SelectList(db.Clientes, "Id", "Nombre", pedido.FK_Id_Cliente);
            return View(pedido);
        }

        [Authorize(Users = "admin@admin.es")]
        // GET: Pedidos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_Id_Cliente = new SelectList(db.Clientes, "Id", "Nombre", pedido.FK_Id_Cliente);
            return View(pedido);
        }

        [Authorize(Users = "admin@admin.es")]
        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FK_Id_Cliente")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_Id_Cliente = new SelectList(db.Clientes, "Id", "Nombre", pedido.FK_Id_Cliente);
            return View(pedido);
        }

        [Authorize(Users = "admin@admin.es")]
        // GET: Pedidos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidoes.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        [Authorize(Users = "admin@admin.es")]
        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                Pedido pedido = db.Pedidoes.Find(id);
                Cliente cliente = db.Clientes.Find(pedido.FK_Id_Cliente);
                cliente.Pedidoes.Remove(pedido);
                db.Pedidoes.Remove(pedido);
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("Error");
            }
            
        }

        private void DecreaseStocks(CarroCompra carroCompra)
        {
            if (ModelState.IsValid)
            {
                foreach (Producto producto in carroCompra)
                {
                    Producto productoBD = db.Productoes.Find(producto.Id);
                    productoBD.Cantidad--;
                    db.Entry(productoBD).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        // GET: Pedidos/Create
        public ActionResult List()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginCliente = User.Identity.Name;
                Cliente cliente = this.db.Clientes.First(c => c.Login == loginCliente);
                List<Pedido> listaPedidos = (from p in this.db.Pedidoes
                                             orderby p.Fecha descending
                                             where p.Cliente.Login == cliente.Login
                                   select p).ToList();
                return View(listaPedidos);
            }
            else
            {
                return RedirectToAction("Login", "Account", routeValues: null);
            }
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
