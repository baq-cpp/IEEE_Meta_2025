using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Pin
    {
        public enum Direction { In, Out }
        public Component Owner { get; }
        public int Index { get; }
        public Direction Dir { get; }
        //public Net Net { get; internal set; }
        public bool CurrentSignal { get; set; }



        public Pin(Component owner, int index, Direction dir)
        {
            Owner = owner;
            Index = index;
            Dir = dir;
        }
    }