using System.Runtime.InteropServices;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Modelos.Seguridad;
using Dapper;
using Helpers;
using Microsoft.Data.SqlClient;

namespace DA.Seguridad
{
    public class SeguridadDA : ISeguridadDA
    {
        IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public SeguridadDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }


        public async Task<IEnumerable<Perfil>> ObtenerPerfilesxUsuario(Usuario usuario)
        {
           
            string sql = @"[ObtenerPerfilesUsuario]"; 

          
            var consulta = await _sqlConnection.QueryAsync<Abstracciones.Entidades.Seguridad.Perfil>(
                sql,
                new { IdUsuario = usuario.Id }
            );

            
            return Convertidor.ConvertirLista<Abstracciones.Entidades.Seguridad.Perfil, Abstracciones.Modelos.Seguridad.Perfil>(consulta);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            
            string sql = @"[ObtenerUsuario]";

            
            var consulta = await _sqlConnection.QueryAsync<Abstracciones.Entidades.Seguridad.Usuario>(
                sql,
                new { CorreoElectronico = usuario.CorreoElectronico, NombreUsuario = usuario.NombreUsuario }
            );

            
            return Convertidor.Convertir<Abstracciones.Entidades.Seguridad.Usuario, Abstracciones.Modelos.Seguridad.Usuario>(consulta.FirstOrDefault());
        }
    }
}
