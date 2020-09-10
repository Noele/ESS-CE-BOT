# ESS-CE-BOT
A discord bot for Eric's support server

It's purpose is to scan pictures sent on the server for "common errors", these errors are [mcp](https://minecraft.gamepedia.com/Programs_and_editors/Mod_Coder_Pack) related and are usualy sent as errors in a users console, the bot sends the url of the image to [ocr.space](https://ocr.space/) where it is scanned for text, said text is sent back to the bot as a string, where the bot can simply test it against its database of "Search"'s.

A "Search" on ESS-CE is just a string that may be contained within a pictures text, if the Search is found, the "Response" is sent.

A "Response" is as it sounds, it's what the bot responds to the picture with, usualy a brief description of the error, followed by a link to a more indepth description.

## TO:DO
<ul>
    <li>Upgrade the ping command so that it shows the actual ms response</li> 
    <li>Get some sleep</li> 
</ul>


#### Notes
I won't be providing much support for this bot other then the person it was made for, this was made to make our workflow easier, not to add to it :P That being said, the entire thing is documented, so understanding the inner workings shouldnt be hard as long as you don't speak moonrunes.
