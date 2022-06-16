using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPeliculas.Models
{
    public class UserU
    {
        public int Id { get; set; }
        public string UsuarioA { get; set; }
        public string PasswordHash { get; set; }
    }
}
