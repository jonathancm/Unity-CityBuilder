using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class PlayerHand : MonoBehaviour
    {
        public Color colorInspect = Color.blue;
        public Color colorValid = Color.green;
        public Color colorInvalid = Color.red;

        [SerializeField]
        private MeshObject cursorMesh = null;

        [SerializeField]
        private MeshObject previewStructureMesh = null;

        public MeshObject Cursor
        {
            get
            {
                return cursorMesh;
            }
        }

        public MeshObject PreviewStructure
        {
            get
            {
                return previewStructureMesh;
            }
        }

        public BoxCollider Collider
        {
            get
            {
                return GetComponent<BoxCollider>();
            }
        }

    }
}
