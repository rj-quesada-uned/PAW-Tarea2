using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarea2.Models
{
    public class LenguajePopularidad
    {
        public int Posicion { get; set; }
        public string NombreLenguaje { get; set; }
        public double Clasificacion { get; set; }
        public double DiferenciaPorcentual { get; set; }
    }
}