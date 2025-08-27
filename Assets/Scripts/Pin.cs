using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Pin : MonoBehaviour
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
