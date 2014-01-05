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

using System;
using System.Windows.Forms;
using System.Windows.Media;
using Bots.Grind;
using DaVinci.Helpers;
using CommonBehaviors.Actions;
using DaVinci.GUI;
using Levelbot.Actions.Death;
using Levelbot.Decorators.Death;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Profiles;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;

namespace DaVinci {
    public class DaVinci : BotBase {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static Composite _root;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        public override string Name {
            get {
                return "DaVinci";
            }
        }

        public override Composite Root {
            get {
                return _root ?? (_root = CreateRoot());
            }
        }

        public override PulseFlags PulseFlags {
            get {
                return PulseFlags.All;
            }
        }

        public override Form ConfigurationForm {
            get {
                var gui = new DaVinciGUI();

                gui.Activate();

                return gui;
            }
        }

        public override bool IsPrimaryType {
            get {
                return base.IsPrimaryType;
            }
        }

        public override bool RequirementsMet {
            get {
                return base.RequirementsMet;
            }
        }

        public override void Start() {
            try {
                ProfileManager.LoadEmpty();

                Targeting.Instance.IncludeTargetsFilter += Character.IncludeTargetsFilter;

                CustomBlacklist.SweepTimer();

                Lua.Events.AttachEvent("UI_ERROR_MESSAGE", Enemy.HandleErrorMessage);
                
                PriorityTreeState.TreeState = PriorityTreeState.State.REQUIREMENTS;
            } catch(Exception e) {
                CustomNormalLog("Could not initialize. Message = " + e.Message + " Stacktrace = " + e.StackTrace);
            } finally {
                CustomNormalLog("Initialization complete.");
            }
        }

        public override void Stop() {
            try {
                Character.StartLocation = WoWPoint.Empty;

                Enemy.ClearAllTargetData();

                CustomBlacklist.RemoveAllGUID();
                CustomBlacklist.RemoveAllEntries();

                Targeting.Instance.IncludeTargetsFilter -= Character.IncludeTargetsFilter;

                Lua.Events.DetachEvent("UI_ERROR_MESSAGE", Enemy.HandleErrorMessage);
            } catch(Exception e) {
                CustomNormalLog("Could not dispose. Message = " + e.Message + " Stacktrace = " + e.StackTrace);
            } finally {
                CustomNormalLog("Shutdown complete.");
            }

        }

        // ===========================================================
        // Methods
        // ===========================================================

        public static void CustomNormalLog(string message, params object[] args) {
            Logging.Write(Colors.DeepSkyBlue, "[DaVinci]: " + message, args);
        }

        public static void CustomDiagnosticLog(string message, params object[] args) {
            Logging.WriteDiagnostic(Colors.DeepSkyBlue, "[DaVinci]: " + message, args);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static Composite CreateRoot() {
            return new PrioritySelector(
                UsePreCombatRoutine(),
                UseCombatRoutine(),
                UseDeathRoutine(),
                PriorityTree()
            );
        }

        private static Composite UsePreCombatRoutine() {
            return new Decorator(ret => !Character.Me.Combat,
                new PrioritySelector(
                    RoutineManager.Current.RestBehavior ?? new ActionAlwaysFail(),
                    RoutineManager.Current.PreCombatBuffBehavior ?? new ActionAlwaysFail()
                )
            );
        }

        private static Composite UseCombatRoutine() {
            return new Decorator(ret => !Character.Me.Mounted && Character.Me.IsActuallyInCombat,
                new PrioritySelector(
                    RoutineManager.Current.HealBehavior ?? new ActionAlwaysFail(),
                    RoutineManager.Current.CombatBuffBehavior ?? new ActionAlwaysFail(),
                    RoutineManager.Current.CombatBehavior ?? new ActionAlwaysFail()
                )
            );
        }

        private static Composite UseDeathRoutine() {
            return new Decorator(ret => !Character.Me.IsAlive,
                new PrioritySelector(
                    new DecoratorNeedToRelease(new ActionReleaseFromCorpse()),
                    new DecoratorNeedToMoveToCorpse(LevelBot.CreateDeathBehavior()),
                    new DecoratorNeedToTakeCorpse(LevelBot.CreateDeathBehavior()),
                    new ActionSuceedIfDeadOrGhost()
                )
            );
        }

        private static Composite PriorityTree() {
            return new Decorator(ret => !Character.Me.Combat,
                new Action(context => PriorityTreeState.TreeStateHandler())
            );
        }
    }
}
