using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture
{
    private List<HandPosition> positions;
    private float delay;

    public Gesture(float Delay)
    {
        delay = Delay;
        positions = new List<HandPosition>();
    }

    public void AddPoisition(HandPosition fingersPosition)
    {
        positions.Add(fingersPosition);
    }

   // public bool 
}
