using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace FieldCamp.Behaviours
{
    public class AlimentoDeForraje
    {
        // Caché : cada id se resuelve UNA sola vez en toda la sesión.
        private static readonly Dictionary<string, ItemObject> _cache = new();

        private static ItemObject Item(string id)
        {
            if (!_cache.TryGetValue(id, out var item))
            {
                item = MBObjectManager.Instance.GetObject<ItemObject>(id);
                if (item != null) _cache[id] = item;   // no cacheamos nulos
            }
            return item;
        }

        // Resuelve el id y solo lo añade si existe (evita claves null en el dict de pesos).
        private static void Add(ZonaForrajeDTO dto, string id, float peso)
        {
            ItemObject item = Item(id);
            if (item != null)
                dto._ListaAlimentoDisponibles[item] = peso;
        }

        public static ZonaForrajeDTO ListaAlimentoDisponiblesEnTerreno(TerrainType terrainType)
        {
            ZonaForrajeDTO dto = new();
            switch (terrainType)
            {
                case TerrainType.Plain:
                    Add(dto, "grain", 1f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Mountain:
                    Add(dto, "grain", 0.7f);
                    Add(dto, "cheese", 0.3f);
                    dto.PosibilidadesExito = 0.75f;
                    break;
                case TerrainType.CoastalSea:
                    Add(dto, "fish", 0.7f);
                    Add(dto, "grain", 0.3f);
                    dto.PosibilidadesExito = 0.9f;
                    break;
                case TerrainType.Forest:
                    Add(dto, "grain", 0.25f);
                    Add(dto, "meat", 0.65f);
                    Add(dto, "hog", 0.1f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Beach:
                    Add(dto, "grain", 0.1f);
                    Add(dto, "fish", 0.6f);
                    Add(dto, "grape", 0.1f);
                    Add(dto, "meat", 0.2f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Bridge:
                    Add(dto, "fish", 0.7f);
                    Add(dto, "grain", 0.3f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Canyon:
                    Add(dto, "grain", 0.4f);
                    Add(dto, "grape", 0.4f);
                    Add(dto, "olives", 0.2f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Desert:
                    Add(dto, "grain", 0.7f);
                    Add(dto, "date_fruit", 0.2f);
                    Add(dto, "olives", 0.1f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Dune:
                    Add(dto, "grain", 0.7f);
                    Add(dto, "date_fruit", 0.2f);
                    Add(dto, "olives", 0.1f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Cliff:
                    Add(dto, "grain", 0.4f);
                    Add(dto, "grape", 0.4f);
                    Add(dto, "olives", 0.2f);
                    dto.PosibilidadesExito = 0.6f;
                    break;
                case TerrainType.Fording:
                    Add(dto, "grain", 0.4f);
                    Add(dto, "fish", 0.6f);
                    dto.PosibilidadesExito = 0.8f;   
                    break;
                case TerrainType.RuralArea:
                    Add(dto, "cow", 0.3f);
                    Add(dto, "sheep", 0.3f);
                    Add(dto, "meat", 0.2f);
                    Add(dto, "grain", 0.2f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Swamp:
                    Add(dto, "grape", 0.2f);
                    Add(dto, "fish", 0.4f);
                    Add(dto, "hog", 0.1f);
                    Add(dto, "grain", 0.3f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Snow:
                    Add(dto, "hog", 0.3f);
                    Add(dto, "beer", 0.3f);
                    Add(dto, "grain", 0.2f);
                    Add(dto, "butter", 0.2f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Steppe:
                    Add(dto, "sheep", 0.5f);
                    Add(dto, "cheese", 0.2f);
                    Add(dto, "grain", 0.3f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                default:
                    Add(dto, "grain", 1f);
                    break;
            }
            return dto;
        }
    }
}