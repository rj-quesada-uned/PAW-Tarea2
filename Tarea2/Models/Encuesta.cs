using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tarea2.Models
{
    public class Encuesta
    {
        public int EncuestaId { get; set; }
        [Required(ErrorMessage = "El campo NombreEncuestado es requerido.")]

        public string NombreEncuestado { get; set; }
        [Required(ErrorMessage = "El campo ApellidoEncuestado es requerido.")]

        public string ApellidoEncuestado { get; set; }
        [Required(ErrorMessage = "El campo Pais es requerido.")]

        public Pais Pais { get; set; }
        [Required(ErrorMessage = "El campo RolTI es requerido.")]

        public RolTI RolTI { get; set; }
        [Required(ErrorMessage = "El campo LenguajePrimario es requerido.")]

        public string LenguajePrimario { get; set; }
        [Required(ErrorMessage = "El campo LenguajeSecundario es requerido.")]
        public string LenguajeSecundario { get; set; }
    }
    public enum RolTI
    {
        ProgramadorFrontEnd,
        ProgramadorBackEnd,
        AnalistaDeSistemas,
        DisenadorGrafico,
        AdministradorDeProyectosTI
    }

    public enum Pais
    {
        CostaRica,
        EstadosUnidos,
        Mexico,
        Colombia,
        Argentina
    }
}