using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class BasicRoad : BasicBuilding
    {
        [System.Serializable]
        public struct RoadTiles
        {
            public GameObject straight;
            public GameObject turn;
            public GameObject threeWay;
            public GameObject fourWay;
        }

        [System.Serializable]
        public struct RoadConnnections
        {
            public BasicRoad front;
            public BasicRoad right;
            public BasicRoad back;
            public BasicRoad left;

            public int CountConnections()
            {
                int count = 0;

                count += (front != null ? 1 : 0);
                count += (right != null ? 1 : 0);
                count += (back != null ? 1 : 0);
                count += (left != null ? 1 : 0);

                return count;
            }

            public void Clear()
            {
                this.front = null;
                this.right = null;
                this.back = null;
                this.left = null;
            }
        }

        //
        // Configurable Parameters
        //
        public RoadTiles roadTiles;

        //
        // Internal Variables
        //
        private RoadConnnections connections;

        public override void OnBuild()
        {
            base.OnBuild();
            RefreshVisualsAndChainOnce();
        }

        public override void OnErase()
        {
            GetComponentInChildren<BoxCollider>().enabled = false;
            RefreshVisualsAndChainOnce();
            base.OnErase();
        }

        public override void RefreshVisuals()
        {
            UpdateTileConnections();
            RefreshTileVisual();
            NotifySurroundingBuildings();
        }

        public override void RefreshVisualsAndChainOnce()
        {
            UpdateTileConnections(true);
            RefreshTileVisual();
            NotifySurroundingBuildings();
        }




        //
        // Alternative to this could be to keep the location of everything in memory.
        // Grid Objects could simply lookup their surroundings using this.
        // Grid Objects would subsribe on start and unsubscribe on destroy
        //
        private void UpdateTileConnections(bool chainedUpdate = false)
        {
            LayerMask layerMask = LayerMask.GetMask("Roads");

            //
            // Search Algorithm
            //
            connections.Clear();
            if (SearchSurroundings(layerMask, Vector3.forward))
                connections.front = _searchResult.GetComponentInParent<BasicRoad>();
            if (SearchSurroundings(layerMask, Vector3.right))
                connections.right = _searchResult.GetComponentInParent<BasicRoad>();
            if (SearchSurroundings(layerMask, Vector3.back))
                connections.back = _searchResult.GetComponentInParent<BasicRoad>();
            if (SearchSurroundings(layerMask, Vector3.left))
                connections.left = _searchResult.GetComponentInParent<BasicRoad>();

            //
            // Chain update to connections (1 chain only)
            //
            if (chainedUpdate)
            {
                if (connections.front != null)
                    connections.front.RefreshVisuals();
                if (connections.right != null)
                    connections.right.RefreshVisuals();
                if (connections.back != null)
                    connections.back.RefreshVisuals();
                if (connections.left != null)
                    connections.left.RefreshVisuals();
            }
        }

        private void RefreshTileVisual()
        {
            roadTiles.straight.SetActive(false);
            roadTiles.turn.SetActive(false);
            roadTiles.threeWay.SetActive(false);
            roadTiles.fourWay.SetActive(false);

            switch (connections.CountConnections())
            {
                case 1:
                {
                    roadTiles.straight.SetActive(true);
                    if (connections.front != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    else if (connections.right != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    else if (connections.back != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    else // connections.left != null
                        transform.rotation = Quaternion.LookRotation(Vector3.left);

                    break;
                }

                case 2:
                {
                    if (connections.front != null && connections.back != null)
                    {
                        roadTiles.straight.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    }
                    else if (connections.left != null && connections.right != null)
                    {
                        roadTiles.straight.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else if (connections.front != null && connections.left != null)
                    {
                        roadTiles.turn.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    }
                    else if (connections.front != null && connections.right != null)
                    {
                        roadTiles.turn.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else if (connections.back != null && connections.right != null)
                    {
                        roadTiles.turn.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    }
                    else // connections.back != null && connections.left != null
                    {
                        roadTiles.turn.SetActive(true);
                        transform.rotation = Quaternion.LookRotation(Vector3.left);
                    }

                    break;
                }

                case 3:
                {
                    roadTiles.threeWay.SetActive(true);
                    if (connections.left != null && connections.front != null && connections.right != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    else if (connections.front != null && connections.right != null && connections.back != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    else if (connections.right != null && connections.back != null && connections.left != null)
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    else // connections.back != null && connections.left != null & connections.front != null
                        transform.rotation = Quaternion.LookRotation(Vector3.left);

                    break;
                }

                case 4:
                {
                    roadTiles.fourWay.SetActive(true);
                    transform.rotation = Quaternion.identity;
                    break;
                }

                default:
                {
                    roadTiles.straight.SetActive(true);
                    transform.rotation = Quaternion.identity;
                    break;
                }
            }
        }

        private void NotifySurroundingBuildings()
        {
            LayerMask layerMask = LayerMask.GetMask("Buildings");
            var buildings = new List<BasicBuilding>();

            //
            // Search Algorithm
            //
            if (SearchSurroundings(layerMask, Vector3.forward)) 
                buildings.Add(_searchResult.GetComponentInParent<BasicBuilding>());
            if (SearchSurroundings(layerMask, Vector3.right))
                buildings.Add(_searchResult.GetComponentInParent<BasicBuilding>());
            if (SearchSurroundings(layerMask, Vector3.back))
                buildings.Add(_searchResult.GetComponentInParent<BasicBuilding>());
            if (SearchSurroundings(layerMask, Vector3.left))
                buildings.Add(_searchResult.GetComponentInParent<BasicBuilding>());

            //
            // Notify
            //
            foreach (var building in buildings)
                building.RefreshVisuals();
        }
    }
}
