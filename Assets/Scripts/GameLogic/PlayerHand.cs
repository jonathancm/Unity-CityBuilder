using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class PlayerHand : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        public Color colorInspect = Color.blue;
        public Color colorValid = Color.green;
        public Color colorInvalid = Color.red;

        [SerializeField]
        private MeshObject _handMesh = null;

        [SerializeField]
        private MeshObject _previewMesh = null;

        //
        // Properties
        //
        public MeshObject HandMesh { get { return _handMesh; } }
        public MeshObject PreviewMesh { get { return _previewMesh; } }
        public Vector3 GridPosition { get { return gridPos; } }

        //
        // Cached References
        //
        private Camera mainCamera = null;
        private BoxCollider boxCollider = null;

        //
        // Internal Variables
        //
        private Vector3 worldPosition = Vector3.zero;
        private Vector3 gridPos = Vector3.zero;


        private void Start()
        {
            mainCamera = Camera.main;
            boxCollider = GetComponent<BoxCollider>();
        }


        public void UpdateWorldPosition()
        {
            if (!InputSystem.IsMouseOverGameWindow())
                return;
                
            RaycastHit hit;
            const float raycastDistance = 2000f;
            LayerMask layerMask = LayerMask.GetMask("Terrain");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                worldPosition = hit.point;
            }
        }

        //
        // TODO: Find way to add padding to snap to prevent buildings being placed on border
        //       Potential solution could be to ray cast in cross pattern and detect borders & corners
        //
        public void UpdateGridPosition()
        {
            float cellSize = GameControlSystem.GRIDSIZE;
            gridPos.x = Mathf.Floor((worldPosition.x + cellSize / 2) / cellSize) * cellSize;
            gridPos.y = worldPosition.y; // Snap to terrain
            gridPos.z = Mathf.Floor((worldPosition.z + cellSize / 2) / cellSize) * cellSize;
            transform.position = gridPos;
        }
    }
}
