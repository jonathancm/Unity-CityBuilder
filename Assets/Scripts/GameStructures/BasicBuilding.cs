using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [SelectionBase]
    public class BasicBuilding : MonoBehaviour
    {
        [System.Serializable]
        public struct GridPosition
        {
            public int x;
            public int y;
            public int z;
        }

        [System.Serializable]
        public struct BuildingState
        {
            public GridPosition position;
            public BuildingDescriptor descriptor;
        }

        public BuildingState state;

        public MeshFilter meshFilter
        {
            get
            {
                return GetComponentInChildren<MeshFilter>();
            }
        }

        public MeshRenderer meshRenderer
        {
            get
            {
                return GetComponentInChildren<MeshRenderer>();
            }
        }

        public virtual void OnBuild()
        {
            state.position.x = (int)transform.position.x;
            state.position.y = (int)transform.position.y;
            state.position.z = (int)transform.position.z;
        }

        public virtual void OnErase()
        {

        }
    }
}
