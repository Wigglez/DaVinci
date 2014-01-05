using System.Diagnostics;
using System.Threading;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Frames;
using Styx.Helpers;
using Styx.Pathing;
using Styx.WoWInternals;

namespace DaVinci.Helpers {
    public class PriorityTreeState {
        // ===========================================================
        // Constants
        // ===========================================================

        public enum State {
            REQUIREMENTS,
            
        };

        // ===========================================================
        // Fields
        // ===========================================================
        
        public static State TreeState = State.REQUIREMENTS;

        public static WoWPoint TargetLocation = WoWPoint.Empty;

        public static Stopwatch PulseStopwatch = new Stopwatch();
        public static readonly Stopwatch StuckTimer = new Stopwatch();


        //private static bool _isDoneBagHandling;
        //private static bool _isDoneVendoring;
        public static bool IsDoneHearthing;

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

        public static void TreeStateHandler() {
            switch(TreeState) {
                case State.REQUIREMENTS:
                    GlobalSettings.Instance.UseFrameLock = true;
                    GlobalSettings.Instance.TicksPerSecond = 35;

                    

                    break;

                    /*
                case State.PICKPOCKET:
                    IsDoneHearthing = false;

                    if(CharacterSettings.Instance.UseMount) {
                        CharacterSettings.Instance.UseMount = false;
                    }

                    if(DaVinciSettings.Instance.UseMobileBanking) {
                        if(Character.Me.Gold > DaVinciSettings.Instance.GoldDepositThreshold && (MobileBanking.MobileBankExists() || MobileBanking.CanCastMobileBanking())) {
                            DaVinci.CustomNormalLog("We have reached the gold threshold for mobile banking.");

                            TreeState = State.INVENTORY_CHECK;
                            return;
                        }
                    }

                    if(Character.HasFullInventory() || Character.Me.LowestDurabilityPercent <= .1) {
                        TreeState = State.INVENTORY_CHECK;
                        return;
                    }

                    if(!Character.IsReadyToScan()) {
                        return;
                    }

                    if(BotStartStuckTimer.IsRunning) {
                        if(BotStartStuckTimer.ElapsedMilliseconds < 120000) {
                            if(Character.StartStuckhandler()) {
                                DaVinci.CustomNormalLog("You got stuck for 20 seconds within 2 minutes of starting the bot.");
                                DaVinci.CustomNormalLog("This is not a good farming area, restart somewhere else.");
                                TreeRoot.Stop();
                            }
                        } else {
                            BotStartStuckTimer.Stop();
                        }
                    }

                    if(Character.Me.Location.Distance(Character.StartLocation) > 4) {
                        Character.StuckHandler();
                    }

                    Character.SapHandler();

                    // ===================================
                    // Navigation handling
                    // ===================================

                    if(PickPocketTarget.Target == null) {
                        PickPocketTarget.GetTarget();

                        if(PickPocketTarget.Target != null) {
                            if(PickPocketTarget.Target.IsValid) {

                                return;
                            }
                            WoWMovement.MoveStop();
                            DaVinci.CustomNormalLog("Blacklisted target due to being invalid (target == null area).");
                            PickPocketTarget.AddToPickPocketedBlacklist();
                            return;
                        }

                        if(!Character.IsAtStartLocation()) {
                            TreeState = State.RETURN_TO_START;
                        }
                    } else {

                        if(!PickPocketTarget.Target.IsValid) {
                            Enemy.ClearAllTargetData();
                            return;
                        }

                        if(!Character.CanNavigateToPickPocketTarget()) {
                            DaVinci.CustomDiagnosticLog("Blacklisting due to unnavigatable target.");
                            PickPocketTarget.AddToPickPocketedBlacklist();
                            return;
                        }

                        if(!Character.IsInPickPocketRange()) {
                            Character.NavigateToPickPocketTarget();
                        } else {
                            if(PickPocketTarget.InRangeValidate()) {
                                if(!PickPocketTimer.IsRunning) {
                                    Character.CastPickPocket();

                                    PickPocketTimer.Start();
                                } else {
                                    if(PickPocketTimer.ElapsedMilliseconds >= 700) {
                                        PickPocketTimer.Reset();
                                    }
                                }
                            } else {
                                DaVinci.CustomDiagnosticLog("Blacklisting due to line of sight or invalid.");
                                PickPocketTarget.AddToPickPocketedBlacklist();
                            }
                        }
                    }

                    break;

                case State.INVENTORY_CHECK:
                    if(!Character.IsAtStartLocation()) {
                        TreeState = State.RETURN_TO_START;
                    } else {
                        if(Character.Me.LowestDurabilityPercent <= .1) {
                            DaVinci.CustomNormalLog("Durability is too low to proceed, vendoring.");
                            TreeState = State.VENDORING;
                            return;
                        }

                        if(Character.HasFullInventory()) {
                            DaVinci.CustomNormalLog("Handling full inventory.");
                            TreeState = State.FULL_INVENTORY;
                            return;
                        }

                        if(DaVinciSettings.Instance.UseMobileBanking && Character.Me.Gold > DaVinciSettings.Instance.GoldDepositThreshold && (MobileBanking.MobileBankExists() || MobileBanking.CanCastMobileBanking())) {
                            DaVinci.CustomNormalLog("We are at the start location and are about to deposit gold.");
                            TreeState = State.MOBILE_BANKING;
                            return;
                        }

                        PickPocketTarget.GetTarget();

                        if(PickPocketTarget.Target == null) {
                            if(StartingLocationSettings.Instance.UseStartingLocationList) {
                                if(!StartingLocationSettings.Instance.RandomizeLocationList) {
                                    if(Character.StartLocationIndex < Character.EnabledStartingLocations.Length - 1) {
                                        Character.StartLocationIndex++;
                                    } else {
                                        Character.StartLocationIndex = 0;
                                    }
                                } else {
                                    if(Character.EnabledStartingLocations.Length > 1) {
                                        var oldStartLocationIndex = Character.StartLocationIndex;
                                        var newStartLocationIndex = RandomNumber.GenerateRandomInt(0, Character.EnabledStartingLocations.Length - 1);

                                        do {
                                            if(oldStartLocationIndex != newStartLocationIndex) {
                                                Character.StartLocationIndex = newStartLocationIndex;
                                            } else {
                                                newStartLocationIndex = RandomNumber.GenerateRandomInt(0, Character.EnabledStartingLocations.Length - 1);
                                            }
                                        } while(oldStartLocationIndex == newStartLocationIndex);
                                    }
                                }

                                Character.StartLocation = Character.EnabledStartingLocations[Character.StartLocationIndex].LocationAsWoWPoint;

                                TreeState = State.RETURN_TO_START;
                            }
                        } else {
                            TreeState = State.PICKPOCKET;
                        }
                    }

                    break;


                case State.MOBILE_BANKING:
                    if(Character.Me.Gold > DaVinciSettings.Instance.GoldDepositThreshold) {
                        if(!MobileBanking.MobileBankExists()) {
                            if(Character.IsCastingOrChanneling()) {
                                return;
                            }

                            MobileBanking.CastMobileBanking();
                            DaVinci.CustomNormalLog("Casting Mobile Bank.");
                            Thread.Sleep(500);
                        } else {
                            if(!MobileBanking.MobileBank.WithinInteractRange) {
                                var mobileBankLocation = WoWMovement.CalculatePointFrom(MobileBanking.MobileBank.Location, 5f);
                                Navigator.MoveTo(mobileBankLocation);
                            } else {
                                if(!MobileBanking.IsGuildBankVisible()) {
                                    MobileBanking.MobileBank.Interact();
                                } else {
                                    var depositGoldAmount = Character.Me.Gold - DaVinciSettings.Instance.AllGoldExceptDepositAmount;

                                    if(Character.Me.Gold <= DaVinciSettings.Instance.AllGoldExceptDepositAmount || depositGoldAmount <= 0) {
                                        return;
                                    }

                                    MobileBanking.DepositGuildBankMoney(depositGoldAmount);
                                    DaVinci.CustomNormalLog("Deposited " + depositGoldAmount + " gold.");
                                    Thread.Sleep(5000);
                                }
                            }
                        }

                        return;
                    }

                    DaVinci.CustomNormalLog("Banking is done.");
                    TreeState = State.PICKPOCKET;

                    break;

                case State.FULL_INVENTORY:
                    _isDoneBagHandling = false;

                    if(DaVinciSettings.Instance.UnlockLockboxCheckBox && LockboxHandler.FindLockedBox() != null) {
                        if(LockboxHandler.LockboxTimer.IsRunning && LockboxHandler.LockboxTimer.ElapsedMilliseconds > 2000) {
                            LockboxHandler.LockboxTimer.Reset();
                        }

                        LockboxHandler.PickLock();
                    }
                    if(Character.Me.FreeBagSlots > 1) {
                        if(DaVinciSettings.Instance.OpenLockboxCheckbox && LockboxHandler.FindLockedBox() == null && LockboxHandler.FindUnlockedBox() != null) {

                            if(LockboxHandler.LockboxTimer.IsRunning && LockboxHandler.LockboxTimer.ElapsedMilliseconds > 2000) {
                                LockboxHandler.LockboxTimer.Reset();
                            }

                            LockboxHandler.OpenLockbox();
                        }
                    } else {
                        _isDoneBagHandling = true;
                    }

                    if((LockboxHandler.FindLockedBox() == null && LockboxHandler.FindUnlockedBox() == null)) {
                        _isDoneBagHandling = true;
                    }

                    if(!DaVinciSettings.Instance.OpenLockboxCheckbox) {
                        _isDoneBagHandling = true;
                    }

                    if(_isDoneBagHandling) {
                        if(Character.HasFullInventory()) {
                            DaVinci.CustomNormalLog("Vendoring.");
                            TreeState = State.VENDORING;
                        } else {
                            DaVinci.CustomNormalLog("Vendoring is not needed, returning to pick pocket.");
                            TreeState = State.PICKPOCKET;
                        }
                    }

                    break;

                case State.VENDORING:
                    _isDoneVendoring = !Character.HasFullInventory() && Character.Me.LowestDurabilityPercent > .1;

                    DaVinci.CustomDiagnosticLog("Done vendoring = " + _isDoneVendoring);

                    if(_isDoneVendoring) {
                        if(MerchantFrame.Instance.IsVisible) {
                            MerchantFrame.Instance.Close();
                        }

                        if(Character.IsAtStartLocation()) {
                            if(Flightor.MountHelper.Mounted) {
                                Flightor.MountHelper.Dismount();
                            }

                            TreeState = State.PICKPOCKET;
                        } else {
                            if(Character.Me.IsIndoors) {
                                Navigator.MoveTo(Character.StartLocation);
                            } else {
                                Flightor.MoveTo(Character.StartLocation, true);
                            }
                        }
                    } else {
                        // Figure out how to get to the repair merchant
                        if(Character.CheckVendorMount() != null) {
                            if(!Character.CheckVendorMount().CanMount) {
                                return;
                            }

                            if(!Character.IsOnVendorMount()) {
                                Character.CheckVendorMount().CreatureSpell.Cast();
                                Thread.Sleep(3000);
                            } else {
                                if(!MerchantFrame.Instance.IsVisible) {
                                    RepairVendor.Near = null;
                                    RepairVendor.GetNear();

                                    if(RepairVendor.Near != null) {
                                        RepairVendor.Near.Interact();
                                    }
                                } else {
                                    RepairVendor.VendorAction();
                                }
                            }
                        } else {
                            if(DaVinciSettings.Instance.UseHearthstoneCheckBox) {
                                if(!IsDoneHearthing) {
                                    WoWMovement.MoveStop();
                                    Character.CastHearthstone();
                                    Thread.Sleep(15000);
                                } else {
                                    if(!Character.Me.IsValid) {
                                        return;
                                    }

                                    // Get a valid vendor
                                    RepairVendor.GetNear();
                                    RepairVendor.GetFar();

                                    // Prefer the near over the far
                                    if(RepairVendor.Near != null) {
                                        RepairVendor.ClearFar();
                                    }

                                    if(!RepairVendor.IsFound()) {
                                        return;
                                    }

                                    RepairVendor.CheckDistanceForMount();

                                    // Move to the valid vendor
                                    RepairVendor.MoveToNear();

                                    RepairVendor.MoveToFar();

                                    RepairVendor.MerchantHandler();
                                }
                            } else {
                                // Get a valid vendor
                                RepairVendor.GetNear();
                                RepairVendor.GetFar();

                                // Prefer the near over the far
                                if(RepairVendor.Near != null) {
                                    RepairVendor.ClearFar();
                                }

                                if(!RepairVendor.IsFound()) {
                                    return;
                                }

                                RepairVendor.CheckDistanceForMount();

                                // Move to the valid vendor
                                RepairVendor.MoveToNear();

                                RepairVendor.MoveToFar();

                                RepairVendor.MerchantHandler();
                            }
                        }
                   }

                    break;

                case State.RETURN_TO_START:
                    if(Character.StartLocation.Distance(Character.Me.Location) > 3) {
                        TreeState = !StartingLocationSettings.Instance.FlyToStartingLocations ? State.RETURN_TO_START_NAVIGATOR : State.RETURN_TO_START_FLIGHTOR;
                    } else {
                        if(Flightor.MountHelper.Mounted) {
                            Flightor.MountHelper.Dismount();
                        } else {
                            Enemy.ClearAllTargetData();

                            TreeState = State.INVENTORY_CHECK;
                        }
                    }

                    break;

                case State.RETURN_TO_START_NAVIGATOR:
                    CharacterSettings.Instance.UseMount = false;

                    if(Character.StartLocation.Distance(Character.Me.Location) <= 3) {
                        TreeState = State.RETURN_TO_START;
                    }

                    if(Character.StartLocation.Distance(Character.Me.Location) > 700) {
                        if(!Flightor.MountHelper.Mounted) {
                            Flightor.MountHelper.MountUp();
                        } else {
                            if(StartingLocationSettings.Instance.UseStartingLocationList) {
                                var movingToLocationInfo = Character.EnabledStartingLocations[Character.StartLocationIndex];

                                if(movingToLocationInfo.Continent != Character.Me.CurrentMap.Name) {
                                    DaVinci.CustomNormalLog("Cannot navigate to requested continent: " + movingToLocationInfo.Continent);
                                    TreeRoot.Stop("See above message for details.");
                                } else {
                                    DaVinci.CustomNormalLog("Moving to " + movingToLocationInfo.SubZone + " in " + movingToLocationInfo.Zone + " in " + movingToLocationInfo.Continent + " " + movingToLocationInfo.LocationAsWoWPoint);
                                }
                            }

                            Flightor.MoveTo(Character.StartLocation, true);
                        }
                        return;
                    }

                    if(!Navigator.CanNavigateFully(Character.Me.Location, Character.StartLocation)) {
                        DaVinci.CustomNormalLog("Can't navigate to starting location. Flying to it.");
                        if(!Flightor.MountHelper.Mounted) {
                            Flightor.MountHelper.MountUp();
                        }

                        Flightor.MoveTo(Character.StartLocation, true);

                    } else {
                        if(Flightor.MountHelper.Mounted) {
                            Flightor.MoveTo(Character.StartLocation, true);
                        } else {
                            if(!Character.IsStealthed()) {
                                Character.CastStealth();
                            }

                            if(!Character.HasFullInventory() && Character.Me.LowestDurabilityPercent > .1) {
                                PickPocketTarget.GetTarget();

                                if(PickPocketTarget.Target != null) {
                                    PickPocketTarget.BlacklistCount++;
                                    DaVinci.CustomDiagnosticLog("BlacklistCount = " + PickPocketTarget.BlacklistCount);

                                    if(PickPocketTarget.BlacklistCount <= 2) {
                                        TreeState = State.PICKPOCKET;
                                    } else {
                                        PickPocketTarget.AddToPickPocketedBlacklist();
                                    }
                                }
                            }

                            Character.SapHandler();

                            Navigator.MoveTo(Character.StartLocation);
                        }
                    }

                    break;

                case State.RETURN_TO_START_FLIGHTOR:
                    if(Character.StartLocation.Distance(Character.Me.Location) <= 3) {
                        TreeState = State.RETURN_TO_START;
                    }

                    if(StartingLocationSettings.Instance.FlyToStartingLocations) {
                        if(!Flightor.MountHelper.Mounted) {
                            Flightor.MountHelper.MountUp();
                        } else {
                            if(StartingLocationSettings.Instance.UseStartingLocationList) {
                                var movingToLocationInfo = Character.EnabledStartingLocations[Character.StartLocationIndex];

                                if(movingToLocationInfo.Continent != Character.Me.CurrentMap.Name) {
                                    DaVinci.CustomNormalLog("Cannot navigate to requested continent: " + movingToLocationInfo.Continent);
                                    TreeRoot.Stop("See above message for details.");
                                } else {
                                    DaVinci.CustomNormalLog("Moving to " + movingToLocationInfo.SubZone + " in " + movingToLocationInfo.Zone + " in " + movingToLocationInfo.Continent + " " + movingToLocationInfo.LocationAsWoWPoint);
                                }
                            }

                            Flightor.MoveTo(Character.StartLocation, true);
                        }
                        return;
                    }

                    if(Character.StartLocation.Distance(Character.Me.Location) > 700) {
                        if(!Flightor.MountHelper.Mounted) {
                            Flightor.MountHelper.MountUp();
                        } else {
                            if(StartingLocationSettings.Instance.UseStartingLocationList) {
                                var movingToLocationInfo = Character.EnabledStartingLocations[Character.StartLocationIndex];

                                if(movingToLocationInfo.Continent != Character.Me.CurrentMap.Name) {
                                    DaVinci.CustomNormalLog("Cannot navigate to requested continent: " + movingToLocationInfo.Continent);
                                    TreeRoot.Stop("See above message for details.");
                                } else {
                                    DaVinci.CustomNormalLog("Moving to " + movingToLocationInfo.SubZone + " in " + movingToLocationInfo.Zone + " in " + movingToLocationInfo.Continent + " " + movingToLocationInfo.LocationAsWoWPoint);
                                }
                            }

                            Flightor.MoveTo(Character.StartLocation, true);
                        }
                    } else {
                        TreeState = State.RETURN_TO_START_NAVIGATOR;
                    }

                    break;
                     */
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}