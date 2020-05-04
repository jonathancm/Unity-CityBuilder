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
            public int uuid;
            public BuildingDescriptor.Category category;
            public GridPosition position;
        }

        //
        // Configurable Parameters
        //
        public BuildingDescriptor descriptor;
        public BuildingState state;

        //
        // Internal Variables
        //
        protected Collider _searchResult = null;

        private void Awake()
        {
            state.uuid = descriptor.uuid;
            state.category = descriptor.category;
        }

        public MeshFilter[] meshFilters
        {
            get
            {
                return GetComponentsInChildren<MeshFilter>(false);
            }
        }

        public MeshRenderer[] meshRenderers
        {
            get
            {
                return GetComponentsInChildren<MeshRenderer>(false);
            }
        }

        public virtual void OnBuild()
        {
            state.position.x = (int)transform.position.x;
            state.position.y = (int)transform.position.y;
            state.position.z = (int)transform.position.z;
            RefreshVisuals();
        }

        public virtual void OnErase()
        {

        }

        public virtual void RefreshVisuals()
        {
            LayerMask layerMask = LayerMask.GetMask("Roads");
            if (SearchSurroundings(layerMask, Vector3.back))
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
            else if (SearchSurroundings(layerMask, Vector3.left))
                transform.rotation = Quaternion.LookRotation(Vector3.right);
            else if (SearchSurroundings(layerMask, Vector3.forward))
                transform.rotation = Quaternion.LookRotation(Vector3.back);
            else if (SearchSurroundings(layerMask, Vector3.right))
                transform.rotation = Quaternion.LookRotation(Vector3.left);
            else
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        public virtual void RefreshVisualsAndChainOnce()
        {
            throw new System.NotImplementedException();
        }

        protected bool SearchSurroundings(LayerMask layerMask, Vector3 searchDirection)
        {
            Vector3 searchLocation;
            Collider[] hitColliders;

            searchLocation = transform.position + (GameControlSystem.GRIDSIZE * searchDirection);
            hitColliders = Physics.OverlapSphere(searchLocation, GameControlSystem.GRIDSIZE / 4.0f, layerMask);
            if (hitColliders.Length > 0)
            {
                _searchResult = hitColliders[0];
                return true;
            }
            else
            {
                _searchResult = null;
                return false;
            }
        }
    }
}
