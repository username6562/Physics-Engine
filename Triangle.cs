using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

class Triangle
{
    public Vector2 pointA;
    public Vector2 pointB;
    public Vector2 pointC;

    public Triangle(Vector2 pointA , Vector2 pointB , Vector2 pointC)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.pointC = pointC;
    }

    public void DrawTriangle(SpriteBatch spriteBatch , Texture2D pixel , Color color)
    {
        
    }

    public void DrawLine(Vector2 start , Vector2 end , SpriteBatch spriteBatch , Color color)
    {
        
    }
}