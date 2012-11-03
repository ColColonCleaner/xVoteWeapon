/* 
    xVoteWeapon by ColColonCleaner
*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Web;
using System.Data;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Reflection;

using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Plugin.Commands;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;
using PRoCon.Core.Battlemap;
using PRoCon.Core.Maps;


namespace PRoConEvents
{

    //Aliases
    using EventType = PRoCon.Core.Events.EventType;
    using CapturableEvent = PRoCon.Core.Events.CapturableEvents;

    public class xVoteWeapon : PRoConPluginAPI, IPRoConPluginInterface
    {

        #region Variables
        /* Inherited:
            this.PunkbusterPlayerInfoList = new Dictionary<String, CPunkbusterInfo>();
            this.FrostbitePlayerInfoList = new Dictionary<String, CPlayerInfo>();
        */
        private bool fIsEnabled;
        private int fDebugLevel;
        private String pluginVersion = "0.4";
        //the list of available weapons
        private List<Weapon> weaponList;
        //the dictionary of available weapons (quick access by string key)
        private Dictionary<String, Weapon> weaponDictionary;
        //the votes given to each weapon in the next round
        private Dictionary<String, Weapon> weaponVotes;
        //the current allowed weapon
        private Weapon currentWeapon;
        //the weapon to be used next round
        private Weapon nextWeapon;
        //whether players have started voting
        private bool votingStarted = false;
        //millisecond delay before killing people for infractions
        Int32 kill_delay = 2000;
        // If 'yes' then players on procon 'Reserved' 
        // list have immunity from kick/kill
        private Dictionary<String, CPlayerInfo> currentPlayers = null;
        private enumBoolYesNo protect_reservedslot_players = enumBoolYesNo.Yes;
        private Int32 infractionsBeforeKick = 4;
        private Int32 infractionsBeforeTBan = 7;
        private double CQTicketCountModifier = 0.67;
        private Int32 TicketsPerPlayer = 25;
        #endregion

        #region init
        public xVoteWeapon()
        {
            //standard
            fIsEnabled = false;
            fDebugLevel = 2;
            //initialize the weapon list
            generateWeapons();
        }
        #endregion

        #region VoteWeaponMethods
        /**
         * Generates and returns the list of weapons available for usage
         * @return ArrayList containing Weapon objects
         **/
        private void generateWeapons()
        {
            //create the list of weapons, mainly used for random decisions if nobody votes
            List<Weapon> tempWeapons = new List<Weapon>();
            tempWeapons.Add(new Weapon("870 Combat Shotgun", "870", "870MCS", "Shotgun"));
            tempWeapons.Add(new Weapon("AEK-971 Assault Rifle", "AEK", "AEK-971", "AssaultRifle"));
            tempWeapons.Add(new Weapon("AKS-74u Assault Rifle", "AKS", "AKS-74u", "SMG"));
            tempWeapons.Add(new Weapon("AN-94 Abakan Assault Rifle", "AN94", "AN-94 Abakan", "AssaultRifle"));
            tempWeapons.Add(new Weapon("AS Val Supressed Assault Rifle", "ASVAL", "AS Val", "AssaultRifle"));
            tempWeapons.Add(new Weapon("DAO-12 Striker Shotgun", "DAO12", "DAO-12", "Shotgun"));
            tempWeapons.Add(new Weapon("Defibrillator", "DEFIB", "Defib", "Melee"));
            tempWeapons.Add(new Weapon("F2000 Assault Rifle", "F2000", "F2000", "AssaultRifle"));
            tempWeapons.Add(new Weapon("FAMAS Assault Rifle", "FAMAS", "FAMAS", "AssaultRifle"));
            tempWeapons.Add(new Weapon("Glock 18 Pistol", "G18", "Glock18", "Handgun"));
            tempWeapons.Add(new Weapon("MP5 Assault Rifle", "MP5", "HK53", "AssaultRifle"));
            tempWeapons.Add(new Weapon("Jackhammer/MK3A1 Shotgun", "MK3A1", "jackhammer", "Shotgun"));
            tempWeapons.Add(new Weapon("JNG90 Sniper Rifle", "JNG90", "JNG90", "SniperRifle"));
            tempWeapons.Add(new Weapon("Knife", "KNIFE", "Weapons/Knife/Knife; Not Weapon Knife_RazorBlade", "Melee"));
            tempWeapons.Add(new Weapon("L96A1 Sniper Rifle", "L96", "L96", "SniperRifle"));
            tempWeapons.Add(new Weapon("LSAT Light Machine Gun", "LSAT", "LSAT", "LMG"));
            tempWeapons.Add(new Weapon("M416", "M416", "M416", "AssaultRifle"));
            tempWeapons.Add(new Weapon("M417 Sniper Rifle", "M417", "M417", "SniperRifle"));
            tempWeapons.Add(new Weapon("M1014 Semi-automatic Shotgun", "M1014", "M1014", "Shotgun"));
            tempWeapons.Add(new Weapon("M16A4 Assault Rifle", "M16A4", "M16A4", "AssaultRifle"));
            tempWeapons.Add(new Weapon("WWII M1911 .45 Pistol", "M1911", "M1911", "Handgun"));
            tempWeapons.Add(new Weapon("M240B Machine Gun", "M240B", "M240", "LMG"));
            tempWeapons.Add(new Weapon("M249 SAW", "M249", "M249", "LMG"));
            tempWeapons.Add(new Weapon("M26 MASS Shotgun", "MASS", "M26Mass", "Shotgun"));
            tempWeapons.Add(new Weapon("M27 IAR", "M27IAR", "M27IAR", "LMG"));
            tempWeapons.Add(new Weapon("M320 Grenade luncher", "M320", "M320", "ProjectileExplosive"));
            tempWeapons.Add(new Weapon("M39 Sniper Rifle", "M39", "M39", "SniperRifle"));
            tempWeapons.Add(new Weapon("M40A5 Sniper Rifle", "M40A5", "M40A5", "SniperRifle"));
            tempWeapons.Add(new Weapon("M4A1 Carbine", "M4A1", "M4A1", "SMG"));
            tempWeapons.Add(new Weapon("M60 LMG", "M60", "M60", "LMG"));
            tempWeapons.Add(new Weapon("M67 Grenade", "M67", "M67", "Explosive"));
            tempWeapons.Add(new Weapon("M9 Pistol", "M9", "M9", "Handgun"));
            tempWeapons.Add(new Weapon("Baretta M93R", "93R", "M93R", "Handgun"));
            tempWeapons.Add(new Weapon("MG36", "MG36", "MG36", "LMG"));
            tempWeapons.Add(new Weapon("MK11 Mod 0 Sniper Rifle", "MK11", "Mk11", "SniperRifle"));
            tempWeapons.Add(new Weapon("Barrett M98B Sniper Rifle", "M98B", "Model98B", "SniperRifle"));
            tempWeapons.Add(new Weapon("MP7 Machine Gun", "MP7", "MP7", "SMG"));
            tempWeapons.Add(new Weapon("PKP Pecheneg Machine Gun", "PKP", "Pecheneg", "LMG"));
            tempWeapons.Add(new Weapon("PP-19 Bison SubMachine Gun", "PP19", "PP-19", "LMG"));
            tempWeapons.Add(new Weapon("PP-2000 SubMachine Gun", "PP2000", "PP-2000", "SMG"));
            tempWeapons.Add(new Weapon("QBB-95 Light Machine Gun", "QBB95", "QBB-95", "LMG"));
            tempWeapons.Add(new Weapon("QBU-88 Sniper Rifle", "QBU88", "QBU-88", "SniperRifle"));
            tempWeapons.Add(new Weapon("QBZ-95 Assault Rifle", "QBZ95", "QBZ-95", "AssaultRifle"));
            tempWeapons.Add(new Weapon("Repair Tool", "REPAIRTOOL", "Repair Tool", "Melee"));
            tempWeapons.Add(new Weapon("RPG/SMAW", "RPG", "RPG-7; Not Weapon SMAW", "ProjectileExplosive"));
            tempWeapons.Add(new Weapon("RPK-74M Light Machine Gun", "RPK74M", "RPK-74M", "LMG"));
            tempWeapons.Add(new Weapon("SCAR-L Assault Rifle", "SCARL", "SCAR-L", "AssaultRifle"));
            //tempWeapons.Add(new Weapon("SIG SG 550 Assault Rifle", "SIG", "SG 553 LB","SMG"));
            tempWeapons.Add(new Weapon("Saiga 20K Shotgun", "SAIGA", "Siaga20k", "Shotgun"));
            tempWeapons.Add(new Weapon("Simonow SKS-45 Rifle", "SKS", "SKS", "SniperRifle"));
            tempWeapons.Add(new Weapon("SPAS-12 Shotgun", "SPAS12", "SPAS-12", "Shotgun"));
            tempWeapons.Add(new Weapon("SV98 Snayperskaya", "SV98", "SV98", "SniperRifle"));
            tempWeapons.Add(new Weapon("SVD Sniper Rifle", "SVD", "SVD", "SniperRifle"));
            tempWeapons.Add(new Weapon("Aug A3 Assault Rifle", "AUG", "Steyr AUG", "AssaultRifle"));
            tempWeapons.Add(new Weapon("Taurus .44 Magnum revolver", "MAGNUM", "taurus .44", "Handgun"));
            tempWeapons.Add(new Weapon("Type88 Machine Gun", "TYPE88", "Type88", "LMG"));
            tempWeapons.Add(new Weapon("USAS-12 automatic Shotgun", "USAS12", "USAS-12", "Shotgun"));
            tempWeapons.Add(new Weapon("A-91 Assault Rifle", "A91", "Weapons/A91/A91", "SMG"));
            tempWeapons.Add(new Weapon("AK-74 Assault Rifle", "AK74", "Weapons/AK74M/AK74", "AssaultRifle"));
            tempWeapons.Add(new Weapon("G36C Assault Rifle", "G36C", "Weapons/G36C/G36C", "SMG"));
            tempWeapons.Add(new Weapon("G3A3 Battle Rifle", "G3A3", "Weapons/G3A3/G3A3", "AssaultRifle"));
            tempWeapons.Add(new Weapon("C4 Explosive", "C4", "Weapons/Gadgets/C4/C4", "Explosive"));
            tempWeapons.Add(new Weapon("Claymore mine", "CLAYMORE", "Weapons/Gadgets/Claymore/Claymore", "Explosive"));
            tempWeapons.Add(new Weapon("KH2002 Assault Rifle", "KH2002", "Weapons/KH2002/KH2002", "AssaultRifle"));
            tempWeapons.Add(new Weapon("Magpul Personal Defense Rifle", "MAGPUL", "Weapons/MagpulPDR/MagpulPDR", "SMG"));
            tempWeapons.Add(new Weapon("MP412 REX Revolver", "REX", "Weapons/MP412Rex/MP412REX", "Handgun"));
            tempWeapons.Add(new Weapon("MP-443 Grach Pistol", "MP443", "Weapons/MP443/MP443; Not Weapon Weapons/MP443/MP443_GM", "Handgun"));
            tempWeapons.Add(new Weapon("P90 Sub Machine Gun", "P90", "Weapons/P90/P90; Not Weapon Weapons/P90/P90_GM", "SMG"));
            tempWeapons.Add(new Weapon("SCAR-H Assault Rifle", "SCARH", "Weapons/SCAR-H/SCAR-H", "SMG"));
            tempWeapons.Add(new Weapon("UMP-45 SubMachine Gun", "UMP45", "Weapons/UMP45/UMP45", "SMG"));
            tempWeapons.Add(new Weapon("L85A2/SA80 Assault Rifle", "L85A2", "Weapons/XP1_L85A2/L85A2", "AssaultRifle"));
            tempWeapons.Add(new Weapon("ACW-R Assault Rifle", "ACWR", "Weapons/XP2_ACR/ACR", "AssaultRifle"));
            tempWeapons.Add(new Weapon("L86A2 Light Machine Gun", "L86A2", "Weapons/XP2_L86/L86", "LMG"));
            tempWeapons.Add(new Weapon("M5K Submachine Gun", "M5K", "Weapons/XP2_MP5K/MP5K", "SMG"));
            tempWeapons.Add(new Weapon("MTAR-21 Assault Rifle", "MTAR", "Weapons/XP2_MTAR/MTAR", "AssaultRifle"));
            this.weaponList = tempWeapons;
            //create the dictionary, used for when people vote for weapons
            this.weaponDictionary = new Dictionary<string, Weapon>();
            foreach (Weapon weapon in this.weaponList)
            {
                this.weaponDictionary.Add(weapon.inGameName, weapon);
            }
        }
        /**
         * Initializes weapon lock on the server, make sure proconrulz is running
         * */
        public void startWeaponLocking()
        {
            this.randomCalculateNextWeapon();
            this.ExecuteCommand("procon.protected.tasks.add", "taskPrintVoteWeaponRules", "0", "20", "-1", "procon.protected.plugins.call", "xVoteWeapon", "printVoteWeaponRules");
            ConsoleWrite("Weapon Lock Enabled!");
        }
        /**
         * Removes weapon lock from the server and shuts down the plugin
         * */
        public void stopWeaponLocking()
        {
            this.ExecuteCommand("procon.protected.tasks.remove", "taskPrintVoteWeaponRules");
            this.ExecuteCommand("procon.protected.plugins.setVariable", "ProconRulz", "Rules", "");
            this.ConsoleWrite("Weapon Lock Deactivated. All ProconRulz Rules Removed.");
        }
        /**
         * Decides/sets the current and next weapon randomly
         * The current and next weapons must never be the same
         * Done when the plugin is first enabled
         * */
        private void randomCalculateNextWeapon()
        {
            this.currentWeapon = this.getRandomWeapon();
            do
            {
                this.nextWeapon = this.getRandomWeapon();
            } while (this.nextWeapon.Equals(this.currentWeapon));
            ConsoleWrite("Next weapon changed randomly to: " + this.nextWeapon.description);
            this.setProconRulz();
        }
        /**
         * Based on the player's votes, set the next weapon
         * */
        private bool voteCalculateNextWeapon()
        {   
            //create new var to store the weapon counts
            Dictionary<Weapon, Int32> voteCounts = new Dictionary<Weapon, int>();
            //loop through all the given votes and count them up by weapon name
            foreach (KeyValuePair<String, Weapon> weaponVote in this.weaponVotes)
            {
                //check to see if the weapon is already in the vote list
                if (voteCounts.ContainsKey(weaponVote.Value))
                {
                    //if it is just increment the vote count by 1
                    voteCounts[weaponVote.Value] += 1;
                }
                else
                {
                    //if it isnt, add it with a vote count of 1
                    voteCounts.Add(weaponVote.Value, 1);
                }
            }
            Weapon topWeapon = null;
            Int32 topCount = 0;
            //loop through all the given votes counts
            foreach (KeyValuePair<Weapon, Int32> voteCount in voteCounts)
            {
                //check to see if the weapon is already in the vote list
                if (voteCount.Value>topCount)
                {
                    topCount = voteCount.Value;
                    topWeapon = voteCount.Key;
                }
            }
            if (!this.nextWeapon.Equals(topWeapon))
            {
                this.sayMessage("Next weapon changed by voting from the " + this.nextWeapon.description + " to the " + topWeapon.description);
                this.ConsoleWrite("Next weapon changed by voting from the " + this.nextWeapon.description + " to the " + topWeapon.description);
                this.nextWeapon = topWeapon;
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
         * Returns a random weapon from the weapon list
         * */
        private Weapon getRandomWeapon()
        {
            Random rand = new Random();
            int weaponIndexChoice = rand.Next(0, this.weaponList.Count);
            return this.weaponList[weaponIndexChoice];
        }
        /**
         * Sets up ProconRulz with the set of weapon limitations based on the currently set weapon
         * */
        private void setProconRulz()
        {
            String melee =  this.currentWeapon.type.ToLower().Equals("melee")?(""):(" Not Weapon Weapons/Knife/Knife; Not Weapon Knife_RazorBlade;");
            //String roundString = "On Round;Exec vars.serverName CQ 24/7 VoteWeapon - Current:" + this.currentWeapon.description + "; Say Current Weapon is the " + this.currentWeapon.description + ";Log server name set";
            String joinString =  "On Join; PlayerSay Welcome to VoteWeapon. This round only use the " + this.currentWeapon.description;
            String spawnString = "On Spawn; PlayerSay Type 'killme' if you haven't equipped the " + this.currentWeapon.description;
            String killString = "On Kill; Not Weapon " + this.currentWeapon.proconName + ";" + melee + " PlayerSay %p% Incorrect Weapon. This round only use the " + this.currentWeapon.description + ".;VictimSay %p% will be slain for using the wrong weapon on you.; Kill " + kill_delay;
            String kickString = "On Kill; Not Weapon " + this.currentWeapon.proconName + ";" + melee + " PlayerCount " + infractionsBeforeKick + "; VictimSay %p% was KICKED for using the wrong weapon repeatedly.; Kick Rule Breaking, you will be temp banned for 2 hours if caught again.";
            String tBanString = "On Kill; Not Weapon " + this.currentWeapon.proconName + ";" + melee + " PlayerCount " + infractionsBeforeTBan + "; VictimSay %p% was TEMP BANNED for using the wrong weapon repeatedly.; TempBan 200 Repeated Rule Breaking.";
            this.ExecuteCommand("procon.protected.plugins.setVariable", "ProconRulz", "Rules", joinString + "|" + spawnString + "|" + tBanString + "|" + kickString + "|" + killString);
            this.ExecuteCommand("procon.protected.plugins.setVariable", "ProconRulz", "Protect 'reserved slots' players from Kick or Kill", protect_reservedslot_players.ToString());
            this.ConsoleWrite("ProconRulz Rules Set. New allowed weapon is " + this.currentWeapon.description);
        }
        /**
         * Prints the server rules
         * */
        public void printVoteWeaponRules()
        {
            this.sayMessage("This server is VoteWeapon, right now you may only use " + this.currentWeapon.description);
        }
        /**
         * Runs the appropriate commands based on the player's message
         * */
        private void parseVoteChat(String speaker, String message)
        {
            if (message.Equals("killme"))
            {
                ExecuteCommand("procon.protected.send", "admin.killPlayer", speaker);
            }
            else if (message.Equals("currentweapon"))
            {
                this.playerSayMessage(speaker, "Current weapon is " + this.currentWeapon.description);
            }
            else if (message.Equals("nextweapon"))
            {
                ConsoleWrite("printing next weapon");
                if (!this.votingStarted)
                {
                    ConsoleWrite("voting has not started");
                    this.playerSayMessage(speaker, "Voting not started, next weapon decided randomly. Currently the " + this.nextWeapon.description + ". Type 'voteweapon' to start voting.");
                }
                else
                {
                    ConsoleWrite("voting has started");
                    this.playerSayMessage(speaker, "The next weapon based on votes will be the " + this.nextWeapon.description);
                }
            }
            else if (!this.votingStarted && message.Equals("voteweapon"))
            {
                this.startVotingSystem(speaker);
            }
            else if (!this.votingStarted && message.StartsWith("vote"))
            {
                this.startVotingSystem(speaker);
                this.sendVote(speaker, message);
            }
            else if (this.votingStarted && message.StartsWith("vote"))
            {
                this.sendVote(speaker, message);
            }
            else if (speaker.ToLower().Contains("lazyengineer") && (message.ToLower().Contains(" kick ") || message.ToLower().Contains(" ban ") || message.ToLower().Contains(" banned ")))
            {
                ExecuteCommand("procon.protected.send", "banList.add", "guid", this.currentPlayers[speaker].GUID,"seconds", "600" , "Bro....Bro....Don't. Temp Ban 10 minutes.");
                ExecuteCommand("procon.protected.send", "banList.save");
                ExecuteCommand("procon.protected.send", "banList.list");
                this.yellMessage("My brother lazyengineer has been TBanned for 10 mins for talking about kicking or banning someone. Love, ColColonCleaner.", 5);
            }
        }

        private void startVotingSystem(string speaker)
        {
            this.votingStarted = true;
            this.weaponVotes = new Dictionary<string, Weapon>();
            this.sayMessage(speaker + " started weapon voting!");
            this.sayMessage("Vote using the form 'vote WeaponCode'. See server details for weapon codes.");
        }

        private void sendVote(String speaker, String message)
        {
            String possibleWeaponString = message.Split(' ')[1].ToUpper();
            Weapon decision = new Weapon("blarg", "blarg", "blarg", "blarg");
            if (this.weaponDictionary.TryGetValue(possibleWeaponString, out decision))
            {
                bool changedVote = false;
                if (this.weaponVotes.ContainsKey(speaker))
                {
                    this.weaponVotes.Remove(speaker);
                    changedVote = true;
                }
                this.weaponVotes.Add(speaker, decision);
                if (!changedVote)
                {
                    this.playerSayMessage(speaker, "You voted the " + decision.description + " be used next round.");
                }
                else
                {
                    this.playerSayMessage(speaker, "You changed your vote to the " + decision.description + ".");
                }
                if (!this.voteCalculateNextWeapon())
                {
                    this.playerSayMessage(speaker, "Not enough to change next weapon, next weapon still the " + this.nextWeapon.description + ".");
                }
            }
            else
            {
                this.playerSayMessage(speaker, possibleWeaponString + " is an invalid weapon Code. Server description on battlelog has weapon codes.");
            }
        }
        #endregion

        #region console and log
        public enum MessageType { Warning, Error, Exception, Normal };

        public String FormatMessage(String msg, MessageType type)
        {
            String prefix = "[^b" + GetPluginName() + "^n] ";

            if (type.Equals(MessageType.Warning))
                prefix += "^1^bWARNING^0^n: ";
            else if (type.Equals(MessageType.Error))
                prefix += "^1^bERROR^0^n: ";
            else if (type.Equals(MessageType.Exception))
                prefix += "^1^bEXCEPTION^0^n: ";

            return prefix + msg;
        }


        public void LogWrite(String msg)
        {
            this.ExecuteCommand("procon.protected.pluginconsole.write", msg);
        }

        public void ConsoleWrite(String msg, MessageType type)
        {
            LogWrite(FormatMessage(msg, type));
        }

        public void ConsoleWrite(String msg)
        {
            ConsoleWrite(msg, MessageType.Normal);
        }

        public void ConsoleWarn(String msg)
        {
            ConsoleWrite(msg, MessageType.Warning);
        }

        public void ConsoleError(String msg)
        {
            ConsoleWrite(msg, MessageType.Error);
        }

        public void ConsoleException(String msg)
        {
            ConsoleWrite(msg, MessageType.Exception);
        }

        public void DebugWrite(String msg, int level)
        {
            if (fDebugLevel >= level) ConsoleWrite(msg, MessageType.Normal);
        }

        public void ServerCommand(params String[] args)
        {
            List<String> list = new List<String>();
            list.Add("procon.protected.send");
            list.AddRange(args);
            this.ExecuteCommand(list.ToArray());
        }
        #endregion

        #region getters setters
        public String GetPluginName()
        {
            return "xVoteWeapon";
        }

        public String GetPluginVersion()
        {
            return this.pluginVersion;
        }

        public String GetPluginAuthor()
        {
            return "ColColonCleaner";
        }

        public String GetPluginWebsite()
        {
            return "http://www.adkgamers.com";
        }

        public String GetPluginDescription()
        {
            return @"
            <h1>xVoteWeapon</h1>
            <p>
                !WARNING! this plugin automates ProconRulz, if you are currently using ProconRulz with any custom settings they will be erased when this plugin is enabled.
                This plugin allows for the 'VoteWeapon' gametype to be played.
            </p>
            <h2>Description</h2>
            <h3>Main</h3>
            <p>
                * Everyone playing on the server can vote which weapon is allowed for use.           <br/>
                * Only one weapon type (+ knife) is allowed each round, the weapon vote from the previous match decides it.<br/>
                * Players who break the rules are killed, then kicked, then TBanned.          <br/>
                * If a Melee weapon e.g. repair tool is chosen, the knife may not be used.          <br/>
                * If nobody votes during a match, the weapon for the next match is chosen randomly from the list below.          <br/>
                * If a player has not unlocked the current set weapon they must use the knife, no exceptions.          <br/>
                * Players must not use underslung weapons. Thanks to DICE the auto-admin thinks they don't exist, they will get the players killed.          <br/>
                * No cheating you rascally admins, even protected players will be slain by this plugin when infractions are caught.          <br/>
            </p>
            <h3>Suggestions</h3>
            <p>
                * Use a low ticket count to let everyone have a chance at getting their favorite weapons played. Rounds are to be quick and dirty.<br>
            </p>
            <h2>Settings</h2>
            <p>
                * 'Delay Before Kill' - The amount of time(ms) after an infraction is committed that the player is slain. This gives them time to read the yell banner. Default is 2000ms.
                * 'Infractions Before Kick' - The number of infractions at which the player is kicked from the server. Default is 4.
                * 'Infractions Before TBan' - The number of infractions at which the player is Temp Banned from the server for 2 Hours. Default is 7.
                * 'Protect 'reserved slots' players from Kick or Kill' - Whether 'reserved slot' players will be protected from infraction punishment. Default is No.
            </p>
            <h2>Future Settings</h2>
            <p>
                * 'Disallowed Weapons' - List of Weapons that will show up as 'Disallowed Weapon' or 'Invalid Weapon Code' and cannot be used as a vote.
            </p>
            <h2>Current In-Game player Commands</h2>
            <p>
                * 'voteweapon' : 		Starts the vote process if not started already.          <br/>
                * 'vote WEAPONCODE' : 	Places your vote for the next weapon, where WEAPONCODE is a code from the list below, and WEAPONCODE is NOT case sensitive.          <br/>
						                It will start the vote system if not started already, and will tell you if the 'next weapon' was altered by your vote.          <br/>
                * 'currentweapon' : 	Tells you the current allowed weapon.          <br/>
                * 'nextweapon' : 		Tells you the current decided weapon for the next round.          <br/>
                * 'killme' : 			If you spawn with the wrong weapon on accident, type this command and you will be killed but your death count will not be incremented.          <br/>
            </p>
            <h2>Future In-Game player Commands</h2>
            <p>
                * 'skipweapon' : 		10 players type this command and the current weapon is skipped, the next round is then started, all kills/deaths from the previous round remain.          <br/>
            </p>
            <h2>Future In-Game admin Commands</h2>
            <p>
                * 'runnextround' : 		Just like skipweapon for players, but when run by an in-game admin requires no votes.
            </p>
            <h2>Weapon Codes</h2>
            <p>" + this.getWeaponHTML() + @"</p>
            <h2>Development</h2>
            <p>
                Started by ColColonCleaner for ADK Gamers on Oct. 31, 2012
            </p>
            <h3>Changelog</h3>
            <blockquote>
                <h4>0.1 (31-OCT-2012)</h4>
	                - initial version          <br/>
                <h4>0.2 (31-OCT-2012)</h4>
	                - initial updates and bug fixes          <br/>
                <h4>0.3 (1-NOV-2012)</h4>
	                <b>Weapon Codes:</b>          <br/>
                        Glock18 Pistol code is now 'G18'          <br/>
                        Repair Tool code is now 'REPAIRTOOL'          <br/>
                        Grenade code is now 'M67'          <br/>
                    <b>Bug Fixes and Enhancements:</b>          <br/>
                        Weapon codes are no longer case sensitive.          <br/>
                        ASVal and RepairTool bugs fixed.          <br/>
                        Implemented Kick after 4 infractions and TBan for 2 hours after 7 infractions.          <br/>
                        Plugin now tells players what the current weapon is on join, before spawning.          <br/>
                        Now players may start a vote just by saying 'vote WEAPONCODE', no need to say 'voteweapon' first, although that can still be done.          <br/>
                    <b>Still to fix:</b>          <br/>
                        Find other bugs and fix them.          <br/>
                <h4>0.4 (2-NOV-2012)</h4>
	                <b>Weapon Codes:</b>          <br/>
                        No Updates         <br/>
                    <b>Bug Fixes and Enhancements:</b>          <br/>
                        Bugs found with AN-94 and the premium knife, fixed.          <br/>
                        ASVal and RepairTool bugs fixed.          <br/>
                        Now if a melee weapon is chosen, only that melee weapon will be allowed, others disabled.          <br/>
                        Server name is updated each match with the new allowed weapon.         <br/>
                        Surprise added for TheLazyEngineer.      <br/>
                    <b>Still to fix:</b>          <br/>
                        Notify the admin if proconrulz is not running.          <br/>
                        Find other bugs and fix them.          <br/>
            </blockquote>
            ";
        }

        private String getWeaponHTML()
        {
            String weaponHTML = "";
            foreach (Weapon weapon in this.weaponList)
            {
                weaponHTML += weapon.ToString() + "<br/>";
            }
            return weaponHTML;
        }

        // ASSIGN values to program globals FROM server_ip.cfg
        public void SetPluginVariable(string strVariable, string strValue)
        {
            try
            {
                strValue = CPluginVariable.Decode(strValue);
                switch (strVariable)
                {
                    case "Delay before kill":
                        this.kill_delay = Int32.Parse(strValue);
                        break;
                    case "Tickets per Player":
                        this.TicketsPerPlayer = Int32.Parse(strValue);
                        break;    
                    case "Infractions Before Kick":
                        this.infractionsBeforeKick = Int32.Parse(strValue);
                        break;
                    case "Infractions Before 2 Hour TBan":
                        this.infractionsBeforeTBan = Int32.Parse(strValue);
                        break;
                    case "Protect 'reserved slots' players from Kick or Kill":
                        this.protect_reservedslot_players
                            = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
                        break;
                    default:
                        break;
                }
                //update proconrulz with whatever changes were made
                this.setProconRulz();
            }
            catch (Exception ex)
            {
                ConsoleWrite(ex.Message);
            }
        }

        // Allow procon to READ current values of program globals (and write values to server_ip.cfg)
        public List<PRoCon.Core.CPluginVariable> GetDisplayPluginVariables()
        {
            try
            {
                List<CPluginVariable> lst = new List<CPluginVariable>();
                lst.Add(new CPluginVariable("Delay before kill", typeof(int), this.kill_delay));
                lst.Add(new CPluginVariable("Tickets per Player", typeof(int), this.TicketsPerPlayer));
                lst.Add(new CPluginVariable("Infractions Before Kick", typeof(int), this.infractionsBeforeKick));
                lst.Add(new CPluginVariable("Infractions Before 2 Hour TBan", typeof(int), this.infractionsBeforeTBan));
                lst.Add(new CPluginVariable("Protect 'reserved slots' players from Kick or Kill", typeof(enumBoolYesNo), this.protect_reservedslot_players));
                return lst;
            }
            catch (Exception ex)
            {
                ConsoleWrite("ERROR PARSING Plugin Vars: " + ex.Message);
                return new List<CPluginVariable>();
            }
        }

        public List<CPluginVariable> GetPluginVariables()
        {
            return GetDisplayPluginVariables();
        }
        #endregion

        #region messaging
        public void playerSayMessage(String target, String message)
        {
            ExecuteCommand("procon.protected.send", "admin.say", message, "player", target);
            ExecuteCommand("procon.protected.chat.write", String.Format("(PlayerSay {0}) ", target) + message);
        }
        public void sayMessage(String message)
        {
            ExecuteCommand("procon.protected.send", "admin.say", message, "all");
            ExecuteCommand("procon.protected.chat.write", message);
        }
        public void yellMessage(String message, Int32 duration)
        {
            ExecuteCommand("procon.protected.send","admin.yell", message, duration.ToString(), "all");
            ExecuteCommand("procon.protected.chat.write", message);
        }
        #endregion

        #region events
        public void OnPluginLoaded(String strHostName, String strPort, String strPRoConVersion)
        {
            this.RegisterEvents(this.GetType().Name, "OnVersion", "OnServerInfo", "OnResponseError", "OnListPlayers", "OnPlayerJoin", "OnPlayerLeft", "OnPlayerKilled", "OnPlayerSpawned", "OnPlayerTeamChange", "OnGlobalChat", "OnTeamChat", "OnSquadChat", "OnRoundOverPlayers", "OnRoundOver", "OnRoundOverTeamScores", "OnLoadingLevel", "OnLevelStarted", "OnLevelLoaded");
        }
        public void OnPluginEnable()
        {
            fIsEnabled = true;
            startWeaponLocking();
            ConsoleWrite("Enabled!");
        }
        public void OnPluginDisable()
        {
            fIsEnabled = false;
            this.stopWeaponLocking();
            ConsoleWrite("Disabled =(");
        }
        public override void OnLevelLoaded(String mapFileName, String Gamemode, int roundsPlayed, int roundsTotal)
        {
            this.currentWeapon = this.nextWeapon;
            String newServerName = "24/7 VoteWeapon - This Round: " + this.currentWeapon.description;
            
            this.ExecuteCommand("procon.protected.send", "vars.serverName", newServerName);
            ConsoleWrite("Server Name set to: '" + newServerName + "'");
            this.setProconRulz();
            this.votingStarted = false;
            this.nextWeapon = this.getRandomWeapon();
        }
        //all messaging is redirected to global chat for analysis
        public override void OnGlobalChat(String speaker, String message)
        {
            parseVoteChat(speaker, message);
        }
        public override void OnTeamChat(String speaker, String message, int teamId) { this.OnGlobalChat(speaker, message); }
        public override void OnSquadChat(String speaker, String message, int teamId, int squadId) { this.OnGlobalChat(speaker, message); }

        public override void OnListPlayers(List<CPlayerInfo> players, CPlayerSubset subset)
        {
            Dictionary<String, CPlayerInfo> playerList = new Dictionary<String, CPlayerInfo>();
            foreach(CPlayerInfo player in players)
            {
                playerList.Add(player.SoldierName, player);
            }
            this.currentPlayers = playerList;
            String newTicketCountString = ((int)(this.currentPlayers.Count * this.TicketsPerPlayer * this.CQTicketCountModifier)) + "";
            this.ExecuteCommand("procon.protected.send", "vars.gameModeCounter", newTicketCountString);
            ConsoleWrite("Next round ticket count updated: " + newTicketCountString);
        }

        //unused methods
        public override void OnVersion(String serverType, String version) { }
        public override void OnServerInfo(CServerInfo serverInfo) { }
        public override void OnResponseError(List<String> requestWords, String error) { }
        public override void OnPlayerJoin(String soldierName){}
        public override void OnPlayerLeft(CPlayerInfo playerInfo){}
        public override void OnPlayerKilled(Kill kKillerVictimDetails){}
        public override void OnPlayerSpawned(String soldierName, Inventory spawnedInventory) { }
        public override void OnPlayerTeamChange(String soldierName, int teamId, int squadId) { }
        public override void OnRoundOverPlayers(List<CPlayerInfo> players) { }
        public override void OnRoundOverTeamScores(List<TeamScore> teamScores) { }
        public override void OnRoundOver(int winningTeamId) { }
        public override void OnLoadingLevel(String mapFileName, int roundsPlayed, int roundsTotal) { }
        public override void OnLevelStarted() { }
        #endregion

        #region helper classes
        //class to store the weapons for comparison
        public class Weapon
        {
            //made public as this is only used internally, not worried about security
            public String description, inGameName, proconName, type;
            public Weapon(String description, String inGameName, String proconName, String type)
            {
                if (description != null && inGameName != null && proconName != null && type != null
                    && description.Length > 0 && inGameName.Length > 0 && proconName.Length > 0 && type.Length > 0)
                {
                    this.description = description;
                    this.inGameName = inGameName;
                    this.proconName = proconName;
                    this.type = type;
                }
                else
                {
                    throw new Exception("Invalid init params for weapon");
                }
            }
            public override bool Equals(object obj)
            {
                Weapon otherWeapon = (Weapon)obj;
                return this.proconName.Equals(otherWeapon.proconName);
            }
            public override string ToString()
            {
                return " | " + this.description + " | " + this.inGameName + " | ";
            }
            public override int GetHashCode()
            {
                Int32 decPlace = 0;
                Int32 total = 0;
                foreach (char charac in this.proconName.ToCharArray())
                {
                    decPlace++;
                    total += (decPlace * charac);
                }
                return total;
            }
        }
        #endregion

    } // end xVoteWeapon
} // end namespace PRoConEvents



