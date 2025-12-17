using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Finals_Exam;

public class Primitives
{
    public Color color;
    public Texture2D texture;
    public Primitives(GraphicsDevice graphicsDevice, Color color)
    {
        this.color = color;
        texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData([color]);
    }
}