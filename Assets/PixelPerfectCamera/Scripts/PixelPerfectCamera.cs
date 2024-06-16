using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Adjusts the Camera component's orthographic size according to the supplied targeted size.
/// </summary>
/// <remarks>
/// If PixelPerfect is enabled, the camera's pixels per unit will be set to a multiple of the 
/// assets' pixels per unit.Thus, an asset pixel will render to an integer number of screen pixels.
/// <para />
/// In order to get pixel perfect result, point-sampling must be used in textures. Using linear filtering 
/// even in a 1-1 mapping, can be blurry. For example if your sprites have a center pivot and the 
/// screen has a non-multiple of 2 dimension or if you translate your sprites in sub-pixel 
/// values. Using point-sampling solves all these issues.
/// </remarks>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCamera : MonoBehaviour {

    public static int PIXELS_PER_UNIT = 100;

    public enum Dimension {Width, Height};
    public enum ConstraintType {None, Horizontal, Vertical};
    public enum PixelSnapMode {Off, RetroSnap, PixelSnap};

    // -- Input --
    [FormerlySerializedAs("maxCameraHalfWidthEnabled")]
    public bool maxCameraWidthEnabled = false;
    [FormerlySerializedAs("maxCameraHalfHeightEnabled")]
    public bool maxCameraHeightEnabled = false;
    [FormerlySerializedAs("maxCameraHalfWidth")]
    public float maxCameraWidth = 600;
    [FormerlySerializedAs("maxCameraHalfHeight")]
    public float maxCameraHeight = 400;
    public Dimension targetDimension = Dimension.Height;
    [FormerlySerializedAs("targetCameraHalfWidth")]
    public float targetCameraWidth = 400;
    [FormerlySerializedAs("targetCameraHalfHeight")]
    public float targetCameraHeight = 300;
    public bool pixelPerfect = false;
    public PixelSnapMode pixelSnapMode = PixelSnapMode.Off;
    public float assetsPixelsPerUnit = PIXELS_PER_UNIT;
    public bool showHUD;

    // -- Output --

    // The camera's half size in units
    [NonSerialized]
    public Vector2 cameraSize;
    
    // If max-width or max-height are used, this value shows how the final camera size 
    // was contrainsted
    [NonSerialized]
    public ConstraintType contraintUsed;

    // The number of screen pixels a unit is rendered to
    [NonSerialized]
    public float cameraPixelsPerUnit;

    // The number of screen pixels an asset pixel will render to
    [NonSerialized]
    public float ratio;

    // The camera resolution in asset pixels
    [NonSerialized]
    public Vector2 nativeAssetResolution;

    // How much of the targeted camera size was covered
    [NonSerialized]
    public float fovCoverage;

    // Becomes true after the camera runs once and performs the calculations. 
    // If false, the other output variables will have default values.
    [NonSerialized]
    public bool isInitialized;

    // -- Internal --
    Resolution res;
    Camera cam;

    //void TestMethod()
    //{
    //    float assetsPixelsPerUnit = 100;
    //    float assetWidth = 300;
    //    Resolution resolution = new Resolution();
    //    resolution.width = 1080;
    //    resolution.height = 1920;
    //    float cameraSize = calculatePixelPerfectCameraSize(true, resolution, assetsPixelsPerUnit, -1, -1, assetWidth, -1, Dimension.Width);
    //}

    float calculatePixelPerfectCameraSize(bool pixelPerfect, Resolution res, float assetsPixelsPerUnit, float maxCameraWidth, float maxCameraHeight
        , float targetWidth, float targetHeight, Dimension targetDimension)
    {
        float AR = (float)res.width / res.height;

        // How many screen pixels will an asset pixel render to?
        // or how many times will the asset dimensions be multiplied?
        float ratioTarget;
        
        if (targetDimension == Dimension.Width)
        {
            ratioTarget = res.width / targetWidth;
        }
        else
        {
            ratioTarget = res.height / targetHeight;        
        }
        float ratioTargetOriginal = ratioTarget;
        if (pixelPerfect)
        {
            float ratioSnapped = Mathf.Ceil(ratioTarget);
            float ratioSnappedPrevious = ratioSnapped - 1;
            // choose the ratio whose fov (or native asset resolution) is nearest to the ratioTarget's fov
            ratioTarget = (1/ratioTarget - 1 / ratioSnapped < 1/ratioSnappedPrevious - 1 / ratioTarget) ? ratioSnapped : ratioSnappedPrevious;
            if (ratioSnapped <= 1)
            {
                ratioTarget = 1;
            }
        }

        float ratioHorizontal = 0;
        float ratioVertical = 0;
        if (maxCameraWidth > 0f)
        {
            ratioHorizontal = res.width / maxCameraWidth;
        }
        if (maxCameraHeight > 0f)
        {
            ratioVertical = res.height / maxCameraHeight;
        }  
        float ratioMin = Mathf.Max(ratioHorizontal, ratioVertical);
        if (pixelPerfect)
        {
            ratioMin = Mathf.Ceil(ratioMin);
        }
        float ratioUsed = Mathf.Max(ratioMin, ratioTarget);

        float horizontalFOV = res.width / (assetsPixelsPerUnit * ratioUsed); // in units
        float verticalFOV = res.height / (assetsPixelsPerUnit * ratioUsed);

        // ------ GUI Calculations  -----
        this.cameraSize = new Vector2(horizontalFOV / 2, verticalFOV / 2);
        bool unconstrained = ratioTarget >= Mathf.Max(ratioHorizontal, ratioVertical) && ratioTargetOriginal >= Mathf.Max(ratioHorizontal, ratioVertical);
        this.contraintUsed = (unconstrained) ? ConstraintType.None : (ratioHorizontal > ratioVertical) ? ConstraintType.Horizontal : ConstraintType.Vertical;
        this.cameraPixelsPerUnit = res.width / horizontalFOV;
        this.ratio = ratioUsed;
        this.nativeAssetResolution = new Vector2(res.width / ratioUsed, res.height / ratioUsed);
        this.fovCoverage = ratioTargetOriginal / ratioUsed;
        this.isInitialized = true;
        // ------ GUI Calculations End  -----

        return verticalFOV / 2;
    }

    public void AdjustCameraFOV()
    {
        // Prevents PPC from running on prefabs. Otherwise, every time the editor's 
        // game resolution would change, the respective ".prefab" file would be updated 
        // with the Camera's new orthographicSize value.
#if UNITY_EDITOR
        if (!gameObject.activeInHierarchy)
            return;
#endif

        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        res = new Resolution();
        res.width = cam.pixelWidth;
        res.height = cam.pixelHeight;
        res.refreshRate = Screen.currentResolution.refreshRate;

        if (res.width == 0 || res.height == 0)
        {
            return;
        }

        float maxCameraWidthReq = (maxCameraWidthEnabled) ? maxCameraWidth : -1;
        float maxCameraHeightReq = (maxCameraHeightEnabled) ? maxCameraHeight : -1;
        float cameraSize = calculatePixelPerfectCameraSize(pixelPerfect, res, assetsPixelsPerUnit, maxCameraWidthReq, maxCameraHeightReq, targetCameraWidth, targetCameraHeight, targetDimension);

        cam.orthographicSize = cameraSize;
    }

    //void Start()
    //{
    //    //testMethod();
    //}

    void OnEnable()
    {
        AdjustCameraFOV();
    }


    void OnValidate () {
        // Migration: from half size in units to full size in asset pixels
        if (maxCameraWidth < 10)
        {
            maxCameraWidth = AssetPixelsFromHalfUnits(maxCameraWidth);
        }

        if (maxCameraHeight < 10)
        {
            maxCameraHeight = AssetPixelsFromHalfUnits(maxCameraHeight);
        }

        if (targetCameraWidth < 10)
        {
            targetCameraWidth = AssetPixelsFromHalfUnits(targetCameraWidth);
        }

        if (targetCameraHeight < 10)
        {
            targetCameraHeight = AssetPixelsFromHalfUnits(targetCameraHeight);
        }

        maxCameraWidth = Math.Max(maxCameraWidth, 20);
        maxCameraHeight = Math.Max(maxCameraHeight, 20);
        targetCameraWidth = Math.Max(targetCameraWidth, 20);
        targetCameraHeight = Math.Max(targetCameraHeight, 20);
        AdjustCameraFOV();
    }

