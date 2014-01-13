/* BotBase created by AknA and Wigglez */

/* Notes
 * 
 * http://wowprogramming.com/docs/api_categories#quest
 * 
 * http://wowpedia.org/API_GetQuestsCompleted
 * http://www.wowwiki.com/API_GetContainerItemQuestInfo
 * http://www.wowwiki.com/Events/Quest
 * http://www.wowwiki.com/API_GetQuestLogTitle
 * http://wowprogramming.com/docs/api/IsUnitOnQuest
 * 
 */

using Styx.WoWInternals;

namespace DaVinci.Helpers {
    public class Questing {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

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

        public static bool AllreadyCompleted() {
            return true;
        }

        public static bool Have() {
            return true;
        }

        public static bool Completed() {
            return true;
        }

        // index 1-Number of quests you can have in your questlog, we need a loop to go through them all.
        // No, maybe it checks if the unit has the quest we check from OUR questlog, must be like that.
        // So if we are going to ask for a specific quest then we need to find the index of that quest in our questlog.
        public static bool IsNameOnQuest(int index, string name) {
            var returnValue = Lua.GetReturnVal<int>(string.Format("return IsUnitOnQuest({0}, '{1}')", index, name), 0);
            DaVinci.CustomDiagnosticLog("returnValue = {0}", returnValue);
            return returnValue == 1;
        }

        /*
         * unitID :
         * 
         * player - The player him/herself
         * party1 to party4 - Another member of the player's party. Indices match the order party member frames are displayed in the default UI 
         * (party1 is at the top, party4 at the bottom), but not consistent among party members (i.e. if Thrall and Cairne are in the same party, 
         * the player Thrall sees as party2 may not be the same player Cairne sees as party2).
        */
        public static bool IsUnitOnQuest(int index, string unitID) {
            var returnValue = Lua.GetReturnVal<int>(string.Format("return IsUnitOnQuest({0}, '{1}')", index, unitID), 0);
            DaVinci.CustomDiagnosticLog("returnValue = {0}", returnValue);
            return returnValue == 1;
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}