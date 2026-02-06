using GameDesign.Data;
using System.Collections.Generic;
using UnityEngine;

namespace GameDesign.Gameplay.Map
{
    [System.Serializable]
    public class BoardTrack
    {
        public List<TileDataSO> Tiles = new List<TileDataSO>();
        public List<Vector3> WorldPositions = new List<Vector3>();

        public int Count => Tiles.Count;

        public Vector3 GetPosition(int index)
        {
            if (WorldPositions.Count == 0) return Vector3.zero;
            return WorldPositions[index % WorldPositions.Count];
        }
    }
}