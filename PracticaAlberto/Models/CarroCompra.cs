using PracticaAlberto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Practica.Models
{
    public class CarroCompra : List<Producto>
    {
        public int Id { get; set; }
    }
}