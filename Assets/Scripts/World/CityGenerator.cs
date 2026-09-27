using UnityEngine;

namespace SpiderDud.World
{
    /// <summary>
    /// Procedurally lays out a grid of placeholder box buildings so there's
    /// something to swing from without needing external art assets. All
    /// generated geometry is placed on the "Swingable" layer so
    /// WebSwingController's raycasts can find it.
    /// </summary>
    public class CityGenerator : MonoBehaviour
    {
        [SerializeField] private int gridSize = 8;
        [SerializeField] private float blockSpacing = 30f;
        [SerializeField] private Vector2 buildingFootprint = new Vector2(12f, 12f);
        [SerializeField] private Vector2 heightRange = new Vector2(20f, 90f);
        [SerializeField] private Material buildingMaterial;
        [SerializeField] private string swingableLayerName = "Swingable";
        [SerializeField] private int randomSeed = 12345;

        private void Awake()
        {
            if (transform.childCount == 0)
            {
                Generate();
            }
        }

        [ContextMenu("Generate City")]
        public void Generate()
        {
            Clear();

            var rng = new System.Random(randomSeed);
            int layer = ResolveLayer();

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    float height = Mathf.Lerp(heightRange.x, heightRange.y, (float)rng.NextDouble());

                    GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    building.name = $"Building_{x}_{z}";
                    building.layer = layer;
                    building.transform.SetParent(transform, false);
                    building.transform.position = new Vector3(x * blockSpacing, height * 0.5f, z * blockSpacing);
                    building.transform.localScale = new Vector3(buildingFootprint.x, height, buildingFootprint.y);

                    if (buildingMaterial != null)
                    {
                        building.GetComponent<Renderer>().sharedMaterial = buildingMaterial;
                    }
                }
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.layer = layer;
            ground.transform.SetParent(transform, false);
            float span = (gridSize - 1) * blockSpacing;
            ground.transform.position = new Vector3(span * 0.5f, 0f, span * 0.5f);
            ground.transform.localScale = Vector3.one * (gridSize * blockSpacing * 0.1f);
        }

        [ContextMenu("Clear City")]
        public void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private int ResolveLayer()
        {
            int layer = LayerMask.NameToLayer(swingableLayerName);
            if (layer < 0)
            {
                Debug.LogWarning(
                    $"CityGenerator: layer '{swingableLayerName}' not found. " +
                    "Add it in Project Settings > Tags and Layers, or run Spider Dud > Build Demo City Scene, " +
                    "which creates it automatically. Falling back to the Default layer for now.");
                return 0;
            }

            return layer;
        }
    }
}
