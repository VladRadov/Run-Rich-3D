using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RunRich3D.Editor
{
    [InitializeOnLoad]
    public static class PlayModeInspectorGuard
    {
        private const string SafeAssetPath = "Assets/Run-Rich-3D";

        static PlayModeInspectorGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                LeavePrefabStage();
                RetargetAwayFromGameObjectInspector();
                return;
            }

            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.delayCall += RebuildInspectors;
            }
        }

        private static void LeavePrefabStage()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                StageUtility.GoToMainStage();
            }
        }

        private static void RetargetAwayFromGameObjectInspector()
        {
            if (!UsesGameObjectInspector(Selection.activeObject))
            {
                return;
            }

            Object safe = AssetDatabase.LoadMainAssetAtPath(SafeAssetPath);
            if (safe == null)
            {
                safe = AssetDatabase.LoadMainAssetAtPath("Assets");
            }

            if (safe != null)
            {
                Selection.activeObject = safe;
            }
        }

        private static bool UsesGameObjectInspector(Object selected)
        {
            return selected is GameObject || selected is Component;
        }

        private static void RebuildInspectors()
        {
            ActiveEditorTracker tracker = ActiveEditorTracker.sharedTracker;
            if (tracker != null)
            {
                tracker.ForceRebuild();
            }
        }
    }
}
