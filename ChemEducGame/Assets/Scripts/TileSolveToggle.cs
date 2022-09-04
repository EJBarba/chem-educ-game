using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSolveToggle : MonoBehaviour
{
   public bool isSolved = false;
   public void TileSolved(bool state)
   {
        isSolved = state;
   }
   public bool TileState()
   {
        return isSolved;
   }
}
