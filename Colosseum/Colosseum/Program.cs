using System;

namespace Colosseum
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ColosseumGame game = new ColosseumGame())
            {
                game.Run();
            }
        }
    }
#endif
}