//#if UNITY_EDITOR
    void Update () {
        if (res.width != cam.pixelWidth || res.height != cam.pixelHeight)
        {
            AdjustCameraFOV();
        }
	}
 //#endif


    // http://docs.unity3d.com/Manual/gui-Basics.html
    void OnGUI () {
        if (showHUD)
        {
            float scale = Screen.dpi / 96.0f;

            // Make a background box
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = (int)(13 * scale);
            GUI.Box(new Rect(10 * scale, 10 * scale, 260 * scale, 90 * scale), "Camera", boxStyle);

            // Make the first button. If it is pressed, update
            // http://forum.unity3d.com/threads/toggle-size.55615/
            GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
            toggleStyle.fontSize = (int)(13 * scale);
            toggleStyle.border = new RectOffset(0, 0, 0, 0);
            toggleStyle.overflow = new RectOffset(0, 0, 0, 0);
            toggleStyle.padding = new RectOffset(0,0,0,0);
            toggleStyle.imagePosition = ImagePosition.ImageOnly;
            pixelPerfect = GUI.Toggle(new Rect(20 * scale, 40 * scale, 20 * scale, 20 * scale), pixelPerfect, new GUIContent("Pixel perfect"), toggleStyle);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = (int)(13 * scale);
            GUI.Label(new Rect(40 * scale, 40 * scale, 80 * scale, 20 * scale), new GUIContent("Pixel perfect"), labelStyle);

            GUIStyle selectionStyle = new GUIStyle(GUI.skin.button);
            selectionStyle.fontSize = (int)(13 * scale);
            pixelSnapMode = (PixelSnapMode)GUI.SelectionGrid(new Rect(20 * scale, 65 * scale, 240 * scale, 20 * scale), (int)pixelSnapMode, Enum.GetNames(typeof(PixelSnapMode)), 3, selectionStyle);

            if (GUI.changed)
            {
                AdjustCameraFOV();
            }

        }
    }

    /// <summary>
    /// Convert from half size in untis to full size in asset pixels
    /// </summary>
    /// <param name="units">the camera's half width or half height in units</param>
    /// <returns>full size in asset pixels</returns>
    private float AssetPixelsFromHalfUnits(float units)
    {
        return units * 2 * assetsPixelsPerUnit;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(PixelPerfectCamera))]
