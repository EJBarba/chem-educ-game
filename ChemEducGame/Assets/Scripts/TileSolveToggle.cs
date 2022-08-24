using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSolveToggle : MonoBehaviour
{
   public bool isSolved = false;
   public void TileSolved()
   {
        isSolved = true;
   }
   public bool TileState()
   {
        return isSolved;
   }
}
