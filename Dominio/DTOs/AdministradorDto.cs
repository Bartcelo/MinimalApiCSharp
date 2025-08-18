using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_Api.Dominio.Enuns;

namespace Minimal_Api.Dominio.DTOs
{
    public class AdministradorDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public Perfil Perfil { get; set; }
    }
}