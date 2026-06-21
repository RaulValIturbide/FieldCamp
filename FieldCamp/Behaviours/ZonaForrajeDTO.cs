using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace FieldCamp.Behaviours
{
    public class ZonaForrajeDTO
    {
        public Dictionary<ItemObject, float> _ListaAlimentoDisponibles { get; set; } = new();
        public float PosibilidadesExito { get; set; } = 0.75f;

        public bool ExitoEnLaBusqueda()
        {
            return MBRandom.RandomFloatRanged(0, 1) <= PosibilidadesExito;
        }
    }
}
