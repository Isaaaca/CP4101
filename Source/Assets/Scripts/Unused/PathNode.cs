using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    public int gCost; //cost of Getting there
    public int hCost; //Heuristic cost
    public int fCost; 

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

}
