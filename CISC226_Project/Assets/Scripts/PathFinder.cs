using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end){
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();

        openList.Add(start);

        while (openList.Count > 0){
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F ).First();
            if (currentOverlayTile.isBlocked){
                continue;
            }
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if(currentOverlayTile == end){
                // finalize our path
                return GetFinishedList(start, end);
            }


            foreach (var neighbour in GetNeighbourTiles(currentOverlayTile)){
                if(neighbour.isBlocked || closedList.Contains(neighbour)){
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);

                neighbour.previous = currentOverlayTile;

                if (!openList.Contains(neighbour) && !neighbour.isBlocked){
                    openList.Add(neighbour);
                }

            }

        }
        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;

        while (currentTile != start){
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse();
        return finishedList;

    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x-neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y-neighbour.gridLocation.y);
    }

    private  List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //top neighbour
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y+1);

        if(map.ContainsKey(locationToCheck) && !map[locationToCheck].isBlocked){
            neighbours.Add(map[locationToCheck]);
        }

        //bottom neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y-1);

        if(map.ContainsKey(locationToCheck)&& !map[locationToCheck].isBlocked){
            neighbours.Add(map[locationToCheck]);
        }

        //right neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x+1, currentOverlayTile.gridLocation.y);

        if(map.ContainsKey(locationToCheck)&& !map[locationToCheck].isBlocked){
            neighbours.Add(map[locationToCheck]);
        }

        //left neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x-1, currentOverlayTile.gridLocation.y);

        if(map.ContainsKey(locationToCheck)&& !map[locationToCheck].isBlocked){
            neighbours.Add(map[locationToCheck]);
        }
        
        return neighbours;
    }
}
