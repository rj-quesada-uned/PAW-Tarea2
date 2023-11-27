using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarea2.Models;

namespace Tarea2.Controllers
{
    public class HomeController : Controller
    {
        private const string EncuestasKey = "Encuestas";
        private const string LenguajesKey = "Lenguajes";
        // GET: Home
        public ActionResult Index()
        {
            List<Encuesta> encuestas = ObtenerEncuestas();
            List<LenguajePopularidad> popularidadLenguajes = ObtenerLenguajes();

            var modeloCompuesto = new Tuple<List<Encuesta>, List<LenguajePopularidad>>(encuestas, popularidadLenguajes);

            return View(modeloCompuesto);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(Encuesta encuesta)
        {
            List<Encuesta> encuestas = ObtenerEncuestas();
            List<LenguajePopularidad> popularidadLenguajes = ObtenerPopularidadLenguajes(encuestas);

            if (encuestas == null)
            {
                encuestas = new List<Encuesta>();
            }

            if (popularidadLenguajes == null)
            {
                popularidadLenguajes = new List<LenguajePopularidad>();
            }

            if (encuesta.LenguajePrimario == encuesta.LenguajeSecundario)
            {
                ModelState.AddModelError("LenguajeSecundario", "El lenguaje secundario no puede ser igual al lenguaje primario.");
                return View(encuesta);
            }

            string lenguajePrimario = encuesta.LenguajePrimario;
            string lenguajeSecundario = encuesta.LenguajeSecundario;

            IncrementarPopularidad(popularidadLenguajes, encuesta.LenguajePrimario, 1);
            IncrementarPopularidad(popularidadLenguajes, encuesta.LenguajeSecundario, 0.5);

            List<LenguajePopularidad> copiaPopularidadLenguajes = new List<LenguajePopularidad>(popularidadLenguajes);
            Session[LenguajesKey] = copiaPopularidadLenguajes;

            encuestas.Add(encuesta);
            Session[EncuestasKey] = encuestas;

            return RedirectToAction("Index");
        }

        private List<Encuesta> ObtenerEncuestas()
        {
            List<Encuesta> encuestas = Session[EncuestasKey] as List<Encuesta>;

            if (encuestas == null)
            {
                encuestas = new List<Encuesta>();
            }

            return encuestas;
        }

        private List<LenguajePopularidad> ObtenerLenguajes()
        {
            List<LenguajePopularidad> lenguajes = Session[LenguajesKey] as List<LenguajePopularidad>;

            if (lenguajes == null)
            {
                lenguajes = new List<LenguajePopularidad>();
            }

            return lenguajes;
        }


        private List<LenguajePopularidad> ObtenerPopularidadLenguajes(List<Encuesta> encuestas)
        {
            encuestas = encuestas ?? new List<Encuesta>();
            List<LenguajePopularidad> popularidadLenguajes = Session[LenguajesKey] as List<LenguajePopularidad>;

            foreach (var encuesta in encuestas)
            {
                IncrementarPopularidad(popularidadLenguajes, encuesta.LenguajePrimario, 1);
                IncrementarPopularidad(popularidadLenguajes, encuesta.LenguajeSecundario, 0.5);
            }

            if (popularidadLenguajes != null && popularidadLenguajes.Any())
            {
                popularidadLenguajes = popularidadLenguajes.OrderByDescending(x => x.Clasificacion).ToList();
            }


            return popularidadLenguajes;
        }

        private void IncrementarPopularidad(List<LenguajePopularidad> popularidadLenguajes, string nombreLenguaje, double incremento)
        {
            var lenguajeExistente = popularidadLenguajes.FirstOrDefault(x => x.NombreLenguaje == nombreLenguaje);
            if (lenguajeExistente != null)
            {
                lenguajeExistente.Clasificacion += incremento;

                // Calcular la diferencia porcentual en relación con el elemento anterior
                int posicion = popularidadLenguajes.FindIndex(x => x.NombreLenguaje == nombreLenguaje);
                if (posicion > 0)
                {
                    double diferenciaPorcentual = lenguajeExistente.Clasificacion - popularidadLenguajes[posicion - 1].Clasificacion;
                    lenguajeExistente.DiferenciaPorcentual = diferenciaPorcentual;
                }
                else
                {
                    // Si es el primer elemento, la diferencia porcentual es igual a la clasificación
                    lenguajeExistente.DiferenciaPorcentual = lenguajeExistente.Clasificacion;
                }
            }
            else
            {
                int nuevaPosicion = popularidadLenguajes.Count + 1;

                popularidadLenguajes.Add(new LenguajePopularidad
                {
                    Posicion = nuevaPosicion,
                    NombreLenguaje = nombreLenguaje,
                    Clasificacion = incremento,
                    DiferenciaPorcentual = 0
                });
            }

        }
    }
}
