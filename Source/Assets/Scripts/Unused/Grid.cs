using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridObjChangedEventArgs> OnGridObjChanged;
    public class OnGridObjChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private TGridObject[,] gridArray;
    private int height;
    private int width;
    private float cellSize;
    private Vector3 originPos;

    public Grid(int width, int height, float cellSize, Vector3 originPos, Func<Grid<TGridObject>, int, int, TGridObject> createGridObj)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObj(this, x, y);
            }
        }

        bool debug = true;
        if (debug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);
        }
        
    }


    private Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPos;
    }

    private void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos.x - originPos.x) / cellSize);
        y = Mathf.FloorToInt((worldPos.y - originPos.y) / cellSize);
    }

    public void SetGrid(int x, int y, TGridObject obj)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = obj;
            if (OnGridObjChanged != null) OnGridObjChanged(this, new OnGridObjChangedEventArgs { x = x, y = y });

        }
    }

    public void TriggerGridObjChanged(int x, int y)
    {
        if (OnGridObjChanged != null) OnGridObjChanged(this, new OnGridObjChangedEventArgs { x = x, y = y });
    }

    public void SetGrid(Vector3 worldPos, TGridObject obj)
    {
        int x, y;
        GetXY(worldPos,out x, out y);
        SetGrid(x, y, obj);
    }

    public TGridObject GetGrid(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }
}
