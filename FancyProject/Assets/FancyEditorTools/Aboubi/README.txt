This folder contains the shaders and models you'll need to make suit textures. A quick note on how suits work, since they're a bit strange:

- The default boiler suit uses the AboubiDefault shader, which uses uv channel 1
- All other suits, along with their first person arm materials, should use AboubiSuit, which uses uv channel 2
- That being said, using your own shaders will work fine if the default ones don't have enough options for the look you want.
- Note: for outlines to work on suits you must expose the properties _ASEOutlineColor (Color) and _ASEOutlineWidth (float).
  For an example implementation of outlines that is very similar to how the game implements them check out the default shaders.

I have no idea why they're set up like this~ it's probably related to the model being originally used in babbdi and repurposed for straftat. Also yeah the default suit is just an image of a guy in a boiler suit shoved onto Aboubi lol.