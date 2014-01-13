/* BotBase created by AknA and Wigglez */

using System;

namespace DaVinci.Helpers {
    public class RandomNumber {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static readonly Random Rand = new Random();

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        /*
	     * Returns a psuedo-random number between min and max, inclusive. The
	     * difference between min and max can be at most
	     * <code>Integer.MAX_VALUE - 1</code>.
	     */
        public static int GenerateRandomInt(int pMinVal, int pMaxVal) {
            // nextInt is normally exclusive of the top value (pMaxVal), so add 1 to
            // make it inclusive

            var randomNumber = Rand.Next((pMaxVal - pMinVal) + 1) + pMinVal;

            return randomNumber;
        }

        public static float GenerateRandomFloat(float pMinVal, float pMaxVal) {
            // nextfloat is normally exclusive of the top value (pMaxVal), so add 1
            // to
            // make it inclusive

            var rndFlt = GenerateRandomInt((int)(pMinVal*100), (int)(pMaxVal*100));

            var randomNumber = rndFlt / 100.0f;

            return randomNumber;
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}