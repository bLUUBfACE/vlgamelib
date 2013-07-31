using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Computing2D
{
    public interface IPathable
    {
        Vector2 MoveNext(float distance);
        Vector2 MoveNext(float distance, out bool isFinished, out float surplus);
        Vector2 Current();
    }
}
