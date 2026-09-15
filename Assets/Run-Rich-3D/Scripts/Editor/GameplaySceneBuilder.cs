using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using RunRich3D.Controllers;
using RunRich3D.Services;
using RunRich3D.Views;

namespace RunRich3D.Editor
{
    public static class GameplaySceneBuilder
    {
        private const string ScenePath = "Assets/Run-Rich-3D/Scenes/Game.unity";
        private const string SkyboxPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/skybox.mat";
        private const string EnviroMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/enviro_mat.mat";
        private const string PlayerMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/player_mat.mat";
        private const string PlayerAtlasPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/atlas_0.png";
        private const string CowboyCasualPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/cowboy_Casual.asset";
        private const string CowboyPoorPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Cowboy_Poor.asset";
        private const string CowboyMiddlePath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Cowboy_Middle.asset";
        private const string CowboyRichPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Cowboy_Rich.asset";
        private const string CowboyMillionairePath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/Cowboy_Millionaire.asset";
        private const string GroundMeshPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/ground.asset";
        private const string FontPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Fonts/Inter-SemiBold.ttf";
        private const string ButtonTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/button.png";
        private const string RetryTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/TryAgain.png";
        private const string DollarTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/Dollar_Green.png";
        private const string BillsTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/dollar_Logo.png";
        private const string ArrowTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/arrow_left_right.png";
        private const string FingerTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/hand.png";
        private const string SettingsTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/settings.png";
        private const string NoAdsTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/no_ads.png";
        private const string ShopSkinTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/shop_skin.png";
        private const string PickupsTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/pickups.png";
        private const string ParquetTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/parquet_violet.png";
        private const string PlusTexPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Texture2D/Plus.png";
        private const string DollarPrefabPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/LowPoly/bills.fbx";
        private const string BottlePrefabPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Mesh/LowPoly/bottle.fbx";
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
        private const string PropsFlatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/props_flat.mat";
        private const string PropsFlatBadPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/props_flat_bad.mat";
        private const string PaperBlocPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/PaperBloc.mat";
        private const string CheckpointsMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/Checkpoints.mat";
        private const string ChoiceMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/props_Outline_Choice.mat";
        private const string FinishMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/Plane_Finish.mat";
        private const string GoodDoorMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/GoodDoor.mat";
        private const string BadDoorMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/BadDoor.mat";
        private const string RedMatPath = "Assets/Run-Rich-3D/OtherAssets/Visual/Material/Red.mat";

