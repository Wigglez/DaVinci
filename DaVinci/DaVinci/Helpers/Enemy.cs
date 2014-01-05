using Styx.WoWInternals;

namespace DaVinci.Helpers {

    public class Enemy {
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

        public static void ClearAllTargetData() {
            /*
            SapTarget.Target = null;
            PickPocketTarget.Target = null;
            PickPocketTarget.TargetLocation = WoWPoint.Empty;
             */


        }

        public static void HandleErrorMessage(object sender, LuaEventArgs args) {
            var errorMessage = args.Args[0].ToString();

            //var localizedAlreadyPickpocketed = Lua.GetReturnVal<string>("return ERR_ALREADY_PICKPOCKETED", 0);

            /*
            if(!errorMessage.Equals(localizedAlreadyPickpocketed)) {
                return;
            }
            */

            /*
            if(errorMessage.Equals(localizedNoPockets)) {
                var noPocketMob = new Blacklist.BlacklistEntry { Entry = PickPocketTarget.Target.Entry, GUID = PickPocketTarget.Target.Guid, Name = PickPocketTarget.Target.Name, Elite = PickPocketTarget.Target.Elite, Comment = "No pockets (auto added)." };
                BlacklistSettings.Instance.BlacklistedUnits.Add(noPocketMob);
                BlacklistSettings.Save();
            } else {
                PickPocketTarget.AddToPickPocketedBlacklist();
            }

            ClearAllTargetData();
             */


        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}