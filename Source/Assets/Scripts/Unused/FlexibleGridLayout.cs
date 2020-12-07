using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public FitType fit;
    public bool forceWidth;
    public bool forceHeight;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        float sqrt = Mathf.Sqrt(transform.childCount);

        switch (fit)
        {
            case FitType.Width:
                columns = Mathf.CeilToInt(sqrt);
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            case FitType.Height:
                rows = Mathf.CeilToInt(sqrt);
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.FixedColumns:
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            case FitType.FixedRows:
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.Uniform:
            default:
                columns = Mathf.CeilToInt(sqrt);
                rows = Mathf.CeilToInt(sqrt);
                break;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = ((parentWidth - padding.right - padding.left + spacing.x) / (float)columns) - spacing.x;
        float cellHeight = (parentHeight - padding.top - padding.bottom + spacing.y)/ (float) rows - spacing.y;

        cellSize.x = forceWidth? cellSize.x : cellWidth;
        cellSize.y = forceHeight? cellSize.y: cellHeight;

        int rowIdx, colIdx = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowIdx = i / columns;
            colIdx = i % columns;

            var item = rectChildren[i];

            var xPos = padding.right + (cellSize.x + spacing.x) * colIdx;
            var yPos = padding.top + (cellSize.y + spacing.y) * rowIdx;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
