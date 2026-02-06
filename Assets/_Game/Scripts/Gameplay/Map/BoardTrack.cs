using System.Collections.Generic;
using UnityEngine;
using GameDesign.Data;

namespace GameDesign.Gameplay.Map
{
    [System.Serializable]
    public class BoardTrack
    {
        public List<TileDataSO> Tiles = new List<TileDataSO>();
        public List<Vector3> WorldPositions = new List<Vector3>();

        public int Count => WorldPositions.Count;

        public Vector3 GetPosition(int index)
        {
            if (Count == 0) return Vector3.zero;
            return WorldPositions[index % Count];
        }

        public TileDataSO GetData(int index)
        {
            if (Tiles.Count == 0) return null;
            return Tiles[index % Tiles.Count];
        }
    }
}