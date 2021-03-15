using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrideMap : MonoBehaviour
{
    public int horCount = 21, verCount = 21;
    public float space = 0.2f;
    public List<GrideCell> grideCells = new List<GrideCell>();

    public Vector3 startCornor;
    public int cellUnit = 1;

    public GrideCell selectedOne, selectedTwo;

    public List<GrideCell> macthList = new List<GrideCell>();

    public List<GrideCell> GrideCells
    {
        get { return grideCells; }
    }

    public ObjectShape target;
    public Transform canvasParent;

    private GrideMap map;
    void Start()
    {
        map = GetComponent<GrideMap>();
        map.GenerateMap();
        map.CellEventHandler();
    }

    public void GenerateMap()
    {
        foreach (var grideCell in GrideCells)
        {
            Util.SafeDestroy(grideCell.objectShape);
        }
        grideCells.Clear();
        for (int i = 0; i < horCount; i++)
        {
            for (int j = 0; j < verCount; j++)
            {
                GrideCell cell = new GrideCell(i, j);
                cell.pos = startCornor + new Vector3(cell.x * cellUnit, cell.y * cellUnit, 0) + new Vector3((i + 1) * space, (j + 1) * space, 0);
                GameObject obj = Instantiate(target.gameObject, cell.pos, Quaternion.identity);
                obj.transform.SetParent(canvasParent);
                ObjectShape objectShape = obj.GetComponent<ObjectShape>();
                objectShape.grideCell = cell;
                cell.SetObjctShade(objectShape);
                objectShape.SetValue(Random.Range(1, 5).ToString());
                grideCells.Add(cell);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (map != null && map.macthList.Count > 0)
        {
            for (int i = 0; i < map.macthList.Count; i++)
            {
                int j = i + 1;
                if (j < map.macthList.Count)
                {
                    Debug.DrawLine(map.macthList[i].pos, map.macthList[j].pos);
                }
            }
        }
    }

    public void CellEventHandler()
    {
        foreach (GrideCell grideCell in GrideCells)
        {
            grideCell.objectShape.onMouseDownAction = (cell) =>
            {
                if (selectedOne == null)
                {
                    selectedOne = cell;
                    selectedOne.objectShape.SetSelctedState();
                    return;
                }

                if (selectedOne != null && selectedTwo == null)
                {
                    selectedTwo = cell;
                    selectedTwo.objectShape.SetSelctedState();
                    //计算
                    if (Macth())
                    {
                        MatchSuccess();
                    }
                    else
                    {
                        MatchFail();
                    }
                }
            };
        }
    }

    private Coroutine drawline;
    void MatchSuccess()
    {
        if (drawline != null)
        {
            StopCoroutine(drawline);
        }

        drawline = StartCoroutine(DrawLine(() =>
        {
            selectedOne.objectShape.PairHandler();
            selectedTwo.objectShape.PairHandler();
            print("selectedOne>x:" + selectedOne.x + "  y:" + selectedOne.y);
            print("selectedTwo>x:" + selectedTwo.x + "  y:" + selectedTwo.y);
            selectedOne = null;
            selectedTwo = null;
            macthList.Clear();
        }));
    }

    void MatchFail()
    {
        if (drawline != null)
        {
            StopCoroutine(drawline);
        }

        drawline = StartCoroutine(DrawLine(() =>
        {
            selectedOne.objectShape.SetNotSelectedState();
            selectedTwo.objectShape.SetNotSelectedState();
            selectedOne = null;
            selectedTwo = null;
            macthList.Clear();
        }));
    }

    IEnumerator DrawLine(Action callBack)
    {
        if (map != null && map.macthList.Count > 0)
        {
            for (int i = 0; i < map.macthList.Count; i++)
            {
                int j = i + 1;
                if (j < map.macthList.Count)
                {
                    Debug.DrawLine(map.macthList[i].pos, map.macthList[j].pos);
                }
            }
        }
        yield return new WaitForSecondsRealtime(0.5f);
        if (callBack != null)
        {
            callBack();
        }
    }


    bool Macth()
    {
        bool isMatchSucess = false;
        macthList.Clear();
        if (selectedOne != null && selectedTwo != null)
        {
            if (selectedOne.objectShape.CompareValue(selectedTwo.objectShape))
            {
                if (selectedOne.x == selectedTwo.x || selectedOne.y == selectedTwo.y)
                {
                    if (NoPoint(selectedOne, selectedTwo))
                    {
                        isMatchSucess = true;
                        macthList.Add(selectedOne);
                        macthList.Add(selectedTwo);
                    }
                    else
                    {
                        GrideCell p0, p1;
                        if (TwoPoint(out p0, out p1))
                        {
                            isMatchSucess = true;
                            macthList.Add(selectedOne);
                            macthList.Add(p0);
                            macthList.Add(p1);
                            macthList.Add(selectedTwo);
                        }
                    }
                }
                else
                {
                    GrideCell p2;
                    if (OnePoint(selectedOne, selectedTwo, out p2))
                    {
                        isMatchSucess = true;
                        macthList.Add(selectedOne);
                        macthList.Add(p2);
                        macthList.Add(selectedTwo);
                    }
                    else
                    {
                        GrideCell p0, p1;
                        if (TwoPoint(out p0, out p1))
                        {
                            isMatchSucess = true;
                            macthList.Add(selectedOne);
                            macthList.Add(p0);
                            macthList.Add(p1);
                            macthList.Add(selectedTwo);
                        }
                    }
                }
            }

        }

        return isMatchSucess;
    }

    bool NoPoint(GrideCell selectedOne, GrideCell selectedTwo)
    {
        bool flag = false;

        if (selectedOne.x == selectedTwo.x)
        {
            int min = Mathf.Min(selectedOne.y, selectedTwo.y);
            int max = Mathf.Max(selectedOne.y, selectedTwo.y);

            bool flag1 = true;
            for (int i = min + 1; i < max; i++)
            {
                flag1 &= GetGrideCell(selectedOne.x, i).CellIsNull;
            }

            flag = flag1;
        }

        if (selectedOne.y == selectedTwo.y)
        {
            int min = Mathf.Min(selectedOne.x, selectedTwo.x);
            int max = Mathf.Max(selectedOne.x, selectedTwo.x);

            bool flag1 = true;
            for (int i = min + 1; i < max; i++)
            {
                flag1 &= GetGrideCell(i, selectedOne.y).CellIsNull;
            }

            flag = flag1;
        }

        return flag;
    }

    bool OnePoint(GrideCell selectedOne, GrideCell selectedTwo, out GrideCell p)
    {
        bool flag = false;
        p = null;
        foreach (GrideCell grideCell in GrideCells)
        {
            if (NoPoint(selectedOne, grideCell) && NoPoint(selectedTwo, grideCell))
            {
                if (grideCell.CellIsNull)
                {
                    flag = true;
                    p = grideCell;
                }
            }
        }
        return flag;
    }

    bool TwoPoint(out GrideCell p0, out GrideCell p1)
    {
        bool flag = false;
        p0 = null;
        p1 = null;
        GrideCell temp;
        foreach (GrideCell grideCell in GrideCells)
        {
            if (OnePoint(selectedOne, grideCell, out temp))
            {
                p0 = temp;
                if (grideCell.CellIsNull)
                {
                    if (NoPoint(grideCell, selectedTwo))
                    {
                        flag = true;
                        p1 = grideCell;
                    }
                }
            }
        }

        if (flag)
        {
            return flag;
        }

        foreach (GrideCell grideCell in GrideCells)
        {
            if (OnePoint(selectedTwo, grideCell, out temp))
            {
                p0 = temp;
                if (grideCell.CellIsNull)
                {
                    if (NoPoint(grideCell, selectedOne))
                    {
                        flag = true;
                        p1 = grideCell;
                    }
                }
            }
        }

        return flag;
    }


    public GrideCell GetGrideCell(int x, int y)
    {
        foreach (GrideCell grideCell in GrideCells)
        {
            if (x == grideCell.x && y == grideCell.y)
            {
                return grideCell;
            }
        }

        return null;
    }

    public GrideCell GetGridePosByWorldPos(Vector3 worlPos)
    {
        GrideCell cell = null;

        for (int i = 0; i < GrideCells.Count; i++)
        {
            int j = grideCells[i].x + 1;
            int k = GrideCells[i].y + 1;

            GrideCell cornorCell = GetGrideCell(j, k);
            if (cornorCell != null)
            {
                if (GrideCells[i].pos.x < worlPos.x && worlPos.x < cornorCell.x && GrideCells[i].pos.z < worlPos.z &&
                    worlPos.z < cornorCell.pos.z)
                {
                    cell = grideCells[i];
                }
            }
        }

        return cell;
    }

    public void Clear()
    {
        foreach (GrideCell grideCell in GrideCells)
        {
            Util.SafeDestroy(grideCell.objectShape);
            grideCell.SetObjctShade(null);
        }
    }
}

public class GrideCell
{
    public int x, y;
    public Vector3 pos;
    public ObjectShape objectShape;

    public bool CellIsNull
    {
        get { return !objectShape.gameObject.activeInHierarchy; }
    }


    public void SetObjctShade(ObjectShape objectShape)
    {
        this.objectShape = objectShape;
    }

    public GrideCell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}