[CanEditMultipleObjects]
public class PixelPerfectCameraEditor : Editor
{
    SerializedProperty maxCameraWidthEnabled;
    SerializedProperty maxCameraHeightEnabled;
    SerializedProperty maxCameraWidth;
    SerializedProperty maxCameraHeight;
    SerializedProperty targetDimension;
    SerializedProperty targetCameraWidth;
    SerializedProperty targetCameraHeight;
    SerializedProperty pixelPerfect;
    SerializedProperty pixelSnapMode;
    SerializedProperty assetsPixelsPerUnit;
    SerializedProperty showHUD;

    void OnEnable()
    {
        maxCameraWidthEnabled = serializedObject.FindProperty("maxCameraWidthEnabled");
        maxCameraHeightEnabled = serializedObject.FindProperty("maxCameraHeightEnabled");
        maxCameraWidth = serializedObject.FindProperty("maxCameraWidth");
        maxCameraHeight = serializedObject.FindProperty("maxCameraHeight");
        targetDimension = serializedObject.FindProperty("targetDimension");
        targetCameraWidth = serializedObject.FindProperty("targetCameraWidth");
        targetCameraHeight = serializedObject.FindProperty("targetCameraHeight");
        pixelPerfect = serializedObject.FindProperty("pixelPerfect");
        pixelSnapMode = serializedObject.FindProperty("pixelSnapMode");
        assetsPixelsPerUnit = serializedObject.FindProperty("assetsPixelsPerUnit");
        showHUD = serializedObject.FindProperty("showHUD");
    }

