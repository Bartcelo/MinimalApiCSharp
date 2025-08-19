
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_Api.Dominio.Entidades;

namespace Minimal_Api.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configurationAppSetings;

        public DbContexto(IConfiguration configurationAppSetings)
        {
            _configurationAppSetings = configurationAppSetings;
        }


        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        public IConfiguration ConfigurationAppSetings => _configurationAppSetings;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Email = "administrador@teste.com",
                    Senha = "123456",
                    Perfil = "Adm"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var stringConexao = ConfigurationAppSetings.GetConnectionString("mysql").ToString();
                if (!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
                }
            }
        }
    }
}