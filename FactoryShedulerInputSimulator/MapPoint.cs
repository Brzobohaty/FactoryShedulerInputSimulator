using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Bod na mapě
/// </summary>
namespace FactoryShedulerInputSimulator
{
    public class MapPoint
    {
        public char state { set; get; }
        public int address { set; get; }
        public string type { set; get; }

        public MapPoint(char state, int address, string type) {
            this.state = state;
            this.address = address;
            this.type = type;
        }
    }
}
