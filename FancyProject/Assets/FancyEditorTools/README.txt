Hii! Welcome to the Fancy custom cosmetics sdk (v1.0.0)! This file contains important information on how to use the sdk to create your own custom cosmetics.

The two key GameObjects used by the sdk are:

- Cosmetic References. These are an GameObjects that contain information about each cosmetic in your bundle (metadata about the cosmetic, as well as the materials/prefabs that make up your cosmetic)
- Cosmetic Bundle Manifest. these are the root GameObject of a cosmetic bundle. They contain important metadata about the bundle, as well as a list of Cosmetic References that the bundle will contain when built

Some instructions on setting up and building your first bundle to get you started:

- Instantiate the "Cosmetics Bundle Template" prefab in a scene
- Right-click the root object of the prefab, and click Prefab > Unpack Completely
- Fill out the name, author and guid fields on the cosmetic manifest (the manifest describes general information about your bundle, as well as the included cosmetics, and is used to build the bundle and load the bundle ingame)
- If you're unsure on what to put for your GUID, it just needs to be a *UNIQUE* string that identifies your cosmetic bundle. I'd recommend using the format <author>.<bundle name> to ensure it's unique ~ for example, "kestrel.examplebundle"
- Create your cosmetic objects (the template includes some defaults that are already set up to show you the correct scripts to use and where to put the objects)

- For suits:
    - Create an empty GameObject as a child of your cosmetic manifest, and add the suit cosmetic reference component to it
    - Create your suit texture and first person arms texture
    - Create a new material using the AboubiSuit shader, and set the texture to your body texture (more info can be found in the Aboubi folder on this)
    - Do the same for the arms texture
    - Fill out the fields on your suit cosmetic reference
    - Optionally, drag the Aboubi GameObject into the "Dummy" field and click "Apply Suit To Dummy" to preview your suit on Aboubi

- For hats/cigs:
    - Create an empty GameObject as a child of the cosmetics anchor, and add the prefab cosmetic reference component to it
    - Import your cosmetic and set it up as a child of that GameObject
    - Set the cosmetic reference's prefab field to your cosmetic's GameObject (by dragging it into the field)
    - Important: if you'd like your cosmetic to appear offset from the anchor at all outside of transforming the model itself (or if you're making a cig that you need to position), you MUST have the GameObject actually containing your cosmetic be a child of an empty GameObject, then set the prefab field to that one. This is because the cosmetics' local positions are ignored when the game instantiates them.
    - Add a collider to at least one object in your cosmetic. This is required for hits on the hat to be registered by the game and for it to fall off properly.
    - Fill out the remaining fields in the cosmetic reference

- After setting up your cosmetics, add all the references to the "Cosmetic References" array on the cosmetic bundle manifest
- Hit "Build Bundle", and your built bundle should appear in the output directory (Assets/BuiltCosmeticBundles by default) as a .cbundle file
- Move this file anywhere within the game's plugins folder (using the plugins/CosmeticBundles folder is recommended, but not required), and start the game with Fancy installed. Your custom cosmetics should now appear in the cosmetics menu!

Extra notes:

- To take thumbnail screenshots for your cosmetics, you can use the CameraScreenshot script provided with the sdk. Just apply it to an object that has a Camera component!
- Aboubi's second, larger set of arms are the arms that you see in first person.
- There isn't actually a requirement for the general structure of your scene, the one walked through here is just an example of how you could set it up. That being said, all the cosmetic references your manifest holds' objects MUST be children of the manifest's object.
- You can have multiple manifests in a scene! For large cosmetic projects with multiple manifests, it might be a good idea to move the Aboubi dummy to the top level of the scene and reuse it for the suits in all of your manifests.
- For any extra questions/help/bug reports, you can contact me @sneaky_kestrel in the official Straftat discord.