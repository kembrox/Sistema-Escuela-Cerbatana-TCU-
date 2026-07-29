using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DA
{
    public class InventarioDA : IInventarioDA
    {
        private readonly IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public InventarioDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        #region OPERACIONES
        public async Task<Guid> Agregar(InventarioResponse inventario)
        {
            string query = @"AgregarActivo"; 

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Guid.NewGuid(),
                CodigoFisico = inventario.CodigoFisico,
                Descripcion = inventario.Descripcion,
                IdCategoria = inventario.IdCategoria,
                IdUbicacion = inventario.IdUbicacion,
                Estado = inventario.Estado,
                Marca = inventario.Marca,
                Modelo = inventario.Modelo,
                Serie = inventario.Serie,
                Precio = inventario.Precio,
                Observaciones = inventario.Observaciones,
                UsuarioRegistra = inventario.UsuarioRegistra,
                FechaRegistro = DateTime.Now 
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Editar(Guid Id, InventarioResponse inventario)
        {
            await verificarInventarioExiste(Id);

            string query = @"EditarActivo";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Id,
                CodigoFisico = inventario.CodigoFisico,
                Descripcion = inventario.Descripcion,
                IdCategoria = inventario.IdCategoria,
                IdUbicacion = inventario.IdUbicacion,
                Estado = inventario.Estado,
                Observaciones = inventario.Observaciones,
                Marca = inventario.Marca,
                Modelo = inventario.Modelo,
                Serie = inventario.Serie,
                Precio = inventario.Precio,
                UsuarioModifica = inventario.UsuarioModifica,
                FechaModificacion = DateTime.Now 
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Eliminar(Guid Id)
        {
            await verificarInventarioExiste(Id);

            string query = @"EliminarActivo";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<IEnumerable<InventarioResponse>> Obtener()
        {
            string query = @"ObtenerActivos";

            var resultado = await _sqlConnection.QueryAsync<InventarioResponse>(query,
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<InventarioResponse> Obtener(Guid Id)
        {
            string query = @"ObtenerActivo";

            var resultadoConsulta = await _sqlConnection.QueryAsync<InventarioResponse>(query,
                new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultadoConsulta.FirstOrDefault();
        }
        #endregion

        #region Helpers

        private async Task verificarInventarioExiste(Guid Id)
        {
            InventarioResponse? resultadoConsultaInventario = await Obtener(Id);
            if (resultadoConsultaInventario == null)
                throw new Exception("El activo no existe en el inventario.");
        }

        #endregion
    }
}
