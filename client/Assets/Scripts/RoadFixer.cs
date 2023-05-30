using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeWay, fourWay;

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        //[right, up, left, down]
        var result = placementManager.GetNeighbourtTypesFor(temporaryPosition);
        int roadCount = 0;
        roadCount = result.Where(x => x == CellType.Road).Count();
        if(roadCount == 0 || roadCount == 1)
        {
            CreateDeadEnd(placementManager, result, temporaryPosition);
        }else if(roadCount == 2)
        {
            if (CreateStraightRoad(placementManager, result, temporaryPosition))
                return;
            CreateCorner(placementManager, result, temporaryPosition);
        }else if(roadCount == 3)
        {
            Create3Way(placementManager, result, temporaryPosition);
        }
        else
        {
            Create4Way(placementManager, result, temporaryPosition);
        }
    }

    private void Create4Way(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        throw new NotImplementedException();
    }

    private void Create3Way(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        throw new NotImplementedException();
    }

    private void CreateCorner(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        throw new NotImplementedException();
    }

    private bool CreateStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        throw new NotImplementedException();
    }

    private void CreateDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        throw new NotImplementedException();
    }
}
