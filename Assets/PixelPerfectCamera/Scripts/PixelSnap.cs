using UnityEngine;


/// <summary>
/// The script enables RetroSnap or PixelSnap.
/// <para />
/// RetroSnap will make the position of the object snap to the pixel grid as defined by the asset's pixels per unit. This will 
/// make the object move to multiples of screen pixels at once resulting in a more "snappy" old-school movement. Note, that 
/// RetroSnap works better with pixel perfect resolutions.
/// <para />
/// PixelSnap will make the position of the object snap to the screen's pixel grid. This can eliminate jittering when traslating 
/// an object under a non pixel perfect resolution. It can also help with artifacts related to subpixel positioning that can occur 
/// even in pixel perfect resolutions.
/// <para />
/// The script has 3 options: Off, Let PixelPerfect camera decide (default), Force Pixel Snap.
/// "Let PixelPerfect camera decide": When the current object gets rendered by a camera, the script will check if it is a 
/// PixelPerfectCamera. If it is, it will read the PixelPerfectCamera's PixelSnap mode setting and apply Retro Snap, Pixel Snap or 
/// nothing accordingly. If the object is rendered by a camera that doesn't have a PixelPerfectCamera component pixel snapping 
/// is not applied.
/// <para />
/// When "force Pixel Snap" is selected, Pixel Snap will be applied regardless of the camera that renders the object. Even when 
/// the object is rendered by a camera that doesn't have a PixelPerfectCamera component, Pixel Snap will be enabled.
/// </summary>
/// <remarks>
/// The script can be attached only on an object that has a renderer component. It will have no effect if attached to a 
/// gameobject without a renderer, even if its children are sprites for example.
/// <para />
/// The script adjusts the object's position (while rendering) so that it snaps. It then restores the original position.
/// <para />
/// For Sprites only: the script takes into account the pivot point and screen resolution for proper texel to screen-pixel placement. This can 
/// help with artifacts where 2 sprites appear to move in different speeds due to a different pivot point precision.
/// <para />
/// It works only when playing.
/// </remarks>
[RequireComponent(typeof(Renderer))]
public class PixelSnap : MonoBehaviour
{
    public enum PixelSnapChoiceMode { Off, LetCameraChoose, ForcePixelSnap};

    [Tooltip("Off: disables Pixel Snap \n" +
        "Let Camera Choose: respects the setting chosen in Pixel Perfect Camera (PPC) \n" +
        "Force Pixel Snap: enables Pixel Snap regardless of the PPC choice")]
    public PixelSnapChoiceMode pixelSnapChoiceMode = PixelSnapChoiceMode.LetCameraChoose;

    private Sprite sprite;
    private Vector3 actualPosition;
    private bool shouldRestorePosition;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void OnWillRenderObject()
    {
        //Debug.Log("on will" + Camera.current);
        if (spriteRenderer != null)
        {
            sprite = spriteRenderer.sprite;
        }
        else
        {
            sprite = null;
        }

        Camera cam = Camera.current;
        if (!cam)
            return;

        PixelPerfectCamera.PixelSnapMode pixelSnapMode = PixelPerfectCamera.PixelSnapMode.Off;
        PixelPerfectCamera pixelPerfectCamera = null;

        switch (pixelSnapChoiceMode)
        {
            case PixelSnapChoiceMode.Off:
                pixelSnapMode = PixelPerfectCamera.PixelSnapMode.Off;
                break;
            case PixelSnapChoiceMode.LetCameraChoose:
                pixelPerfectCamera = cam.GetComponent<PixelPerfectCamera>();
                if (pixelPerfectCamera == null)
                {
                    pixelSnapMode = PixelPerfectCamera.PixelSnapMode.Off;
                    break;
                }
                pixelSnapMode = pixelPerfectCamera.pixelSnapMode;
                break;
            case PixelSnapChoiceMode.ForcePixelSnap:
                pixelSnapMode = PixelPerfectCamera.PixelSnapMode.PixelSnap;
                break;
        }

        if (pixelSnapMode == PixelPerfectCamera.PixelSnapMode.Off)
        {
            return;
        }

        shouldRestorePosition = true;
        actualPosition = transform.position;

        float cameraPPU = (float)cam.pixelHeight / (2f * cam.orthographicSize);
        float cameraUPP = 1.0f / cameraPPU;

        Vector2 camPos = cam.transform.position.xy();
        Vector2 pos = actualPosition.xy();
        Vector2 relPos = pos - camPos;

        Vector2 offset = new Vector2(0, 0);
        // offset for screen pixel edge if screen size is odd
        offset.x = (cam.pixelWidth % 2 == 0) ? 0 : 0.5f;
        offset.y = (cam.pixelHeight % 2 == 0) ? 0 : 0.5f;
        
        // offset for pivot in Sprites
        Vector2 pivotOffset = new Vector2(0, 0);
        if (sprite != null)
        {
            // The vector from the nearest texel edge to the pivot towards the pivot in texture pixels 
            pivotOffset = sprite.pivot - new Vector2(Mathf.Round(sprite.pivot.x), Mathf.Round(sprite.pivot.y)); // the fractional part in texture pixels           
            if (pixelSnapMode == PixelPerfectCamera.PixelSnapMode.RetroSnap)
            {
                // Nothing to do here. Pivot offset is already in asset pixels.
            }
            else // PixelSnap
            {
                float camPixelsPerAssetPixel = cameraPPU / sprite.pixelsPerUnit;
                pivotOffset *= camPixelsPerAssetPixel; // convert to screen pixels
            }
        }

        if (pixelSnapMode == PixelPerfectCamera.PixelSnapMode.RetroSnap)
        {
            float assetPPU = pixelPerfectCamera.assetsPixelsPerUnit;
            float assetUPP = 1.0f / assetPPU;
            float camPixelsPerAssetPixel = cameraPPU / assetPPU;

            offset.x /= camPixelsPerAssetPixel; // zero or half a screen pixel in texture pixels
            offset.y /= camPixelsPerAssetPixel;
            
            // Convert from units to asset pixels, round them, convert back to units.
            relPos.x = (Mathf.Round(relPos.x / assetUPP - offset.x) + offset.x + pivotOffset.x) * assetUPP;
            relPos.y = (Mathf.Round(relPos.y / assetUPP - offset.y) + offset.y + pivotOffset.y) * assetUPP;

            // The offsets make sure that the distance we round is from screen pixel (fragment) edges to texel edges.
            // We don't take into account the pivot offset when rounding to avoid the artifact where 2 sprites appear to move at different times 
            // because of the fractional part of their pivots. We also consider the subtexel part of the pivot as an error which we want to correct.
        }
        else // PixelSnap
        {
            // Convert the units to screen pixels, round them, convert back to units. 
            relPos.x = (Mathf.Round(relPos.x / cameraUPP - offset.x) + offset.x + pivotOffset.x) * cameraUPP;
            relPos.y = (Mathf.Round(relPos.y / cameraUPP - offset.y) + offset.y + pivotOffset.y) * cameraUPP;

            // The comments reagarding offset and pivotOffest in the RetroSnap case above, also apply here.
        }

        pos = relPos + camPos;

        transform.position = new Vector3(pos.x, pos.y, actualPosition.z);
    }

    // This scripts is based on the assumption that every camera that calls OnWillRenderObject(), will call OnRenderObject() before any other
    // camera calls any of these methods.
    void OnRenderObject()
    {
        //Debug.Log("on did" + Camera.current);
        if (shouldRestorePosition)
        {
            shouldRestorePosition = false;
            transform.position = actualPosition;
        }
    }

}
