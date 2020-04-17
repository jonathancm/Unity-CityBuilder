using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CityBuilder
{
    public class GameController : MonoBehaviour
    {
        public enum MouseButtons
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        [System.Serializable]
        public struct UserInput
        {
            public bool leftClickDown;
            public bool leftClickUp;
            public bool leftClickHold;
            public bool rightClickDown;
            public bool rightClickUp;
            public bool rightClickHold;
            public bool middleClickDown;
            public bool middleClickUp;
            public bool middleClickHold;
        }

        [System.Serializable]
        public struct CameraSettings
        {
            public float panSpeed;
            public float minSize;
            public float maxSize;
        }

        [System.Serializable]
        public struct GridSettings
        {
            public float cellSize;
        }

        //
        // Configurable Parameters
        //
        public GridSettings gridSettings;
        public CameraSettings cameraSettings;

        //
        // Cached References
        //
        [SerializeField] private CityBuildingLibrary buildingLibrary = null;
        private Camera mainCamera = null;
        private CanvasMonitor canvasMonitor = null;
        private PlayerHand playerHand = null;
        private BasicBuilding structureToBuild = null;

        //
        // Internal Variables
        //
        private Dictionary<int, BasicBuilding> buildingHashMap;
        private UserInput userInput;
        private Vector2? initialMousePosition = null;
        private Vector3 mouseAimPos = Vector3.zero;
        private Vector3 mouseGridPos = Vector3.zero;

        void Start()
        {
            mainCamera = Camera.main;
            playerHand = FindObjectOfType<PlayerHand>();
            canvasMonitor = FindObjectOfType<CanvasMonitor>();

            buildingHashMap = buildingLibrary.GetHashMap();
        }

        void FixedUpdate()
        {
            if (!IsMouseOverGameWindow())
                return;

            RaycastHit hit;
            const float raycastDistance = 200f;
            LayerMask layerMask = LayerMask.GetMask("Terrain");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
                mouseAimPos = hit.point;
        }

        private bool IsMouseOverGameWindow()
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }

        void Update()
        {
            RegisterUserInput();
            UpdatePlayerHandPosition();

            bool isUiAction = false;
            if (userInput.leftClickDown || userInput.rightClickDown || userInput.middleClickDown)
            {
                isUiAction = canvasMonitor.isPointerOverGUI;
            }

            // Apply User Input
            if (!isUiAction)
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

            //
            // TODO
            //
            // 1. Play car animations (Time.deltaTime * timeDilation)
            // 2. Play pedestrian animations (Time.deltaTime * timeDilation)
            // 3. Play Building Animations (Time.deltaTime * timeDilation)
        }

        private void RegisterUserInput()
        {
            userInput.leftClickDown = Input.GetMouseButtonDown((int)MouseButtons.LeftClick);
            userInput.leftClickUp = Input.GetMouseButtonUp((int)MouseButtons.LeftClick);
            userInput.leftClickHold = Input.GetMouseButton((int)MouseButtons.LeftClick);
            userInput.rightClickDown = Input.GetMouseButtonDown((int)MouseButtons.RightClick);
            userInput.rightClickUp = Input.GetMouseButtonUp((int)MouseButtons.RightClick);
            userInput.rightClickHold = Input.GetMouseButton((int)MouseButtons.RightClick);
            userInput.middleClickDown = Input.GetMouseButtonDown((int)MouseButtons.MiddleClick);
            userInput.middleClickUp = Input.GetMouseButtonUp((int)MouseButtons.MiddleClick);
            userInput.middleClickHold = Input.GetMouseButton((int)MouseButtons.MiddleClick);
        }

        private void UpdatePlayerHandPosition()
        {
            if (playerHand == null)
                return;

            mouseGridPos.x = Mathf.Floor((mouseAimPos.x + gridSettings.cellSize / 2) / gridSettings.cellSize) * gridSettings.cellSize;
            mouseGridPos.y = mouseAimPos.y; // Snap to terrain
            mouseGridPos.z = Mathf.Floor((mouseAimPos.z + gridSettings.cellSize / 2) / gridSettings.cellSize) * gridSettings.cellSize;

            //
            // TODO: Find way to add padding to snap to prevent buildings being placed on border
            //       Potential solution could be to ray cast in cross pattern and detect borders & corners
            //
            playerHand.transform.position = mouseGridPos;
        }

        private void HandleCameraMovement()
        {
            //
            // Camera Zooming
            //
            if (Input.mouseScrollDelta.y != 0 && IsMouseOverGameWindow())
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - Input.mouseScrollDelta.y, cameraSettings.minSize, cameraSettings.maxSize);

            //
            // Camera Panning
            //
            if (userInput.rightClickDown && IsMouseOverGameWindow())
                initialMousePosition = Input.mousePosition;
            else if (userInput.rightClickUp)
                initialMousePosition = null;

            if (initialMousePosition != null && userInput.rightClickHold)
            {
                var mouseDelta = (Vector2)(Input.mousePosition) - initialMousePosition.Value;
                var camPosition = mainCamera.transform.position;
                camPosition.x += mouseDelta.x * cameraSettings.panSpeed * mainCamera.orthographicSize * Time.deltaTime;
                camPosition.z += mouseDelta.y * cameraSettings.panSpeed * mainCamera.orthographicSize * Time.deltaTime;
                mainCamera.transform.position = camPosition;
            }
        }

        private void HandleConstructionActions()
        {
            bool isGridOccupied = false;
            const float raycastDistance = 200f;
            LayerMask layerMask = LayerMask.GetMask("Buildings", "Roads");
            BasicBuilding structureAtLocation = null;

            if (!IsMouseOverGameWindow())
                return;

            if (structureToBuild == null)
            {
                playerHand.Cursor.meshRenderer.material.color = playerHand.colorInspect;
                return;
            }

            RaycastHit hitResult;
            if (Physics.Raycast(mouseGridPos + new Vector3(0, raycastDistance - 1, 0), Vector3.down, out hitResult, raycastDistance, layerMask))
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
                        playerHand.Cursor.meshRenderer.material.color = playerHand.colorInvalid;
                    else
                        playerHand.Cursor.meshRenderer.material.color = playerHand.colorValid;

                    break;
                }

                default: // Eraser
                {
                    if (isGridOccupied)
                        playerHand.Cursor.meshRenderer.material.color = playerHand.colorValid;
                    else
                        playerHand.Cursor.meshRenderer.material.color = playerHand.colorInvalid;

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
                    if (userInput.leftClickDown && !isGridOccupied)
                    {
                        CreateStructure(structureToBuild, mouseGridPos);
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
                    if (userInput.leftClickHold && !isGridOccupied)
                    {
                        CreateStructure(structureToBuild, mouseGridPos);
                    }
                    break;
                }

                default: // Eraser
                {
                    if (userInput.leftClickHold && isGridOccupied)
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
            playerHand.PreviewStructure.meshFilter.mesh = null;
            playerHand.PreviewStructure.meshRenderer.material = null;
            structureToBuild = buildingHashMap[uuid];
            if (structureToBuild == null)
                return;

            MeshFilter[] meshFilters = structureToBuild.meshFilters;
            if (meshFilters.Length == 0)
                return;

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            playerHand.PreviewStructure.meshFilter.mesh = new Mesh();
            playerHand.PreviewStructure.meshFilter.mesh.CombineMeshes(combine);
            playerHand.PreviewStructure.meshRenderer.material = structureToBuild.meshRenderers[0].sharedMaterial;
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

        public Vector2 WorldToGrid(Vector3 worldPos)
        {
            Vector3 gridPos = Vector3.zero;
            gridPos.x = (int)((worldPos.x - gridSettings.cellSize / 2) / gridSettings.cellSize);
            gridPos.y = (int)((worldPos.y - gridSettings.cellSize / 2) / gridSettings.cellSize);
            gridPos.z = (int)((worldPos.z - gridSettings.cellSize / 2) / gridSettings.cellSize);
            return gridPos;
        }

        public Vector3 GridToWorld(Vector3 gridPos)
        {
            Vector3 worldPos = Vector3.zero;
            worldPos.x = gridPos.x * gridSettings.cellSize + (gridSettings.cellSize / 2);
            worldPos.y = gridPos.y * gridSettings.cellSize + (gridSettings.cellSize / 2);
            worldPos.z = gridPos.z * gridSettings.cellSize + (gridSettings.cellSize / 2);
            return worldPos;
        }
    }
}