        [MenuItem("Run Rich 3D/Rebuild Scene Hierarchy")]
        public static void SetupScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var skybox = AssetDatabase.LoadAssetAtPath<Material>(SkyboxPath);
            var enviroMat = AssetDatabase.LoadAssetAtPath<Material>(EnviroMatPath);
            var playerMat = AssetDatabase.LoadAssetAtPath<Material>(PlayerMatPath);
            var playerAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(PlayerAtlasPath);
            var cowboyCasual = AssetDatabase.LoadAssetAtPath<Mesh>(CowboyCasualPath);
            var cowboyPoor = AssetDatabase.LoadAssetAtPath<Mesh>(CowboyPoorPath);
            var cowboyMiddle = AssetDatabase.LoadAssetAtPath<Mesh>(CowboyMiddlePath);
            var cowboyRich = AssetDatabase.LoadAssetAtPath<Mesh>(CowboyRichPath);
            var cowboyMillionaire = AssetDatabase.LoadAssetAtPath<Mesh>(CowboyMillionairePath);
            var groundMesh = AssetDatabase.LoadAssetAtPath<Mesh>(GroundMeshPath);
            var hudFont = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            var buttonTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ButtonTexPath);
            var retryTex = AssetDatabase.LoadAssetAtPath<Texture2D>(RetryTexPath);
            var dollarTex = AssetDatabase.LoadAssetAtPath<Texture2D>(DollarTexPath);
            var billsTex = AssetDatabase.LoadAssetAtPath<Texture2D>(BillsTexPath);
            var arrowTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ArrowTexPath);
            var fingerTex = AssetDatabase.LoadAssetAtPath<Texture2D>(FingerTexPath);
            var settingsTex = AssetDatabase.LoadAssetAtPath<Texture2D>(SettingsTexPath);
            var noAdsTex = AssetDatabase.LoadAssetAtPath<Texture2D>(NoAdsTexPath);
            var shopSkinTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ShopSkinTexPath);
            var pickupsTex = AssetDatabase.LoadAssetAtPath<Texture2D>(PickupsTexPath);
            var parquetTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ParquetTexPath);
            var plusTex = AssetDatabase.LoadAssetAtPath<Texture2D>(PlusTexPath);
            var dollarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DollarPrefabPath);
            var bottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BottlePrefabPath);
            var boxMesh = AssetDatabase.LoadAssetAtPath<Mesh>(BoxMeshPath);
            var flagMesh = AssetDatabase.LoadAssetAtPath<Mesh>(FlagMeshPath);
            var choiceDoorMesh = AssetDatabase.LoadAssetAtPath<Mesh>(ChoiceDoorMeshPath);
            var partyMesh = AssetDatabase.LoadAssetAtPath<Mesh>(PartyMeshPath);
            var studyMesh = AssetDatabase.LoadAssetAtPath<Mesh>(StudyMeshPath);
            var finishBlue = AssetDatabase.LoadAssetAtPath<Mesh>(FinishBluePath);
            var finishGreen = AssetDatabase.LoadAssetAtPath<Mesh>(FinishGreenPath);
            var finishOrange = AssetDatabase.LoadAssetAtPath<Mesh>(FinishOrangePath);
            var finishYellow = AssetDatabase.LoadAssetAtPath<Mesh>(FinishYellowPath);
            var doorPoor = AssetDatabase.LoadAssetAtPath<Mesh>(DoorPoorPath);
            var doorRich = AssetDatabase.LoadAssetAtPath<Mesh>(DoorRichPath);
            var doorMillion = AssetDatabase.LoadAssetAtPath<Mesh>(DoorMillionPath);
            var finishPlaneMesh = AssetDatabase.LoadAssetAtPath<Mesh>(FinishPlaneMeshPath);
            var finishStarMesh = AssetDatabase.LoadAssetAtPath<Mesh>(FinishStarMeshPath);
            var propsFlat = AssetDatabase.LoadAssetAtPath<Material>(PropsFlatPath);
            var propsFlatBad = AssetDatabase.LoadAssetAtPath<Material>(PropsFlatBadPath);
            var paperBloc = AssetDatabase.LoadAssetAtPath<Material>(PaperBlocPath);
            var checkpointsMat = AssetDatabase.LoadAssetAtPath<Material>(CheckpointsMatPath);
            var choiceMat = AssetDatabase.LoadAssetAtPath<Material>(ChoiceMatPath);
            var finishMat = AssetDatabase.LoadAssetAtPath<Material>(FinishMatPath);
            var goodDoorMat = AssetDatabase.LoadAssetAtPath<Material>(GoodDoorMatPath);
            var badDoorMat = AssetDatabase.LoadAssetAtPath<Material>(BadDoorMatPath);
            var redMat = AssetDatabase.LoadAssetAtPath<Material>(RedMatPath);

            RenderSettings.skybox = skybox;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.86f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.48f, 0.78f, 0.95f);
            RenderSettings.ambientGroundColor = new Color(0.78f, 0.78f, 0.72f);

            var bootstrapRoot = GetOrCreateRoot("1. Bootstrap");
            var servicesRoot = GetOrCreateRoot("2. Services");
            var uiRoot = GetOrCreateRoot("3. UI");
            var gameRoot = GetOrCreateRoot("4. Game");

            var camera = EnsureCamera(gameRoot.transform);
            var directional = EnsureLight(gameRoot.transform);

            var path = GetOrCreateChild(gameRoot.transform, "Path");
            ConfigureMesh(path, groundMesh, enviroMat, new Vector3(0f, 0f, 28f), new Vector3(1f, 1f, 8f));
            if (groundMesh == null)
            {
                ConfigureBuiltinMesh(path, PrimitiveType.Cube, enviroMat, new Vector3(0f, -0.1f, 35f), new Vector3(6f, 0.2f, 80f));
            }

            var finish = GetOrCreateChild(gameRoot.transform, "Finish");
            finish.transform.localPosition = new Vector3(0f, 0f, 46f);
            finish.transform.localRotation = Quaternion.identity;
            finish.transform.localScale = Vector3.one;

            var levelRoot = GetOrCreateChild(gameRoot.transform, "Level");
            levelRoot.transform.localPosition = Vector3.zero;

            var player = GetOrCreateChild(gameRoot.transform, "Player");
            player.transform.localPosition = Vector3.zero;
            DestroyComponent<PlayerView>(player);

            Transform visual = player.transform.Find("Visual");
            if (visual == null)
            {
                var visualObject = new GameObject("Visual");
                visual = visualObject.transform;
                visual.SetParent(player.transform, false);
            }

            for (int i = visual.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(visual.GetChild(i).gameObject);
            }

            StripPlaceholderVisual(visual.gameObject);
            visual.localPosition = Vector3.zero;
            visual.localRotation = Quaternion.identity;
            visual.localScale = Vector3.one;

            var lightingService = EnsureService<LightingService>(servicesRoot.transform, "LightingService");
            var inputService = EnsureService<InputService>(servicesRoot.transform, "InputService");
            var playerService = EnsureService<PlayerService>(servicesRoot.transform, "PlayerService");
            var cameraService = EnsureService<CameraService>(servicesRoot.transform, "CameraService");
            var levelService = EnsureService<LevelService>(servicesRoot.transform, "LevelService");
            var gameLoopService = EnsureService<GameLoopService>(servicesRoot.transform, "GameLoopService");

            var lightingSo = new SerializedObject(lightingService);
            lightingSo.FindProperty("_sun").objectReferenceValue = directional;
            lightingSo.FindProperty("_skybox").objectReferenceValue = skybox;
            lightingSo.ApplyModifiedPropertiesWithoutUndo();

            var inputSo = new SerializedObject(inputService);
            inputSo.FindProperty("_uiRoot").objectReferenceValue = uiRoot.transform;
            inputSo.ApplyModifiedPropertiesWithoutUndo();

            var playerSo = new SerializedObject(playerService);
            playerSo.FindProperty("_playerEntity").objectReferenceValue = player;
            playerSo.FindProperty("_visualRoot").objectReferenceValue = visual;
            playerSo.FindProperty("_inputService").objectReferenceValue = inputService;
            playerSo.FindProperty("_labelFont").objectReferenceValue = hudFont;
            playerSo.FindProperty("_casualMesh").objectReferenceValue = cowboyCasual;
            playerSo.FindProperty("_poorMesh").objectReferenceValue = cowboyPoor;
            playerSo.FindProperty("_middleMesh").objectReferenceValue = cowboyMiddle;
            playerSo.FindProperty("_richMesh").objectReferenceValue = cowboyRich;
            playerSo.FindProperty("_millionaireMesh").objectReferenceValue = cowboyMillionaire;
            playerSo.FindProperty("_playerMaterial").objectReferenceValue = playerMat;
            playerSo.FindProperty("_playerAtlas").objectReferenceValue = playerAtlas;
            playerSo.ApplyModifiedPropertiesWithoutUndo();

            var cameraSo = new SerializedObject(cameraService);
            cameraSo.FindProperty("_camera").objectReferenceValue = camera;
            cameraSo.FindProperty("_playerService").objectReferenceValue = playerService;
            cameraSo.ApplyModifiedPropertiesWithoutUndo();

            var loopSo = new SerializedObject(gameLoopService);
            loopSo.FindProperty("_playerService").objectReferenceValue = playerService;
            loopSo.FindProperty("_inputService").objectReferenceValue = inputService;
            loopSo.FindProperty("_levelService").objectReferenceValue = levelService;
            loopSo.FindProperty("_font").objectReferenceValue = hudFont;
            loopSo.FindProperty("_buttonTexture").objectReferenceValue = buttonTex;
            loopSo.FindProperty("_retryTexture").objectReferenceValue = retryTex;
            loopSo.FindProperty("_dollarTexture").objectReferenceValue = dollarTex;
            loopSo.FindProperty("_billsTexture").objectReferenceValue = billsTex;
            loopSo.FindProperty("_arrowTexture").objectReferenceValue = arrowTex;
            loopSo.FindProperty("_fingerTexture").objectReferenceValue = fingerTex;
            loopSo.FindProperty("_settingsTexture").objectReferenceValue = settingsTex;
            loopSo.FindProperty("_noAdsTexture").objectReferenceValue = noAdsTex;
            loopSo.FindProperty("_shopSkinTexture").objectReferenceValue = shopSkinTex;
            loopSo.FindProperty("_pickupsTexture").objectReferenceValue = pickupsTex;
            loopSo.FindProperty("_parquetTexture").objectReferenceValue = parquetTex;
            loopSo.ApplyModifiedPropertiesWithoutUndo();

            var levelSo = new SerializedObject(levelService);
            levelSo.FindProperty("_levelRoot").objectReferenceValue = levelRoot.transform;
            levelSo.FindProperty("_playerService").objectReferenceValue = playerService;
            levelSo.FindProperty("_legacyPath").objectReferenceValue = path;
            levelSo.FindProperty("_dollarPrefab").objectReferenceValue = dollarPrefab;
            levelSo.FindProperty("_bottlePrefab").objectReferenceValue = bottlePrefab;
            levelSo.FindProperty("_moneyMaterial").objectReferenceValue = paperBloc != null ? paperBloc : propsFlat;
            levelSo.FindProperty("_bottleMaterial").objectReferenceValue = redMat != null ? redMat : (badDoorMat != null ? badDoorMat : propsFlatBad);
            levelSo.FindProperty("_plusSignTexture").objectReferenceValue = plusTex;
            levelSo.ApplyModifiedPropertiesWithoutUndo();

            DestroyComponent<GameBootstrap>(bootstrapRoot);
            var bootstrap = bootstrapRoot.AddComponent<GameBootstrap>();
            var bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("_lightingService").objectReferenceValue = lightingService;
            bootstrapSo.FindProperty("_inputService").objectReferenceValue = inputService;
            bootstrapSo.FindProperty("_playerService").objectReferenceValue = playerService;
            bootstrapSo.FindProperty("_cameraService").objectReferenceValue = cameraService;
            bootstrapSo.FindProperty("_levelService").objectReferenceValue = levelService;
            bootstrapSo.FindProperty("_gameLoopService").objectReferenceValue = gameLoopService;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            LevelWorldBaker.BakeActiveScene();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Run Rich 3D: hierarchy Bootstrap / Services / UI / Game is ready.");
        }

        private static GameObject GetOrCreateRoot(string name)
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].name == name)
                {
                    return roots[i];
                }
            }

            return new GameObject(name);
        }

        private static GameObject GetOrCreateChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var created = new GameObject(name);
            created.transform.SetParent(parent, false);
            return created;
        }

        private static T EnsureService<T>(Transform servicesRoot, string name) where T : Component
        {
            var go = GetOrCreateChild(servicesRoot, name);
            var service = go.GetComponent<T>();
            if (service == null)
            {
                service = go.AddComponent<T>();
            }

            return service;
        }

        private static Camera EnsureCamera(Transform gameRoot)
        {
            Camera camera = null;
            var existing = gameRoot.Find("Main Camera");
            if (existing != null)
            {
                camera = existing.GetComponent<Camera>();
            }

            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.transform.SetParent(gameRoot, false);
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                camera.tag = "MainCamera";
            }

            DestroyComponent<FollowCameraView>(camera.gameObject);
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 50f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 220f;
            camera.allowHDR = true;
            camera.transform.SetLocalPositionAndRotation(new Vector3(0f, 5.8f, -8.4f), Quaternion.Euler(18f, 0f, 0f));
            return camera;
        }

        private static Light EnsureLight(Transform gameRoot)
        {
            Light directional = null;
            var existing = gameRoot.Find("Directional Light");
            if (existing != null)
            {
                directional = existing.GetComponent<Light>();
            }

            if (directional == null)
            {
                var lightObject = new GameObject("Directional Light");
                lightObject.transform.SetParent(gameRoot, false);
                directional = lightObject.AddComponent<Light>();
                directional.type = LightType.Directional;
            }

            DestroyComponent<SceneLightingView>(directional.gameObject);
            directional.color = new Color(1f, 0.97f, 0.88f);
            directional.intensity = 1.15f;
            directional.shadows = LightShadows.Soft;
            directional.shadowStrength = 0.62f;
            directional.transform.localRotation = Quaternion.Euler(48f, 38f, 0f);
            RenderSettings.sun = directional;
            return directional;
        }

        private static void DestroyComponent<T>(GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component != null)
            {
                Object.DestroyImmediate(component);
            }
        }

        private static void StripPlaceholderVisual(GameObject visual)
        {
            DestroyComponent<MeshFilter>(visual);
            DestroyComponent<MeshRenderer>(visual);
            var colliders = visual.GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                Object.DestroyImmediate(colliders[i]);
            }
        }

        private static void ConfigureMesh(GameObject go, Mesh mesh, Material material, Vector3 position, Vector3 scale)
        {
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
            }

            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = go.AddComponent<MeshRenderer>();
            }

            if (mesh != null)
            {
                filter.sharedMesh = mesh;
            }

            if (material != null)
            {
                renderer.sharedMaterial = material;
            }

            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
        }

        private static void ConfigureBuiltinMesh(GameObject go, PrimitiveType type, Material material, Vector3 position, Vector3 scale)
        {
            var temp = GameObject.CreatePrimitive(type);
            var mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(temp);
            ConfigureMesh(go, mesh, material, position, scale);
        }
    }
}
