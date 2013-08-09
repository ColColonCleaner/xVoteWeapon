<h1>xVoteWeapon</h1>
<p>!!!WARNING!!! This plugin automates ProconRulz, if you are currently using ProconRulz with any custom settings they will be erased when this plugin is enabled. This plugin allows for the "VoteWeapon" gametype to be played.</p>

<h2>Descripton</h2>

<p>Everyone playing on the server can vote which weapon is allowed for use. Only one weapon type (+ knife) is allowed each round, the weapon vote from the previous match decides it. Players who break the rules are killed, then kicked, then temp banned. If a Melee weapon e.g. repair tool is chosen, the knife may not be used. If nobody votes during a match, the weapon for the next match is chosen randomly from the list below.</p>
<p>If a player has not unlocked the current set weapon they must use the knife, no exceptions. Players must not use underslung weapons. Thanks to DICE the auto-admin thinks they don't exist, they will get the players killed. No cheating you rascally admins, even protected players will be slain by this plugin when infractions are caught.</p>

<h2>Suggestions</h2>
<p>Use a low ticket count to let everyone have a change at getting their favorite weapons played. Rounds are to be quick and dirty.</p>
<ul>
    <li>Delay Before Kill
        <ul>
            <li>The amount of time(ms) after an infraction is committed that the player is slain. This gives them time to read the yell banner. Default is 2000ms. (2 seconds)</li>
        </ul>
    </li>
    <li>Infractions Before Kick
        <ul>
            <li>The number of infractions at which the player is kicked from the server. Default is 4.</li>
        </ul>
    </li>
    <li>Infractions Before Temp Ban
        <ul>
            <li>The number of infractions at which the player is Temp Banned from the server for 2 Hours. Default is 7.</li>
        </ul>
    </li>
    <li>Protect 'reserved slots' players from Kick or Kill
        <ul>
            <li>Whether 'reserved slot' players will be protected from infraction punishment. Default is No.</li>
        </ul>
    </li>
</ul>

<h2>Future Settings</h2>
<ul>
    <li>Disallowed Weapons
        <ul>
            <li>List of Weapons that will show up as 'Disallowed Weapon' or 'Invalid Weapon Code' and cannot be used as a vote.</li>
        </ul>
    </li>
</ul>

<h2>Current In-Game Player Commands</h2>
<ul>
    <li>voteweapon
        <ul>
            <li>Starts the vote process if not started already.<li>
        </ul>
    </li>
    <li>vote [WEAPONCODE]
        <ul>
            <li>Places your vote for the next weapon, where [WEAPONCODE] is a code from the list below, and [WEAPONCODE] is NOT case sensitive.</li>
            <li>It will start the vote system if not started already, and will tell you if the 'next weapon' was altered by your vote.</li>
        </ul>
    </li>
    <li>currentweapon
        <ul>
            <li>Tells you the current allowed weapon.</li>
        </ul>
    </li>
    <li>nextweapon
        <ul>
            <li>Tells you the current decided weapon for the next round.</li>
        </ul>
    </li>
    <li>killme
        <ul>
            <li>If you spawn with the wrong weapon on accident, type this command and you will be killed but your death count will not be incremented.</li>
        </ul>
    </li>
</ul>

<h2>Future In-Game Player Commands</h2>
<ul>
    <li>skipweapon
        <ul>
            <li>10 players type this command and the current weapon is skipped, the next round is then started, all kills/deaths from the previous round remain.</li>
        </ul>
    </li>
</ul>

<h2>Future In-Game Admin Commands</h2>
<ul>
    <li>runnextround
        <ul>
            <li>Just like skipweapon for players, but when run by an in-game admin requires no votes.</li>
        </ul>
    </li>
</ul>

<h2>Development</h2>
<p>Started by ColColonCleaner for ADK Gamers on October 31, 2012</p>
