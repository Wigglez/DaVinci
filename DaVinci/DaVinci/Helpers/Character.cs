using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.CommonBot;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace DaVinci.Helpers {
    public class Character {
        // ===========================================================
        // Constants
        // ===========================================================

        public const int AllianceTundraMammothNPC = 32633;
        public const int HordeTundraMammothNPC = 32640;
        public const int ExpeditionYakNPC = 62809;

        public const int AllianceTundraMammothSpell = 61425;
        public const int HordeTundraMammothSpell = 61447;
        public const int ExpeditionYakSpell = 122708;

        // ===========================================================
        // Fields
        // ===========================================================

        public static WoWPoint StartLocation = WoWPoint.Empty;
        public static int StartLocationIndex = 0;

        public static WoWPoint StuckOldLocation = WoWPoint.Empty;
        public static WoWPoint StartStuckOldLocation = WoWPoint.Empty;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void IncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits) {
            if(Me.GotTarget && Me.CurrentTarget.Attackable) {
                outgoingUnits.Add(Me.CurrentTarget);
            }
        }

        public static void CastHearthstone() {
            if(IsCastingOrChanneling()) {
                return;
            }

            var hearthstone = ObjectManager.GetObjectsOfTypeFast<WoWItem>().FirstOrDefault(u => u.Entry == 6948 || u.Entry == 64488);

            if(hearthstone == null) {
                DaVinci.CustomNormalLog("You don't have a hearthstone.");
                return;
            }

            if(hearthstone.Cooldown <= 0) {
                hearthstone.Use();
            } else {
                PriorityTreeState.IsDoneHearthing = true;
            }
        }

        public static bool IsCastingOrChanneling() { return Me.IsCasting || Me.IsChanneling; }

        public static void StuckHandler() {
            if(Me.Combat) {
                PriorityTreeState.StuckTimer.Reset();
                return;
            }

            if(StuckOldLocation == WoWPoint.Empty) {
                StuckOldLocation = Me.Location;
                return;
            }

            if(StuckOldLocation.Distance(Me.Location) > 3) {
                StuckOldLocation = Me.Location;
                PriorityTreeState.StuckTimer.Reset();
                return;
            }

            if(!PriorityTreeState.StuckTimer.IsRunning) {
                PriorityTreeState.StuckTimer.Start();
                return;
            }

            if(PriorityTreeState.StuckTimer.ElapsedMilliseconds < 4000) {
                return;
            }

            GetUnstuck();
            PriorityTreeState.StuckTimer.Reset();
        }

        public static void GetUnstuck() {
            PriorityTreeState.StuckTimer.Reset();

            //PickPocketTarget.AddToStuckBlacklist();

            Enemy.ClearAllTargetData();

            WoWPoint myNewWoWPoint;

            do {
                float x, y, z;
                do {
                    x = Me.Location.X + RandomNumber.GenerateRandomFloat(-20, 20);
                    y = Me.Location.Y + RandomNumber.GenerateRandomFloat(-20, 20);
                } while(!Navigator.FindHeight(x, y, out z));
                myNewWoWPoint = new WoWPoint(x, y, z);
            } while(!Navigator.CanNavigateFully(Me.Location, myNewWoWPoint) && myNewWoWPoint.Distance(Me.Location) < 5);

            DaVinci.CustomDiagnosticLog("Unstuck: Moving to " + myNewWoWPoint);
            Navigator.MoveTo(myNewWoWPoint);
        }

        public static void VanishPoint() {
            WoWPoint myNewWoWPoint;

            do {
                float x, y, z;
                do {
                    x = Me.Location.X + RandomNumber.GenerateRandomFloat(-20, 20);
                    y = Me.Location.Y + RandomNumber.GenerateRandomFloat(-20, 20);
                } while(!Navigator.FindHeight(x, y, out z));

                myNewWoWPoint = new WoWPoint(x, y, z);
            } while(!Navigator.CanNavigateFully(Me.Location, myNewWoWPoint) && myNewWoWPoint.Distance(Me.Location) < 5);

            DaVinci.CustomDiagnosticLog("Vanish: Moving to " + myNewWoWPoint + " for safety.");
            Navigator.MoveTo(myNewWoWPoint);
        }

        /*
        public static void NavigateToPickPocketTarget() {
            if(!PickPocketTarget.IsValid()) {
                return;
            }

            // If we can navigate to the target, calculate how to get to it

            if(PickPocketTarget.Target.IsMoving && Me.IsBehind(PickPocketTarget.Target)) {
                PickPocketTarget.TargetLocation = WoWMovement.CalculatePointFrom(PickPocketTarget.Target.Location, 6f);
            } else {
                PickPocketTarget.TargetLocation = WoWMovement.CalculatePointFrom(PickPocketTarget.Target.Location, 8f);
            }

            // Move to the target's exact location
            if(PickPocketTarget.Target.IsAlive) {
                Navigator.MoveTo(PickPocketTarget.TargetLocation);
            } else {
                PickPocketTarget.AddToPickPocketedBlacklist();
            }
        }
         */

        public static bool HasFullInventory() {
            return Me.FreeBagSlots <= 5; // DaVinciSettings.Instance.InventoryFull (fix later)
        }

        /*
        public static bool CanNavigateToPickPocketTarget() {
            if(!PickPocketTarget.IsValid()) {
                return false;
            }

            // If we can navigate to the target
            return Navigator.CanNavigateFully(Me.Location, PickPocketTarget.Target.Location);
        }
        */

        /*
        public static bool IsInPickPocketRange() {
            if(!PickPocketTarget.IsValid()) {
                return false;
            }

            return PickPocketTarget.Target.Location.Distance(Me.Location) <= 9f;
        }
        */

        public static bool IsReadyToScan() {
            // Not in proximity
            // Not on the same quest

            return true;
        }

        public static bool IsOnVendorMount() { return Me.HasAura(AllianceTundraMammothSpell) || Me.HasAura(HordeTundraMammothSpell) || Me.HasAura(ExpeditionYakSpell); }

        public static Mount.MountWrapper CheckVendorMount() { return Me.IsAlliance ? Mount.GroundMounts.FirstOrDefault(mount => mount.CreatureId == AllianceTundraMammothNPC || mount.CreatureId == ExpeditionYakNPC) : Mount.GroundMounts.FirstOrDefault(mount => mount.CreatureId == HordeTundraMammothNPC || mount.CreatureId == ExpeditionYakNPC); }


        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}