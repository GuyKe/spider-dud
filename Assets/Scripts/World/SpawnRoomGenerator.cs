using System.Collections.Generic;
using UnityEngine;

namespace SpiderDud.World
{
    /// <summary>
    /// Procedurally builds the player's spawn room: a cluttered teenage
    /// bedroom (bed, messy desk, corkboard wall, window blinds, a hanging
    /// pendant lamp, scattered clutter). Built entirely from primitives with
    /// generic placeholder materials/colors — no copyrighted poster art or
    /// likenesses, just the general composition and mood.
    /// </summary>
    public class SpawnRoomGenerator : MonoBehaviour
    {
        [Header("Room Dimensions")]
        [SerializeField] private float roomWidth = 4.2f;
        [SerializeField] private float roomDepth = 4.6f;
        [SerializeField] private float roomHeight = 2.6f;
        [SerializeField] private int randomSeed = 2026;

        private System.Random rng;

        private void Awake()
        {
            if (transform.childCount == 0)
            {
                Generate();
            }
        }

        [ContextMenu("Generate Spawn Room")]
        public void Generate()
        {
            Clear();
            rng = new System.Random(randomSeed);

            BuildShell();
            BuildWindowAndBlinds();
            BuildDoor();
            BuildBed();
            BuildDesk();
            BuildChair();
            BuildCorkboardWall();
            BuildHangingLamp();
            BuildClutter();
        }

        [ContextMenu("Clear Spawn Room")]
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

        // ---- Shell -----------------------------------------------------

        private void BuildShell()
        {
            float wallThickness = 0.1f;

            CreateBox("Floor", ColorOf(0.42f, 0.30f, 0.20f), transform,
                new Vector3(0f, -wallThickness * 0.5f, 0f),
                new Vector3(roomWidth, wallThickness, roomDepth));

            CreateBox("Ceiling", ColorOf(0.92f, 0.91f, 0.87f), transform,
                new Vector3(0f, roomHeight + wallThickness * 0.5f, 0f),
                new Vector3(roomWidth, wallThickness, roomDepth));

            Color wallColor = ColorOf(0.88f, 0.85f, 0.78f);

            // South wall (behind spawn point, solid).
            CreateBox("Wall_South", wallColor, transform,
                new Vector3(0f, roomHeight * 0.5f, -roomDepth * 0.5f),
                new Vector3(roomWidth, roomHeight, wallThickness));

            // East wall (solid) — corkboard goes here.
            CreateBox("Wall_East", wallColor, transform,
                new Vector3(roomWidth * 0.5f, roomHeight * 0.5f, 0f),
                new Vector3(wallThickness, roomHeight, roomDepth));

            // West wall, split to leave a window gap.
            float windowWidth = 1.4f;
            float windowSegment = (roomDepth - windowWidth) * 0.5f;
            CreateBox("Wall_West_A", wallColor, transform,
                new Vector3(-roomWidth * 0.5f, roomHeight * 0.5f, -roomDepth * 0.5f + windowSegment * 0.5f),
                new Vector3(wallThickness, roomHeight, windowSegment));
            CreateBox("Wall_West_B", wallColor, transform,
                new Vector3(-roomWidth * 0.5f, roomHeight * 0.5f, roomDepth * 0.5f - windowSegment * 0.5f),
                new Vector3(wallThickness, roomHeight, windowSegment));
            CreateBox("Wall_West_Lintel", wallColor, transform,
                new Vector3(-roomWidth * 0.5f, roomHeight - 0.4f, 0f),
                new Vector3(wallThickness, 0.8f, windowWidth));

            // North wall, split to leave a doorway gap.
            float doorWidth = 0.9f;
            float doorHeight = 2.0f;
            float doorSegment = (roomWidth - doorWidth) * 0.5f;
            CreateBox("Wall_North_A", wallColor, transform,
                new Vector3(-roomWidth * 0.5f + doorSegment * 0.5f, roomHeight * 0.5f, roomDepth * 0.5f),
                new Vector3(doorSegment, roomHeight, wallThickness));
            CreateBox("Wall_North_B", wallColor, transform,
                new Vector3(roomWidth * 0.5f - doorSegment * 0.5f, roomHeight * 0.5f, roomDepth * 0.5f),
                new Vector3(doorSegment, roomHeight, wallThickness));
            CreateBox("Wall_North_Lintel", wallColor, transform,
                new Vector3(0f, doorHeight + (roomHeight - doorHeight) * 0.5f, roomDepth * 0.5f),
                new Vector3(doorWidth, roomHeight - doorHeight, wallThickness));
        }

        private void BuildWindowAndBlinds()
        {
            var blinds = new GameObject("WindowBlinds");
            blinds.transform.SetParent(transform, false);
            blinds.transform.position = new Vector3(-roomWidth * 0.5f + 0.03f, roomHeight - 0.5f, 0f);

            int slatCount = 12;
            float slatSpacing = 0.09f;
            for (int i = 0; i < slatCount; i++)
            {
                CreateBox($"Slat_{i}", ColorOf(0.82f, 0.80f, 0.74f), blinds.transform,
                    new Vector3(0f, -i * slatSpacing, 0f),
                    new Vector3(0.02f, 0.04f, 1.35f),
                    Quaternion.Euler(15f, 0f, 0f));
            }
        }

