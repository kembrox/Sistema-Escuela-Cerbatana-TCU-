using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DA
{
    public class CategoriaDA : ICategoriaDA
    {
        private readonly IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public CategoriaDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        #region OPERACIONES
        public async Task<Guid> Agregar(CategoriaResponse categoria)
        {
            string query = @"AgregarCategoria";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Guid.NewGuid(), 
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Editar(Guid Id, CategoriaResponse categoria)
        {
            await verificarCategoriaExiste(Id);

            string query = @"EditarCategoria";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Eliminar(Guid Id)
        {
            await verificarCategoriaExiste(Id);

            string query = @"EliminarCategoria";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<IEnumerable<CategoriaResponse>> Obtener()
        {
            string query = @"ObtenerCategorias";

            var resultado = await _sqlConnection.QueryAsync<CategoriaResponse>(query,
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<CategoriaResponse> Obtener(Guid Id)
        {
            string query = @"ObtenerCategoria";

            var resultadoConsulta = await _sqlConnection.QueryAsync<CategoriaResponse>(query,
                new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultadoConsulta.FirstOrDefault();
        }
        #endregion
        #region Helpers

        private async Task verificarCategoriaExiste(Guid Id)
        {
            CategoriaResponse? resultadoConsultaCategoria = await Obtener(Id);
            if (resultadoConsultaCategoria == null)
                throw new Exception("La categoría no existe en el sistema.");
        }

        #endregion
    }
}
