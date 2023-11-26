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
            List<LenguajePopularidad> popularidadLenguajes = ObtenerPopularidadLenguajes(encuestas);

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

            string lenguajePrimario = encuesta.LenguajePrimario;
            var lenguajeExistente = popularidadLenguajes.FirstOrDefault(x => x.NombreLenguaje == lenguajePrimario);
            if (lenguajeExistente != null)
            {
                lenguajeExistente.Clasificacion += 1;
            }
            else
            {
                popularidadLenguajes.Add(new LenguajePopularidad
                {
                    Posicion = 1,
                    NombreLenguaje = lenguajePrimario,
                    Clasificacion = 1,
                    DiferenciaPorcentual = 0
                });
            }

            // Copia la lista antes de asignarla a la sesión
            List<LenguajePopularidad> copiaPopularidadLenguajes = new List<LenguajePopularidad>(popularidadLenguajes);
            Session[LenguajesKey] = copiaPopularidadLenguajes;

            encuestas.Add(encuesta);
            Session[EncuestasKey] = encuestas;
            if (encuesta.LenguajePrimario == encuesta.LenguajeSecundario)
            {
                // Si los lenguajes primario y secundario son iguales, muestra un mensaje de error
                ModelState.AddModelError("LenguajeSecundario", "El lenguaje secundario no puede ser igual al lenguaje primario.");
                // Puedes personalizar el mensaje de error según tus necesidades
                return View(encuesta);
            }
            return RedirectToAction("Index");
        }

        private List<Encuesta> ObtenerEncuestas()
        {
            List<Encuesta> encuestas = Session[EncuestasKey] as List<Encuesta>;
            List<LenguajePopularidad> popularidadLenguajes = ObtenerPopularidadLenguajes(encuestas);

            if (encuestas == null)
            {
                encuestas = new List<Encuesta>();
            }

            Session[LenguajesKey] = popularidadLenguajes;

            return encuestas;
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
            if (popularidadLenguajes != null && popularidadLenguajes.Any())
            {
                var lenguaje = popularidadLenguajes.FirstOrDefault(x => x.NombreLenguaje == nombreLenguaje);
                if (lenguaje != null)
                {
                    lenguaje.Clasificacion += incremento;

                    // Calcular la diferencia porcentual en relación con el elemento anterior
                    int posicion = popularidadLenguajes.FindIndex(x => x.NombreLenguaje == nombreLenguaje);
                    if (posicion > 0)
                    {
                        double diferenciaPorcentual = lenguaje.Clasificacion - popularidadLenguajes[posicion - 1].Clasificacion;
                        lenguaje.DiferenciaPorcentual = diferenciaPorcentual;
                    }
                    else
                    {
                        // Si es el primer elemento, la diferencia porcentual es igual a la clasificación
                        lenguaje.DiferenciaPorcentual = lenguaje.Clasificacion;
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
}
