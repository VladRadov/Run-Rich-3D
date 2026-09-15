using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using RunRich3D.Services;
using RunRich3D.Views;

namespace RunRich3D.Editor
{
    public static class LevelWorldBaker
    {
        private const string ScenePath = "Assets/Run-Rich-3D/Scenes/Game.unity";
        private const string GroundMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/ground.asset";
        private const string BoxMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Box.asset";
        private const string FlagMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Flag.asset";
        private const string ChoiceDoorMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Door_Descent_02.asset";
        private const string PartyMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Party.asset";
        private const string StudyMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Study.asset";
        private const string FinishBluePath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/EndLevel_Blue.asset";
        private const string FinishGreenPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/EndLevel_Green.asset";
        private const string FinishOrangePath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/EndLevel_Orange.asset";
        private const string FinishYellowPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/EndLevel_Yellow.asset";
        private const string DoorPoorPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Door_Poor_000.asset";
        private const string DoorRichPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Door_Rich_00.asset";
        private const string DoorMillionPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Door_Million_00.asset";
        private const string FinishPlaneMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Plane.asset";
        private const string FinishStarMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/StarFinishtLine.002.asset";
        private const string EnviroMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/enviro_mat.mat";
        private const string CheckpointsMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/Checkpoints.mat";
        private const string ChoiceMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/props_Outline_Choice.mat";
        private const string FinishMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/Plane_Finish.mat";
        private const string GoodDoorMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/GoodDoor.mat";
        private const string BadDoorMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/BadDoor.mat";
        private const string PropsFlatMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/props_flat.mat";
        private const string FontPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Fonts/Inter-SemiBold.ttf";

        [MenuItem("Run Rich 3D/Bake Static Level Into Scene")]
        public static void Bake()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BakeActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Run Rich 3D: static level baked into the scene.");
        }

        internal static void BakeActiveScene()
        {
            LevelService levelService = FindSceneComponent<LevelService>();
            if (levelService == null || levelService.LevelRoot == null)
            {
                Debug.LogError("Run Rich 3D: LevelService or Level root is missing.");
                return;
            }

            Transform levelRoot = levelService.LevelRoot;
            GameObject staticGo = GetOrCreateChild(levelRoot, "Static");
            GameObject pickupGo = GetOrCreateChild(levelRoot, "Pickups");
            ClearChildren(staticGo.transform);

            FlagView[] flags = levelService.BakeStaticWorld(staticGo.transform, CreateCatalog());
            var so = new SerializedObject(levelService);
            so.FindProperty("_pickupRoot").objectReferenceValue = pickupGo.transform;
            SerializedProperty flagProp = so.FindProperty("_flagViews");
            flagProp.arraySize = flags.Length;
            for (int i = 0; i < flags.Length; i++)
            {
                flagProp.GetArrayElementAtIndex(i).objectReferenceValue = flags[i];
            }

            SerializedProperty gateProp = so.FindProperty("_gateView");
            if (gateProp != null)
            {
                gateProp.objectReferenceValue = staticGo.GetComponentInChildren<GateView>(true);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        internal static LevelWorldBuilder.Catalog CreateCatalog()
        {
            return new LevelWorldBuilder.Catalog(
                null,
                null,
                AssetDatabase.LoadAssetAtPath<Mesh>(GroundMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(BoxMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FlagMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(ChoiceDoorMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(PartyMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(StudyMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishBluePath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishGreenPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishOrangePath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishYellowPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(DoorPoorPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(DoorRichPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(DoorMillionPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishPlaneMeshPath),
                AssetDatabase.LoadAssetAtPath<Mesh>(FinishStarMeshPath),
                AssetDatabase.LoadAssetAtPath<Material>(EnviroMatPath),
                null,
                null,
                AssetDatabase.LoadAssetAtPath<Material>(CheckpointsMatPath),
                AssetDatabase.LoadAssetAtPath<Material>(ChoiceMatPath),
                AssetDatabase.LoadAssetAtPath<Material>(FinishMatPath),
                AssetDatabase.LoadAssetAtPath<Material>(GoodDoorMatPath),
                AssetDatabase.LoadAssetAtPath<Material>(BadDoorMatPath),
                AssetDatabase.LoadAssetAtPath<Material>(PropsFlatMatPath),
                AssetDatabase.LoadAssetAtPath<Font>(FontPath));
        }

        private static T FindSceneComponent<T>() where T : Component
        {
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                T found = roots[i].GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static GameObject GetOrCreateChild(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var created = new GameObject(name);
            created.transform.SetParent(parent, false);
            return created;
        }

        private static void ClearChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(root.GetChild(i).gameObject);
            }
        }
    }
}
