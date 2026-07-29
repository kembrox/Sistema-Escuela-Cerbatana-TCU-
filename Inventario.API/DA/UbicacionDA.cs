using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DA
{
    public class UbicacionDA : IUbicacionDA
    {
        private readonly IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;

        public UbicacionDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        #region OPERACIONES
        public async Task<Guid> Agregar(UbicacionResponse ubicacion)
        {
            string query = @"AgregarUbicacion";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Guid.NewGuid(), 
                Nombre = ubicacion.Nombre,
                TipoArea = ubicacion.TipoArea,
                Estado = ubicacion.Estado
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Editar(Guid Id, UbicacionResponse ubicacion)
        {
            await verificarUbicacionExiste(Id);

            string query = @"EditarUbicacion";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Id,
                Nombre = ubicacion.Nombre,
                TipoArea = ubicacion.TipoArea,
                Estado = ubicacion.Estado
            }, commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<Guid> Eliminar(Guid Id)
        {
            await verificarUbicacionExiste(Id);

            string query = @"EliminarUbicacion";

            var resultado = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<IEnumerable<UbicacionResponse>> Obtener()
        {
            string query = @"ObtenerUbicaciones";

            var resultado = await _sqlConnection.QueryAsync<UbicacionResponse>(query,
                commandType: CommandType.StoredProcedure);

            return resultado;
        }

        public async Task<UbicacionResponse> Obtener(Guid Id)
        {
            string query = @"ObtenerUbicacion";

            var resultadoConsulta = await _sqlConnection.QueryAsync<UbicacionResponse>(query,
                new { Id = Id },
                commandType: CommandType.StoredProcedure);

            return resultadoConsulta.FirstOrDefault();
        }
        #endregion

        #region Helpers

        private async Task verificarUbicacionExiste(Guid Id)
        {
            UbicacionResponse? resultadoConsultaUbicacion = await Obtener(Id);
            if (resultadoConsultaUbicacion == null)
                throw new Exception("La ubicación no existe en el sistema.");
        }

        #endregion
    }
}
