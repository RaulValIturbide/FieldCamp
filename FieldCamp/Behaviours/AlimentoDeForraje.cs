using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace FieldCamp.Behaviours
{
    public class AlimentoDeForraje
    {
        public static ZonaForrajeDTO ListaAlimentoDisponiblesEnTerreno(TerrainType terrainType)
        {
            ZonaForrajeDTO dto = new();
            switch(terrainType)
            {
                case TerrainType.Plain:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 1f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Mountain:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.7f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("cheese"), 0.3f);
                    dto.PosibilidadesExito = 0.75f;
                    break;
                case TerrainType.CoastalSea:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("fish"), 0.7f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.3f);
                    dto.PosibilidadesExito = 0.9f;
                    break;
                case TerrainType.Forest:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.25f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("meat"), 0.65f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("hog"), 0.1f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Beach:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.1f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("fish"), 0.6f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grape"), 0.1f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("meat"), 0.2f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Bridge:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("fish"), 0.7f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"),0.3f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Canyon:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grape"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("olives"), 0.2f);
                    dto.PosibilidadesExito = 0.8f;
                    break;
                case TerrainType.Desert:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.7f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("date_fruit"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("olives"), 0.1f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Dune:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.7f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("date_fruit"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("olives"), 0.1f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Cliff:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grape"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("olives"), 0.2f);
                    dto.PosibilidadesExito = 0.6f;
                    break;
                case TerrainType.Fording:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("fish"), 0.6f);
                    break;
                case TerrainType.RuralArea:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("cow"), 0.3f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("sheep"), 0.3f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("meat"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.2f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Swamp:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grape"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("fish"), 0.4f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("hog"), 0.1f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.3f);
                    dto.PosibilidadesExito = 1f;
                    break;
                case TerrainType.Snow:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("hog"), 0.3f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("beer"), 0.3f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("butter"), 0.2f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                case TerrainType.Steppe:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("sheep"), 0.5f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("cheese"), 0.2f);
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 0.3f);
                    dto.PosibilidadesExito = 0.4f;
                    break;
                default:
                    dto._ListaAlimentoDisponibles.Add(new ItemObject("grain"), 1f);                    
                    break;                    
            }
            return dto;
        }
    }
}
