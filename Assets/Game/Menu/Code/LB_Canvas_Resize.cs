using UnityEngine;

/**
 * This resizes the canvas to the clickable area of a screen.
 * Source: https://www.behance.net/gallery/109643193/Updating-your-GUI-for-iPhone-X-and-Notched-Devices
 * Modified by: Laurence Buist
 */
public class LB_Canvas_Resize : MonoBehaviour
{
    RectTransform Panel;
    Rect LastSafeArea = new Rect (0, 0, 0, 0);

    // Bottom bar
    public RectTransform menuRectTransform;

    void Awake ()
    {
        Panel = GetComponent<RectTransform> ();
        Refresh ();
    }

    void Update ()
    {
        Refresh ();
    }

    void Refresh ()
    {
        Rect safeArea = GetSafeArea ();

        if (safeArea != LastSafeArea)
            ApplySafeArea (safeArea);
    }

    Rect GetSafeArea ()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea (Rect r)
    {
        LastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        /*Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;*/

        // Set scale of bottom bar, so that the bottom bar fits into the safe area of the screen
        menuRectTransform.localScale.Set(Screen.width / Screen.safeArea.width, 
            Screen.height / Screen.safeArea.height,
            0);

    }
}
