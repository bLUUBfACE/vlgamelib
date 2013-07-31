using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLibrary.Other
{
    public class ColorEngine
    {
        enum EngineMode { Merging, Static, }

        public Color Color { get { return pausePredicate(this) ? currentColor : ForceUpdate(); } private set { } }
        public bool Pause = false;
        private Predicate<ColorEngine> pausePredicate;

        Color currentColor;
        List<Color> Colors;
        EngineMode mode;
        float speed;
        DateTime lastUpdate = DateTime.Now;

        #region Merging
        int curindex;
        float percent;
        bool loop;
        #endregion


        public static Color GenerateColorByRandom(Random random)
        {
            return new Color(random.Next(255), random.Next(255), random.Next(255));
        }
        public static Color MergeColor(Color color1, Color color2, float percent)
        {
            return new Color(color1.R - (int)((float)(color1.R - color2.R) / 100f * percent),
                color1.G - (int)((float)(color1.G - color2.G) / 100f * percent),
                color1.B - (int)((float)(color1.B - color2.B) / 100f * percent),
                color1.A - (int)((float)(color1.A - color2.A) / 100f * percent));
        }


        public ColorEngine() { this.pausePredicate = c => c.Pause; }
        public ColorEngine(Predicate<ColorEngine> PauseChecker) { this.pausePredicate = PauseChecker; }

        public void StartMerging(Color c1, Color c2, float Speed, bool loop)
        {
            StartMerging(new List<Color>() { c1, c2 }, Speed, loop);
        }
        public void StartMerging(List<Color> Colors, float Speed, bool loop)
        {
            this.Colors = Colors;
            curindex = 0;
            this.speed = Speed;
            mode = EngineMode.Merging;
            this.loop = loop;
        }

        public void ShowStaticColor(Color c)
        {
            Colors = new List<Color>() { c };
            curindex = 0;
            mode = EngineMode.Static;
        }

        public bool SelectNextColor()
        {
            percent = 0;
            if (++curindex == Colors.Count)
                if (loop) { curindex = 0; return true; }
                else return false;
            else return true;
        }
        public Color ForceUpdate()
        {
            float diff = speed * (float)(DateTime.Now - lastUpdate).TotalSeconds + percent; percent = 0;
            lastUpdate = DateTime.Now;
            switch (mode)
            {
                case EngineMode.Merging:
                    while (diff >= 100)
                    {
                        if (!SelectNextColor()) goto case EngineMode.Static; diff -= 100;
                    }
                    percent = diff;
                    currentColor = MergeColor(Colors[curindex], Colors[curindex == Colors.Count - 1 ? 0 : curindex + 1], percent);
                    return currentColor;
                case EngineMode.Static:
                    return currentColor = Colors[curindex];
            }
            return currentColor;
        }
    }
}
