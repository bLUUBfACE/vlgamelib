using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Other
{
    public enum Direction
    {
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4,
        None = 0
    }

    public class ScoreManager
    {
        float score;
        public int Score { get { return (int)score; } }
        public float Multiplier { get; private set; }
        TimeSpan multipldur;
        float realScore;
        float scorePerSecond;
        readonly float speed;

        public ScoreManager(float Speed) { this.speed = Speed; Multiplier = 1; }

        public void AddPoints(int Points)
        {
            realScore += (int)((float)Points * Multiplier);
            scorePerSecond = Math.Max((int)((float)(realScore - Score) * speed), 1);
        }

        public void SetMultiplier(float Multiplier, TimeSpan Duration)
        {
            multipldur = Duration;
            this.Multiplier += Multiplier;
        }

        public void Update(GameTime gameTime)
        {
            multipldur.Add(gameTime.ElapsedGameTime.Negate());
            if (multipldur.TotalMilliseconds < 0) Multiplier = 1;
            score = Math.Min(score + (float)scorePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds, (float)realScore);
        }
    }

    public static class PhysicsHelper
    {
        public static class FreeFallCalculator
        {
            public static float TimeToGround(float gravity, float distanceToGround, float startingSpeedY)
            {
                if(startingSpeedY < 0 || distanceToGround < 0 || gravity <= 0) throw new ArgumentException();
                return ((float)Math.Sqrt(2 * gravity * distanceToGround + startingSpeedY * startingSpeedY) - startingSpeedY) / gravity;
            }

            public static float TimeToMaxHeight(float gravity, float startingSpeedY)
            {
                if (startingSpeedY > 0 || gravity <= 0) throw new ArgumentException();
                return startingSpeedY / gravity * -1;
            }

            public static float DistanceToMaxHeight(float gravity, float startingSpeedY)
            {
                if(gravity <= 0) throw new ArgumentException();
                return startingSpeedY > 0 ? 0 : (startingSpeedY * startingSpeedY) / (2 * gravity);
            }
        }
    }
}

namespace GameLibrary.Computing3D
{
    public enum MovingDirection
    {
        Upwards,
        Downwards,
        Leftwards,
        Rightwards,
        Forwards,
        Backwards,
    }
}
