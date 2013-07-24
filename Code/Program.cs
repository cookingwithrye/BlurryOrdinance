using System;

namespace OSBO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}
            
            ////using (Game3Physicstesting game = new Game3Physicstesting())
            //using (Game4AnimationTest game = new Game4AnimationTest())
            //{
            //    game.Run();
            //}
                using (Game2 game = new Game2())
                {
                    game.Run();
                }
        }
    }
}

