using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimal_Api.Dominio.ModelViews
{
    public class Home
    {
        public string Mensagem { get => "Bem vindo a minha API de Veiculos"; }
        public string Marcelo { get => "https://marcelo.digitalbos.com.br"; }

        public string Documentacao { get => "/swagger"; }
    }
}