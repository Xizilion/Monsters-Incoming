using UnityEngine;

public class Cursor : MonoBehaviour
{
    public int Height;
    public int Width;
    public float OffsetX;
    public float OffsetY;
    public Texture2D Texture;

    private void Start()
    {
        Screen.showCursor = false;
    }

    private void OnGUI()
    {
        GUI.DrawTexture(
            new Rect((Event.current.mousePosition.x - Width / 2.0f) + OffsetX, 
                     (Event.current.mousePosition.y - Height / 2.0f) + OffsetY, 
                     Width, 
                     Height),
            Texture);
    }
}