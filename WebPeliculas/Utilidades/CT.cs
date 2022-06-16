using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPeliculas.Utilidades
{
    public static class CT
    {
        public static string UrlBaseAPI = "https://localhost:44317/";
        public static string RutaCategoriasAPI = UrlBaseAPI + "api/Categorias/";
        public static string RutaPeliculasAPI = UrlBaseAPI + "api/Peliculas/";
        public static string RutaUsuariosAPI = UrlBaseAPI + "api/Usuarios/";
    }
}
