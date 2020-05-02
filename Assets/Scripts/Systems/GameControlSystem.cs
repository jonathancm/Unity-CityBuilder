using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CityBuilder
{
    public class GameControlSystem : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        public static float GRIDSIZE = 1.0f;

        //
        // Cached References
        //
        [SerializeField]
        private CityBuildingLibrary buildingLibrary = null;
        private InputSystem inputSystem = null;
        private CanvasMonitor canvasMonitor = null;
        private PlayerHand playerHand = null;
        private CameraRig cameraRig = null;

        //
        // Internal Variables
        //
        private Dictionary<int, BasicBuilding> buildingHashMap;
        private BasicBuilding structureToBuild = null;


        private void Start()
        {
            buildingHashMap = buildingLibrary.GetHashMap();
            inputSystem = GetComponent<InputSystem>();
            cameraRig = FindObjectOfType<CameraRig>();
            playerHand = FindObjectOfType<PlayerHand>();
            canvasMonitor = FindObjectOfType<CanvasMonitor>();
        }


        private void FixedUpdate()
        {
            playerHand.UpdateWorldPosition();
            playerHand.UpdateGridPosition();
        }


        //
        // TODO
        //
        // 1. Play car animations (Time.deltaTime * timeDilation)
        // 2. Play pedestrian animations (Time.deltaTime * timeDilation)
        // 3. Play Building Animations (Time.deltaTime * timeDilation)
        private void Update()
        {
            inputSystem.UpdateInput();
            if (inputSystem.ActionType != InputSystem.UserActionType.UI)
            {
                HandleCameraMovement();
                switch (canvasMonitor.GetActivePanel())
                {
                    case CanvasMonitor.ToolboxPanels.Construction:
                        HandleConstructionActions();
                        break;

                    case CanvasMonitor.ToolboxPanels.Eletricity:
                        HandleElectricityActions();
                        break;

                    case CanvasMonitor.ToolboxPanels.Aqueduct:
                        HandleAqueductActions();
                        break;

                    default: // None
                        HandleInspectActions();
                        break;
                }
            }
        }


        private void HandleCameraMovement()
        {
            if (inputSystem.Mouse.deltaScroll.y != 0)
                cameraRig.Zoom(inputSystem.Mouse.deltaScroll.y);

            if (inputSystem.Mouse.dragStartPosition != null && inputSystem.Mouse.rightClick == InputSystem.InputButtonState.isHeldDown)
                cameraRig.Pan(inputSystem.Mouse.dragStartPosition.Value, inputSystem.Mouse.currentScreenPosition);
        }

        private void HandleConstructionActions()
        {
            bool isGridOccupied = false;
            const float raycastDistance = 200f;
            LayerMask layerMask = LayerMask.GetMask("Buildings", "Roads");
            BasicBuilding structureAtLocation = null;

            if (structureToBuild == null)
            {
                playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInspect;
                return;
            }

            RaycastHit hitResult;
            if (Physics.Raycast(playerHand.GridPosition + new Vector3(0, raycastDistance - 1, 0), Vector3.down, out hitResult, raycastDistance, layerMask))
            {
                isGridOccupied = true;
                structureAtLocation = hitResult.collider.transform.root.GetComponent<BasicBuilding>();
            }

            //
            // TODO: Implement "PreviewDrag" feature (Available for roads).
            //       User clicks and holds. While holding, user can resize construction area.
            //       Once user releases mouse button, construction action is applied.
            //
            // Determine Type of action
            // 1. Build structure on click down
            // 2. Build Road on click down + hold
            // 3. Destroy on hold

            //
            // Update Cursor Color
            //
            switch (structureToBuild.descriptor.category)
            {
                case BuildingDescriptor.Category.Building:
                case BuildingDescriptor.Category.Roadway:
                case BuildingDescriptor.Category.PowerLine:
                case BuildingDescriptor.Category.Aqueduct:
                {
                    if (isGridOccupied)
                        playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInvalid;
                    else
                        playerHand.HandMesh.meshRenderer.material.color = playerHand.colorValid;

                    break;
                }

                default: // Eraser
                {
                    if (isGridOccupied)
                        playerHand.HandMesh.meshRenderer.material.color = playerHand.colorValid;
                    else
                        playerHand.HandMesh.meshRenderer.material.color = playerHand.colorInvalid;

                    break;
                }
            }

            //
            // Handle Click Events
            //
            switch (structureToBuild.descriptor.category)
            {
                case BuildingDescriptor.Category.Building:
                {
                    if (inputSystem.Mouse.leftClick == InputSystem.InputButtonState.wasJustPressed && !isGridOccupied)
                    {
                        CreateStructure(structureToBuild, playerHand.GridPosition);
                    }
                    break;
                }

                case BuildingDescriptor.Category.Roadway:
                case BuildingDescriptor.Category.PowerLine:
                case BuildingDescriptor.Category.Aqueduct:
                {
                    //
                    // Allow click and hold
                    //
                    if (inputSystem.Mouse.leftClick == InputSystem.InputButtonState.isHeldDown && !isGridOccupied)
                    {
                        CreateStructure(structureToBuild, playerHand.GridPosition);
                    }
                    break;
                }

                default: // Eraser
                {
                    if (inputSystem.Mouse.leftClick == InputSystem.InputButtonState.isHeldDown && isGridOccupied)
                    {
                        structureAtLocation.OnErase();
                        Destroy(structureAtLocation.gameObject);
                    }
                    break;
                }
            }
        }

        private void HandleElectricityActions()
        {

        }

        private void HandleAqueductActions()
        {

        }

        private void HandleInspectActions()
        {

        }

        public void SetSelectedStructure(int uuid)
        {
            playerHand.PreviewMesh.meshFilter.mesh = null;
            playerHand.PreviewMesh.meshRenderer.material = null;
            if (!buildingHashMap.ContainsKey(uuid))
                return;

            structureToBuild = buildingHashMap[uuid];
            MeshFilter[] meshFilters = structureToBuild.meshFilters;
            if (meshFilters.Length == 0)
                return;

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            playerHand.PreviewMesh.meshFilter.mesh = new Mesh();
            playerHand.PreviewMesh.meshFilter.mesh.CombineMeshes(combine);
            playerHand.PreviewMesh.meshRenderer.material = structureToBuild.meshRenderers[0].sharedMaterial;
        }

        public void CreateStructure(BasicBuilding.BuildingState structureInfo)
        {
            Vector3 gridPos = new Vector3(structureInfo.position.x, structureInfo.position.y, structureInfo.position.z);
            var newStructure = Instantiate(buildingHashMap[structureInfo.uuid], gridPos, Quaternion.identity);
            newStructure.GetComponent<BasicBuilding>().OnBuild();
        }

        public void CreateStructure(BasicBuilding structurePrefab, Vector3 gridPosition)
        {
            var newStructure = Instantiate(structurePrefab, gridPosition, Quaternion.identity);
            newStructure.OnBuild();
        }

        //public Vector2 WorldToGrid(Vector3 worldPos)
        //{
        //    Vector3 gridPos = Vector3.zero;
        //    gridPos.x = (int)((worldPos.x - gridSettings.cellSize / 2) / gridSettings.cellSize);
        //    gridPos.y = (int)((worldPos.y - gridSettings.cellSize / 2) / gridSettings.cellSize);
        //    gridPos.z = (int)((worldPos.z - gridSettings.cellSize / 2) / gridSettings.cellSize);
        //    return gridPos;
        //}

        //public Vector3 GridToWorld(Vector3 gridPos)
        //{
        //    Vector3 worldPos = Vector3.zero;
        //    worldPos.x = gridPos.x * gridSettings.cellSize + (gridSettings.cellSize / 2);
        //    worldPos.y = gridPos.y * gridSettings.cellSize + (gridSettings.cellSize / 2);
        //    worldPos.z = gridPos.z * gridSettings.cellSize + (gridSettings.cellSize / 2);
        //    return worldPos;
        //}
    }
}
