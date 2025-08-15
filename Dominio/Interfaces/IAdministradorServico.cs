using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;

namespace Minimal_Api.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador Login(LoginDto loginDto);

        
    }
}