        private void BuildDoor()
        {
            float doorWidth = 0.9f;
            float doorHeight = 2.0f;
            GameObject door = CreateBox("Door", ColorOf(0.94f, 0.93f, 0.90f), transform,
                new Vector3(0f, doorHeight * 0.5f, roomDepth * 0.5f - 0.1f),
                new Vector3(doorWidth * 0.85f, doorHeight, 0.06f),
                Quaternion.Euler(0f, 20f, 0f));

            // Generic placeholder pinned portrait on the back of the door.
            CreateBox("DoorPortraitPlaceholder", ColorOf(0.97f, 0.95f, 0.9f), door.transform,
                new Vector3(0f, 0.15f, 0.04f),
                new Vector3(0.4f, 0.5f, 0.01f));
        }

        // ---- Furniture ---------------------------------------------------

        private void BuildBed()
        {
            Vector3 bedCorner = new Vector3(-roomWidth * 0.5f + 0.9f, 0f, -roomDepth * 0.5f + 1.1f);

            CreateBox("BedFrame", ColorOf(0.35f, 0.22f, 0.14f), transform,
                bedCorner + new Vector3(0f, 0.22f, 0f),
                new Vector3(1.6f, 0.4f, 2.1f));

            CreateBox("Mattress", ColorOf(0.88f, 0.86f, 0.82f), transform,
                bedCorner + new Vector3(0f, 0.48f, 0f),
                new Vector3(1.55f, 0.2f, 2.0f));

            // Rumpled blanket approximated by a couple of offset, tilted boxes.
            CreateBox("Blanket_A", ColorOf(0.55f, 0.12f, 0.14f), transform,
                bedCorner + new Vector3(0.1f, 0.63f, -0.2f),
                new Vector3(1.5f, 0.16f, 1.3f),
                Quaternion.Euler(2f, 6f, 0f));
            CreateBox("Blanket_B", ColorOf(0.55f, 0.12f, 0.14f), transform,
                bedCorner + new Vector3(-0.15f, 0.7f, 0.5f),
                new Vector3(1.3f, 0.14f, 0.7f),
                Quaternion.Euler(-3f, -8f, 4f));

            CreateBox("Pillow", ColorOf(0.9f, 0.9f, 0.88f), transform,
                bedCorner + new Vector3(0.15f, 0.66f, -0.85f),
                new Vector3(0.55f, 0.18f, 0.4f),
                Quaternion.Euler(0f, 10f, 0f));
        }

        private void BuildDesk()
        {
            Vector3 deskCenter = new Vector3(-roomWidth * 0.5f + 0.5f, 0f, -0.2f);

            CreateBox("DeskTop", ColorOf(0.4f, 0.27f, 0.17f), transform,
                deskCenter + new Vector3(0f, 0.75f, 0f),
                new Vector3(0.7f, 0.05f, 1.6f));

            foreach (float z in new[] { deskCenter.z - 0.75f, deskCenter.z + 0.75f })
            {
                CreateCylinder("DeskLeg", ColorOf(0.3f, 0.2f, 0.12f), transform,
                    new Vector3(deskCenter.x - 0.3f, 0.375f, z),
                    0.03f, 0.75f);
                CreateCylinder("DeskLeg", ColorOf(0.3f, 0.2f, 0.12f), transform,
                    new Vector3(deskCenter.x + 0.3f, 0.375f, z),
                    0.03f, 0.75f);
            }

            // Monitor + keyboard clutter on the desk.
            CreateBox("Monitor", ColorOf(0.08f, 0.08f, 0.08f), transform,
                deskCenter + new Vector3(0f, 0.95f, 0.5f),
                new Vector3(0.35f, 0.25f, 0.03f));
            CreateBox("Keyboard", ColorOf(0.15f, 0.15f, 0.15f), transform,
                deskCenter + new Vector3(0f, 0.79f, 0.5f),
                new Vector3(0.35f, 0.02f, 0.12f));

            // Overflowing bookshelf hutch above the desk.
            CreateBox("DeskHutch", ColorOf(0.4f, 0.27f, 0.17f), transform,
                deskCenter + new Vector3(0f, 1.6f, -0.5f),
                new Vector3(0.4f, 1.1f, 1f));
            BuildBookStack(deskCenter + new Vector3(0.05f, 1.3f, -0.85f), 6, new Vector3(0.05f, 0.02f, 0.22f));
        }

