using System.Text.Json;

namespace Helpers
{
    public static class Convertidor
    {
        public static TDestino Convertir<TOrigen, TDestino>(TOrigen origen)
        {
            if (origen == null) return default;

            var json = JsonSerializer.Serialize(origen);
            return JsonSerializer.Deserialize<TDestino>(json);
        }

        public static IEnumerable<TDestino> ConvertirLista<TOrigen, TDestino>(IEnumerable<TOrigen> origen)
        {
            if (origen == null) return new List<TDestino>();

            var json = JsonSerializer.Serialize(origen);
            return JsonSerializer.Deserialize<IEnumerable<TDestino>>(json);
        }
    }
}
