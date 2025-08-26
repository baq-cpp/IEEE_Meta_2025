using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace breadboard
{
    public class Pin
    {

        public enum Direction { In, Out }
        public Component Owner { get; set; }
        public int Index { get; set; }
        public Direction Dir { get; set; }
        //public Net Net { get; internal set; }
        public bool CurrentSignal { get; set; }


        public Pin(Component owner, int index, Direction dir)
        {
            Owner = owner;
            Index = index;
            Dir = dir;
        }
    }
}
    
