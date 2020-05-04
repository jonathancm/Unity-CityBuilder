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
            HandleCameraMovement();

            if (inputSystem.GetMouseInput().actionType != InputSystem.ActionType.UI)
            {
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


        //
        // TODO: Create system to map input to game action
        //
        private void HandleCameraMovement()
        {
            InputSystem.MouseInput mouseInput = inputSystem.GetMouseInput();

            //
            // Reset Orientation
            //
            if (inputSystem.GetButtonState(KeyCode.Home) == InputSystem.ButtonState.wasJustPressed)
            {
                cameraRig.ResetOrientation();
                return;
            }


            //
            // Zooming
            //
            if (mouseInput.actionType != InputSystem.ActionType.UI && mouseInput.deltaScroll.y != 0)
            {
                float zoomAxis = Mathf.Clamp(mouseInput.deltaScroll.y, -1, 1);
                cameraRig.Zoom(zoomAxis);
            }


            //
            // Rotating
            //
            bool isRotating = false;
            if (inputSystem.GetButtonState(KeyCode.Q) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.E) == InputSystem.ButtonState.isHeldDown)
                isRotating = true;

            if (isRotating)
            {
                float axisRight = 0;
                if (inputSystem.GetButtonState(KeyCode.E) == InputSystem.ButtonState.isHeldDown)
                    axisRight = 1;

                float axisLeft = 0;
                if (inputSystem.GetButtonState(KeyCode.Q) == InputSystem.ButtonState.isHeldDown)
                    axisLeft = 1;

                float rotateAxis = Mathf.Clamp(axisRight - axisLeft, -1, 1);
                cameraRig.Rotate(rotateAxis);
            }


            //
            // Panning
            //
            bool isKeyboardPan = false;
            if (inputSystem.GetButtonState(KeyCode.W) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.A) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.S) == InputSystem.ButtonState.isHeldDown
                || inputSystem.GetButtonState(KeyCode.D) == InputSystem.ButtonState.isHeldDown)
                isKeyboardPan = true;

            bool isMousePan = false;
            if (mouseInput.actionType != InputSystem.ActionType.UI
                && mouseInput.dragStartPosition != null 
                && inputSystem.GetButtonState(KeyCode.Mouse1) == InputSystem.ButtonState.isHeldDown)
                isMousePan = true;

            if (isKeyboardPan)
            {
                float axisUp = 0;
                if (inputSystem.GetButtonState(KeyCode.W) == InputSystem.ButtonState.isHeldDown)
                    axisUp = 1;

                float axisDown = 0;
                if (inputSystem.GetButtonState(KeyCode.S) == InputSystem.ButtonState.isHeldDown)
                    axisDown = 1;

                float axisRight = 0;
                if (inputSystem.GetButtonState(KeyCode.D) == InputSystem.ButtonState.isHeldDown)
                    axisRight = 1;

                float axisLeft = 0;
                if (inputSystem.GetButtonState(KeyCode.A) == InputSystem.ButtonState.isHeldDown)
                    axisLeft = 1;

                Vector2 panAxis = Vector2.zero;
                panAxis.x = Mathf.Clamp(axisRight - axisLeft, -1, 1);
                panAxis.y = Mathf.Clamp(axisUp - axisDown, -1, 1);
                cameraRig.Pan(panAxis);
            }
            else if(isMousePan)
            {
                float mouseDeltaX = inputSystem.GetMouseInput().currentScreenPosition.x - inputSystem.GetMouseInput().dragStartPosition.Value.x;
                float mouseDeltaY = inputSystem.GetMouseInput().currentScreenPosition.y - inputSystem.GetMouseInput().dragStartPosition.Value.y;

                float axisRight = (mouseDeltaX / Screen.width) * 2;
                float axisUp = (mouseDeltaY / Screen.height) * 2;

                Vector2 panAxis = Vector2.zero;
                panAxis.x = Mathf.Clamp(axisRight, -1, 1);
                panAxis.y = Mathf.Clamp(axisUp, -1, 1);
                cameraRig.Pan(panAxis * 3);
            }
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
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.wasJustPressed && !isGridOccupied)
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
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.isHeldDown && !isGridOccupied)
                    {
                        CreateStructure(structureToBuild, playerHand.GridPosition);
                    }
                    break;
                }

                default: // Eraser
                {
                    if (inputSystem.GetButtonState(KeyCode.Mouse0) == InputSystem.ButtonState.isHeldDown && isGridOccupied)
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
