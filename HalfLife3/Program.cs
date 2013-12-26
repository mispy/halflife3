using System;

namespace HalfLife3
{
#if WINDOWS || XBOX
    static class Program
    {
        public static Game1 Game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game = new Game1();
            using (Game)
            {
                Game.Run();
            }
        }
    }
#endif
}

