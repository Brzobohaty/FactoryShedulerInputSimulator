using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryShedulerInputSimulator
{
    class Device
    {
        public char status { get; set; }
        public char type { get; set; }


        public Device(char status, char type) {
            this.status = status;
            this.type = type;
        }
    }
}
