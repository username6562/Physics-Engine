using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Data;
using System.Net.Http.Headers;

namespace PhysicsEngine;

public class Circle 
{
    
    public Vector2 Position;
    public float radius;
    public int Segments;
    public float mass;
    Texture2D pixel;
    Color color;
    public Vector2 velocity;
    static float restitution = 0.6f;
    float friction = 0.3f;
    public Vector2 gravity = new Vector2(0 , 9.8f);
    public bool isSleeping = false;

    const float SLEEP_THRESHOLD = 0.3f ;
    const float SLEEP_TIME = 40;
    int sleepCounter = 0;
    const float WAKE_THRESHOLD= 0.8f;
    const float FRICTION = 0.8f;
    float deltaTime = 0.9f;

    Vector2 acceleration;
    Vector2 force = new Vector2(100 , 0);
    public Circle(float mass ,Vector2 position, float radius, int segments, Texture2D pixel , Color color)
    {
        this.Position= position;
        this.radius = radius;
        this.Segments = segments;
        this.pixel = pixel;
        this.color = color;
        this.mass = mass;
        velocity = new Vector2(0, 2f);
        
    }
    public void Draw(SpriteBatch spriteBatch , bool fill) 
    {
        float step = MathHelper.TwoPi / Segments;
        for (int i = 0; i <Segments ; i++)
        {

            
            float angleA = i * step;
            float angleB = (i+1) *step;    

            Vector2 pointA = new Vector2(Position.X + (float)Math.Cos(angleA) * radius , Position.Y + (float)Math.Sin(angleA) * radius);

            Vector2 pointB = new Vector2(Position.X + (float)Math.Cos(angleB) *radius , Position.Y + (float)Math.Sin(angleB)* radius);

            
            DrawLine(pointA , pointB , color, spriteBatch);

        }
        if (fill)
        {
            for (int x  = (int) -radius ; x < radius ;x++)
            {
            for (int y = (int) -radius ; y < radius ; y++)
            {
                Vector2 point = new Vector2(Position.X + x , Position.Y + y);
                if (Vector2.Distance(point , Position) <= radius)
                {
                    spriteBatch.Draw(pixel , point , color);
                }
            }
            }
        }
        
    }
    public void DrawLine(Vector2 start, Vector2 end, Color color , SpriteBatch spriteBatch)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);

         spriteBatch.Draw(
            pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2),null,color,angle,Vector2.Zero,SpriteEffects.None,0);
        
    }
    public void Update()
    {
        ApplyGravity();
        
    }
    
    public void ApplyGravity()
    {
        
        acceleration = gravity / mass;
        velocity += acceleration * deltaTime ;
        Position += velocity ;
        
    }
    public void Bounce(float wallX,float floorY)
    {
        
        
        Vector2 ceilingNormal = new Vector2(0 , 1);
        Vector2 floorNormal = new Vector2(0 , -1);
        Vector2 rightWall = new Vector2(-1 , 0);
        Vector2 leftWall = new Vector2(1 , 0);
        if (Position.Y - radius <= 0)
        {
            Position.Y = 0 + radius;
            float velocityAlongNormal = Vector2.Dot(velocity , ceilingNormal);

            float impulse =  -(1+ restitution) *velocityAlongNormal;
            Vector2 impulseVector = impulse * ceilingNormal;

            velocity += impulseVector;
        }
        
        if (Position.Y + radius >= floorY)
        {
            Position.Y = floorY - radius;
             float velocityAlongNormal = Vector2.Dot(velocity ,floorNormal);

            float impulse =  -(1+ restitution) *velocityAlongNormal;
            Vector2 impulseVector = impulse * floorNormal;

            velocity += impulseVector;
            if (Math.Abs(velocity.X)>0)
                velocity.X *= friction;
        }
        if ( Position.X - radius <= 0)
        {
            Position.X = 0 + radius;
             float velocityAlongNormal = Vector2.Dot(velocity , leftWall);

            float impulse =  -(1+ restitution) *velocityAlongNormal;
            Vector2 impulseVector = impulse * leftWall;

            velocity += impulseVector;
        }
        if (Position.X +radius >= wallX)
        {
            Position.X = wallX - radius;
             float velocityAlongNormal = Vector2.Dot(velocity , rightWall);

            float impulse =  -(1+ restitution) *velocityAlongNormal;
            Vector2 impulseVector = impulse * rightWall;

            velocity += impulseVector;
        }

        
    }
    public void GetAngleTrajectory(int angle , float speed)
    {
        float radian = MathHelper.ToRadians(angle);
        var x = (float)Math.Cos(radian) * speed;
        var y = (float)-Math.Sin(radian) * speed;
        velocity = new Vector2(x , y);
    }
    

    public static void CircleToCircleCollision(Circle a , Circle b)
    {
         // Delta returns the Distance between to Circles and Stores it in a Vector2
         // So To Get Direction later
         // Distance Returns the Distance Between The Points
        Vector2 delta = b.Position - a.Position;
        Vector2 relativeVelocity = b.velocity - a.velocity;
        float distance = delta.Length();
        
        if (distance == 0)
        {
            delta = new Vector2(1 , 0); 
            distance = 1f;
        }

        //Returns How much circles a repenetrating
        float overlap = a.radius + b.radius- distance;

        if (overlap > 0)
        {
            // It Gets It Direction  while removing the magnitude of delta
            // Summary : Delta os the distance with direction
            // Distance is only the distance
            // Normal is the just the direction
            Vector2 normal = delta / distance;

            Vector2 correction = normal * (overlap *0.5f);

            a.Position -= correction;
            b.Position += correction;
            
            float velocityAlongNormal = Vector2.Dot(relativeVelocity , normal);

            if (velocityAlongNormal < 0)
            {
                
                var totalVelocity = -(1 + restitution) * velocityAlongNormal;

                var impulse = totalVelocity / (1 / a.mass +1/ b.mass);
                Vector2 impulseVector = impulse * normal;
                
                a.velocity -= impulseVector + (1/a.mass*impulseVector);

                b.velocity+= impulseVector + (1/b.mass*impulseVector);
                
                a.isSleeping = false;
                b.isSleeping = false;

                a.sleepCounter = 0;
                b.sleepCounter = 0;
            }
        }

    }

    
    public  static void CircleToLineCollision(Circle circle , Vector2 pointA  , Vector2 pointB )
    {

        Vector2 AB = pointB - pointA;
        Vector2 AC = circle.Position - pointA;
        float ab2 = Vector2.Dot(AB , AB);

        if (ab2<= 0.0001) return;
        var t = Vector2.Dot(AC , AB) / Vector2.Dot(AB , AB);
        t = Math.Clamp(t , 0 , 1);
        Vector2 closestPoint = Circle.ClosestDistanceInALine(circle , pointA , pointB);
        
    
        var delta = circle.Position - closestPoint;

        float distanceSq = delta.LengthSquared(); 
        
        if (distanceSq <= circle.radius * circle.radius)
        { 
            var distance = (float)Math.Sqrt(distanceSq) ;
            Vector2 normal;

            if (t <= 0)
                normal = Vector2.Normalize(circle.Position - pointA);
            else if (t >= 1)
                normal = Vector2.Normalize(circle.Position - pointB);
            else
                normal = delta / distance;
           
            var velocityAlongNormal = Vector2.Dot(circle.velocity , normal);
            
            var totalVelocity = -(1 + restitution) * velocityAlongNormal * circle.mass;

            

            float overlap = circle.radius - distance;
            if (overlap > 0)
            {
                var correction = normal * overlap ;

                circle.Position += correction;
            }

            
            
            if (velocityAlongNormal < 0)
            {
                float inverseMassCircle = 1.0f / circle.mass;
                float inverseMassLine = 0.0f; // Static

                float j = -(1 + restitution) * velocityAlongNormal;
                j /= (inverseMassCircle + inverseMassLine);

                // 2. Apply the Impulse Vector to the velocity
                Vector2 impulseVector = j * normal;
                circle.velocity += impulseVector * inverseMassCircle; 
            }

            if (velocityAlongNormal > 0)
            {
                Vector2 tanget = new Vector2(-normal.Y , normal.X);
                var velocityAlongTangent = Vector2.Dot(tanget , circle.velocity);

                var tangentVelocity = tanget * velocityAlongTangent;
                var normalVelocity = normal * velocityAlongNormal;
                tangentVelocity *= 1 - FRICTION;
                circle.velocity = tangentVelocity +  normalVelocity;

            }
        }
    }
    
    public static Vector2 ClosestDistanceInALine(Circle circle , Vector2 pointA , Vector2 pointB)
    {   
        Vector2 AB = pointB - pointA;
        Vector2 AC = circle.Position - pointA;

        var t = Vector2.Dot(AC , AB) / Vector2.Dot(AB , AB);
        t = Math.Clamp(t , 0 , 1);
        var point = pointA + t * AB;
        return point;
    }

    public  bool IsColliding(Circle a, Circle b)
    {
        float dx = b.Position.X - a.Position.X;
        float dy = b.Position.Y - a.Position.Y;
        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

        if (distance < a.radius + b.radius)
        {
            return true;
        }
        else 
            return false;

    }
    public static void ToggleIsSleeping(List<Circle> circles)
    {
        foreach (var circle in circles)
        {
            float speed = circle.velocity.Length();

            if (circle.isSleeping)
            {
                
                circle.sleepCounter +=1;
                if (speed > WAKE_THRESHOLD)
                {
                    circle.isSleeping = false;
                    circle.sleepCounter = 0;
                }

                continue;
            }
            

            if (speed < SLEEP_THRESHOLD )
            {
                circle.sleepCounter +=1;

            }
            else
            {
                circle.sleepCounter =0;
            }
            if (circle.sleepCounter > SLEEP_TIME)
            {
                circle.isSleeping = true;
                circle.velocity = Vector2.Zero;
            }
            
        }
    }
}