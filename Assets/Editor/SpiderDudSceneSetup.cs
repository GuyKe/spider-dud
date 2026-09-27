using SpiderDud.Core;
using SpiderDud.Player;
using SpiderDud.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpiderDud.EditorTools
{
    /// <summary>
    /// Builds the demo city scene entirely through the Editor API, so Unity
    /// itself serializes the resulting .unity file (avoids hand-authored
    /// scene YAML, which is easy to get subtly wrong).
    ///
    /// Run this once after the project first opens, then follow SETUP.md to
    /// drop in the real XR Origin rig from the XR Interaction Toolkit
    /// samples (that part genuinely requires the Editor GUI).
    /// </summary>
    public static class SpiderDudSceneSetup
    {
        private const string SwingableLayerName = "Swingable";
        private const string ScenePath = "Assets/Scenes/CityDemo.unity";

        [MenuItem("Spider Dud/Build Demo City Scene")]
        public static void BuildScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var light = new GameObject("Directional Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 1.1f;

            EnsureLayer(SwingableLayerName);

            var cityGO = new GameObject("City");
            cityGO.AddComponent<CityGenerator>();

            var gameManagerGO = new GameObject("GameManager");
            gameManagerGO.AddComponent<GameManager>();

            BuildPlayer();

            System.IO.Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log(
                "Spider Dud demo scene created at " + ScenePath + ". " +
                "See SETUP.md for the remaining XR setup: installing XR Interaction Toolkit samples, " +
                "dropping the XR Origin (XR Rig) prefab into the Player object, and wiring its Camera/" +
                "LeftHand/RightHand transforms into WebSwingController.");
        }

        private static void BuildPlayer()
        {
            var playerGO = new GameObject("Player");
            playerGO.transform.position = new Vector3(0f, 2f, -10f);

            var controller = playerGO.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            playerGO.AddComponent<PlayerRespawn>();
            var swing = playerGO.AddComponent<WebSwingController>();

            Transform head = CreatePlaceholder("HeadAnchor (replace with XR Origin Camera)", playerGO.transform, new Vector3(0f, 1.6f, 0f));
            Transform leftHand = CreatePlaceholder("LeftHandAnchor (replace with XR Origin LeftHand Controller)", playerGO.transform, new Vector3(-0.3f, 1.2f, 0.2f));
            Transform rightHand = CreatePlaceholder("RightHandAnchor (replace with XR Origin RightHand Controller)", playerGO.transform, new Vector3(0.3f, 1.2f, 0.2f));

            LineRenderer leftLine = ConfigureWebLine(leftHand.gameObject.AddComponent<LineRenderer>());
            LineRenderer rightLine = ConfigureWebLine(rightHand.gameObject.AddComponent<LineRenderer>());

            var swingSO = new SerializedObject(swing);
            swingSO.FindProperty("headAnchor").objectReferenceValue = head;
            swingSO.FindProperty("leftHandAnchor").objectReferenceValue = leftHand;
            swingSO.FindProperty("rightHandAnchor").objectReferenceValue = rightHand;
            swingSO.FindProperty("leftWebLine").objectReferenceValue = leftLine;
            swingSO.FindProperty("rightWebLine").objectReferenceValue = rightLine;
            swingSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform CreatePlaceholder(string name, Transform parent, Vector3 localPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            return go.transform;
        }

        private static LineRenderer ConfigureWebLine(LineRenderer line)
        {
            line.positionCount = 2;
            line.startWidth = 0.02f;
            line.endWidth = 0.02f;
            line.enabled = false;
            line.material = new Material(Shader.Find("Sprites/Default"));
            return line;
        }

        private static void EnsureLayer(string layerName)
        {
            Object tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var tagManager = new SerializedObject(tagManagerAsset);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSlot = layers.GetArrayElementAtIndex(i);
                if (layerSlot.stringValue == layerName)
                {
                    return;
                }

                if (string.IsNullOrEmpty(layerSlot.stringValue))
                {
                    layerSlot.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }

            Debug.LogWarning($"Could not find a free layer slot for '{layerName}'. Add it manually in Project Settings > Tags and Layers.");
        }
    }
}