        private void BuildChair()
        {
            Vector3 seatPos = new Vector3(-roomWidth * 0.5f + 1.2f, 0.45f, 0.6f);
            Quaternion rot = Quaternion.Euler(0f, -35f, 0f);

            CreateBox("ChairSeat", ColorOf(0.5f, 0.08f, 0.1f), transform, seatPos, new Vector3(0.42f, 0.05f, 0.42f), rot);
            CreateBox("ChairBack", ColorOf(0.5f, 0.08f, 0.1f), transform,
                seatPos + rot * new Vector3(0f, 0.3f, -0.19f), new Vector3(0.42f, 0.5f, 0.05f), rot);
            CreateCylinder("ChairPost", ColorOf(0.15f, 0.15f, 0.15f), transform,
                seatPos + Vector3.down * 0.2f, 0.03f, 0.4f);
        }

        private void BuildCorkboardWall()
        {
            Vector3 boardCenter = new Vector3(roomWidth * 0.5f - 0.04f, 1.55f, 0.3f);

            CreateBox("Corkboard", ColorOf(0.65f, 0.5f, 0.32f), transform,
                boardCenter, new Vector3(0.03f, 1.2f, 1.6f), Quaternion.identity);

            // Grid of generic pinned "photo" squares — no real photos/likenesses.
            int rows = 4;
            int cols = 5;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float y = boardCenter.y + 0.45f - r * 0.28f;
                    float z = boardCenter.z - 0.7f + c * 0.32f;
                    Color tint = ColorOf(0.8f + RandFloat(-0.1f, 0.1f), 0.78f + RandFloat(-0.1f, 0.1f), 0.72f);
                    CreateBox("PinnedPhoto", tint, transform,
                        new Vector3(boardCenter.x - 0.02f, y, z),
                        new Vector3(0.005f, 0.12f, 0.09f));
                }
            }

            // Generic large map-style panel above the corkboard.
            CreateBox("MapPlaceholder", ColorOf(0.85f, 0.83f, 0.76f), transform,
                boardCenter + new Vector3(0f, 0.9f, 0f), new Vector3(0.02f, 0.7f, 1.1f));
        }

        private void BuildHangingLamp()
        {
            Vector3 top = new Vector3(0.6f, roomHeight, 1.2f);
            CreateCylinder("LampCord", ColorOf(0.05f, 0.05f, 0.05f), transform,
                top + Vector3.down * 0.3f, 0.008f, 0.6f);

            GameObject shade = CreateCylinder("LampShade", ColorOf(0.65f, 0.07f, 0.06f), transform,
                top + Vector3.down * 0.62f, 0.22f, 0.2f);

            var light = new GameObject("LampLight").AddComponent<Light>();
            light.transform.SetParent(shade.transform, false);
            light.transform.localPosition = Vector3.down * 0.15f;
            light.type = LightType.Point;
            light.color = ColorOf(1f, 0.85f, 0.7f);
            light.intensity = 1.6f;
            light.range = 4f;
        }

        private void BuildClutter()
        {
            var clutterRoot = new GameObject("FloorClutter");
            clutterRoot.transform.SetParent(transform, false);

            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = new Vector3(
                    RandFloat(-roomWidth * 0.5f + 0.3f, roomWidth * 0.2f),
                    0.05f,
                    RandFloat(-roomDepth * 0.2f, roomDepth * 0.5f - 0.3f));

                GameObject piece = CreateBox($"ClutterBook_{i}", RandomClutterColor(), clutterRoot.transform,
                    pos, new Vector3(RandFloat(0.15f, 0.3f), 0.03f, RandFloat(0.2f, 0.35f)),
                    Quaternion.Euler(0f, RandFloat(0f, 360f), 0f));
                RemoveCollider(piece);
            }
        }

        private void BuildBookStack(Vector3 basePos, int count, Vector3 bookSize)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject book = CreateBox($"Book_{i}", RandomClutterColor(), transform,
                    basePos + new Vector3(0f, i * (bookSize.y + 0.002f), RandFloat(-0.03f, 0.03f)),
                    bookSize, Quaternion.Euler(0f, RandFloat(-4f, 4f), 0f));
                RemoveCollider(book);
            }
        }

        // ---- Helpers -----------------------------------------------------

        private readonly Dictionary<Color, Material> materialCache = new Dictionary<Color, Material>();

        private GameObject CreateBox(string name, Color color, Transform parent, Vector3 localPos, Vector3 size, Quaternion? rotation = null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = rotation ?? Quaternion.identity;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = GetMaterial(color);
            return go;
        }

        private GameObject CreateCylinder(string name, Color color, Transform parent, Vector3 localPos, float radius, float height)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            go.GetComponent<Renderer>().sharedMaterial = GetMaterial(color);
            return go;
        }

        private static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(collider);
            }
            else
            {
                DestroyImmediate(collider);
            }
        }

        private Material GetMaterial(Color color)
        {
            if (materialCache.TryGetValue(color, out Material cached))
            {
                return cached;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = color };
            materialCache[color] = material;
            return material;
        }

        private static Color ColorOf(float r, float g, float b) => new Color(r, g, b);

        private float RandFloat(float min, float max)
        {
            return min + (float)rng.NextDouble() * (max - min);
        }

        private Color RandomClutterColor()
        {
            float shade = 0.3f + (float)rng.NextDouble() * 0.5f;
            return new Color(shade, shade * 0.9f, shade * 0.8f);
        }
    }
}
