using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PhysicsEngine;

public class Game1 : Game
{
    public GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D pixel;
    // public Circle circle ;
    
    int height;
    int width;
    List<Circle> circles = new List<Circle>();
    Circle circle;


    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight= 720;

        _graphics.ApplyChanges();
        width = _graphics.PreferredBackBufferWidth;
        height = _graphics.PreferredBackBufferHeight;
      

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        pixel = new Texture2D(GraphicsDevice , 1 , 1);
        pixel.SetData(new[] {Color.White});

        circle = new Circle(40f ,new Vector2(100, 50),  32,30,pixel,  Color.Red);
        // MultipleCircles(500000, 30);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
    

        base.Update(gameTime);

        for (int i = 0; i < circles.Count ; i++)
        {
            circles[i].Bounce(width , height);
            circles[i].ApplyGravity();
            if (circles[i].isSleeping)
            {
                continue;
            }
        }

        for (int i = 0 ; i < circles.Count ; i++)
        {
            for (int j = 0 ; j < circles.Count ; j++)
            {
                if (j == i)
                {
                    continue;
                }
                for (int r = 0 ; r < 10 ; r++)
                {
                    Circle.CircleToCircleCollision(circles[i] , circles[j]);
                }
            }
        }

        circle.ApplyGravity();
        Circle.CircleToLineCollision(circle , new Vector2(100 , 200), new Vector2(700 , 700));
        circle.Bounce(width , height);
        
    }
    

    protected override void Draw(GameTime gameTime)
    {
        
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();

       foreach (var circle in circles)
        {
            circle.Draw(_spriteBatch , true);
        }
        DrawTriangle(new Vector2(100 , 200), new Vector2(100, 700) , new Vector2( 700, 700));
        circle.Draw(_spriteBatch , true);
        _spriteBatch.End();
    }
    void MultipleCircles(int numberOfCircles , float radius )
    {
        Random random = new Random();
        for (int i = 0 ; i < numberOfCircles ; i++)
        {
            var mas = Random.Shared.Next(10, 20);
            
            int x = Random.Shared.Next(100 , width - (int)radius); 
            int y = Random.Shared.Next(400 , height - (int) radius);

            Circle newCircle = new Circle(1, new Vector2(x , y) , radius , 30, pixel, Color.Blue);

            bool overlapping= false;

            foreach (var existingCircles in circles)
            {
                if (newCircle.IsColliding(newCircle , existingCircles))
                {
                    overlapping = true;
                    break;
                }

            }
            if (!overlapping)
            {
                circles.Add(newCircle);
            }
        }
    }

    void CreatePyramid()
    {
    float radius = 20f;
    float spacing = radius * 2.2f;

    int rows = 5;
    float centerX = width / 2f;
    float startY = 100f;

        for (int row = 0; row < rows; row++)
        {
            int count = row + 1;

            float rowWidth = (count - 1) * spacing;
            float startX = centerX - rowWidth / 2f;

            float y = startY + row * spacing;

            for (int i = 0; i < count; i++)
            {
                float x = startX + i * spacing;

                circles.Add( new Circle(60 ,new Vector2(x, y),  radius,30,pixel,  Color.Red ));
            }
        }
    }

    void DrawTriangle(Vector2 sideA , Vector2 sideB , Vector2 sideC)
    {
        circle.DrawLine(sideA , sideB , Color.Blue , _spriteBatch);
        circle.DrawLine(sideA , sideC , Color.Blue , _spriteBatch);
        circle.DrawLine(sideB , sideC , Color.Blue , _spriteBatch);

    }
}

