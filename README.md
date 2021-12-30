fork of [osu-profile](https://github.com/Entrivax/osu-profile) by [Entrivax](https://github.com/Entrivax) with some more stats and features mainly added by me ([respektive on osu!](https://osu.ppy.sh/users/1023489/osu)) and [-Koyuki-](https://osu.ppy.sh/users/8202576)(https://github.com/FreddyBadAdd).

> Note that this is not actively being worked on anymore as it's just a pain in the ass to work with this code. In the future i might start working on a new osu! statistics tracking tool, but till then this has to do. Feel free to fork this yourself to add more stuff, as i probably won't check any PRs either. You might also check out [OsuAchievedOverlay](https://github.com/EngineerMark/OsuAchievedOverlay) which is getting regular updates and might get the spiritual successor of osu!Profile.

# Score Rank API
If you want to add the Score Rank to any of your programs you can use my Score Rank API which works by using the osu!Apiv2 to get the top 10k users of the score ranking leaderboard and saving it in a DB updating every 30min-ish.
- https://score.respektive.pw/rankings shows the top 50 for osu!std. you can use parameters to go to other pages or check a different mode e.g. `https://score.respektive.pw/rankings?page=2&mode=mania` or `https://score.respektive.pw/rankings?page=13&m=1`
- https://score.respektive.pw/u/3172980 to look up the rank and score for a user id. mode parameters work here too like `https://score.respektive.pw/u/3172980?m=3`
- https://score.respektive.pw/rank/727 to check the user for a specific rank. mode parameters work here too like `https://score.respektive.pw/rank/1?m=3`

> I will try to keep this API running until there is a Score Rank Endpoint in the official API. But don't be suprised if this might just go offline at one point (It probably won't but I will make sure to update this readme then tho)

# osu!Profile
osu!Profile is a little program for helping you to see your progress in the ranking (if you're not very attentive and miss how many rank you've just passed when you finished a map), also allow you to export these data into files and importing them into your livestream with OBS or XSplit!

## Features
- Shows your stats or these of any people
- Auto check your progress
- Choose the time interval between each update
- Auto check if new version is available
- You can export data shown in multiple files, many as you want
- Shows stats changes after each time you finished a game

## Links
[osu! forum thread](http://osu.ppy.sh/forum/t/252160) | [Website](http://entrivax.fr/osu!p) | [Get your API key](https://osu.ppy.sh/p/api)
