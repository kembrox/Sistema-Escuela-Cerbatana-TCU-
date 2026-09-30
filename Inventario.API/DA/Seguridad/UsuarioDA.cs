using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Modelos.Seguridad;
using Dapper;
using Helpers;
using Microsoft.Data.SqlClient;

namespace DA.Seguridad
{
    public class UsuarioDA : IUsuarioDA
    {
        IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public UsuarioDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }
        public async Task<Guid> CrearUsuario(Usuario usuario)
        {
            var sql = @"[AgregarUsuario]";
            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(sql, new
            {
                NombreUsuario = usuario.NombreUsuario,
                PasswordHash = usuario.PasswordHash,
                CorreoElectronico = usuario.CorreoElectronico
            });
            return resultado;
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