    public override void OnInspectorGUI()
    {
        // Using serialized objects requires a bit more work, but enables multi-object editing, undo, and Prefab overrides.
        // https://docs.unity3d.com/ScriptReference/Editor.html
        // https://stackoverflow.com/questions/55027410/editor-target-vs-editor-serializedobject

        serializedObject.Update();

        // Targeted Size
        PixelPerfectCamera.Dimension dimensionType = (PixelPerfectCamera.Dimension)Enum.GetValues(typeof(PixelPerfectCamera.Dimension)).GetValue(targetDimension.enumValueIndex);
        targetDimension.enumValueIndex = (int)(PixelPerfectCamera.Dimension)EditorGUILayout.EnumPopup("Target size", dimensionType);
        if (targetDimension.enumValueIndex == (int)PixelPerfectCamera.Dimension.Width)
        {
            EditorGUILayout.PropertyField(targetCameraWidth, new GUIContent("Width", "The target width of the camera in pixels."));
        }
        else
        {
            EditorGUILayout.PropertyField(targetCameraHeight, new GUIContent("Height", "The target height of the camera in pixels."));
        }
        EditorGUILayout.BeginHorizontal();
        maxCameraWidthEnabled.boolValue = EditorGUILayout.Toggle(maxCameraWidthEnabled.boolValue, GUILayout.Width(12));
        EditorGUI.BeginDisabledGroup(!maxCameraWidthEnabled.boolValue);
        EditorGUILayout.PropertyField(maxCameraWidth, new GUIContent("Max Width", "The maximum allowed width of the camera in pixels."));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        maxCameraHeightEnabled.boolValue = EditorGUILayout.Toggle(maxCameraHeightEnabled.boolValue, GUILayout.Width(12));
        EditorGUI.BeginDisabledGroup(!maxCameraHeightEnabled.boolValue);
        EditorGUILayout.PropertyField(maxCameraHeight, new GUIContent("Max Height", "The maximum allowed height of the camera in pixels."));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // Pixels Per Unit
        EditorGUILayout.PropertyField(assetsPixelsPerUnit);

        // Pixel Perfect toggle
        pixelPerfect.boolValue = EditorGUILayout.Toggle(new GUIContent("Pixel Perfect", 
            "Makes the camera's pixels per unit to be a multiple of the assets' pixels per unit."), pixelPerfect.boolValue);

        // PixelSnap menu
        PixelPerfectCamera.PixelSnapMode pixelSnapModeSelected = (PixelPerfectCamera.PixelSnapMode)Enum.GetValues(typeof(PixelPerfectCamera.PixelSnapMode)).GetValue(pixelSnapMode.enumValueIndex);
        pixelSnapMode.enumValueIndex = (int)(PixelPerfectCamera.PixelSnapMode)EditorGUILayout.EnumPopup(new GUIContent("Pixel Snap mode", 
            "Off: disables Pixel Snap \n" +
            "RetroSnap: Makes the objects snap to the asset's pixel grid \n" +
            "PixelSnap: Makes the objects snap to the screen's pixel grid \n\n" +
            "This option affects only objects that use the PixelSnap script."), pixelSnapModeSelected);

        // Show HUD toggle
        EditorGUILayout.PropertyField(showHUD);

        serializedObject.ApplyModifiedProperties();

        // Show results
        if (!((PixelPerfectCamera)target).isInitialized)
            return;
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        DrawSizeStats();
        GUILayout.EndVertical();
    }

    private void DrawSizeStats()
    {
        PixelPerfectCamera myCamera = (PixelPerfectCamera)target;
        EditorGUI.BeginDisabledGroup(true);

        // Size
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            string width = string.Format("{0:0.00}", myCamera.cameraSize.x);
            string height = string.Format("{0:0.00}", myCamera.cameraSize.y);
            if (myCamera.contraintUsed == PixelPerfectCamera.ConstraintType.Horizontal)
            {
                width = makeBold(width);
            }
            else if (myCamera.contraintUsed == PixelPerfectCamera.ConstraintType.Vertical)
            {
                height = makeBold(height);
            }
            string tooltipString = "The camera half size in units.";
            EditorGUILayout.LabelField(new GUIContent("Size", tooltipString), new GUIContent(string.Format("{0} x {1}", width, height)), style);
        }

        // Pixels Per Unit
        {
            string ppuString = string.Format("{0:0.00}", myCamera.cameraPixelsPerUnit);
            string tooltipString = "The number of screen pixels a unit is rendered to.";
            EditorGUILayout.LabelField(new GUIContent("Pixels Per Unit", tooltipString), new GUIContent(ppuString));
        }

        // Ratio
        {
            string ratioFormat = (myCamera.pixelPerfect) ? "{0:0}" : "{0:0.0000}";
            string pixelsString = string.Format(ratioFormat + "x [{1:0.00} x {2:0.00}]", myCamera.ratio, myCamera.nativeAssetResolution.x, myCamera.nativeAssetResolution.y);
            string tooltipString = "The screen resolution as a multiple of 2 numbers. The first is the number of screen pixels an asset pixel will render to. The second is the camera resolution in asset pixels.";
            EditorGUILayout.LabelField(new GUIContent("Pixels", tooltipString), new GUIContent(pixelsString));
        }

        // Target Coverage
        {
            string percentageUsed = string.Format("{0:P2}", myCamera.fovCoverage);
            string tooltipString = "How much of the targeted camera size was covered.";
            EditorGUILayout.LabelField(new GUIContent("Coverage", tooltipString), new GUIContent(percentageUsed));
        }

        EditorGUI.EndDisabledGroup();
    }

    private string makeBold(string str)
    {
        return "<b>" + str + "</b>";
    }


}
#endif
