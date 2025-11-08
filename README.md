# Hogs_Game_Editor

a tool to edit maps data, planning to do much more 


Primary functions : display in a 2d format all entities of each map of the game, 
customizing the game with modifing stuff and even adding new ones 


done : 
- mega exporter, read all gamefiles and convert them (audio with ffmpeg, maps, skys & skyboxes, models,textures, some ui hats )
- HIR & MCAP to JSON 
- remove forced passing "extension" on PMG,PTG,etcc.. classes (use memorystream + FileReadallbyte instead of filestream)
- revamp the editor primary functions : rewrite "Additem" and save function 
- modding mode : build a verified working MAD + mtd with correct fac indexes, one for normal maps, one for snow maps 
- pigs are displayed with cross instead of squares
- PogToJson V2 (easy, c# JsonSerialiser in .net)
- export skyboxes 
- TIM.ConvertToPng() function
- export map (terrain) with textures


TODO : 
the tool : 
- export 1 model with multiple skins 
- export FEBmps (ui)
- export 1 charater with multiple skins colors
- manage to export character model with skeleton (.hir) 
- manage to export character model with animations/motionCapture (mcap)
- class triangle, plane, normal, vertices (pmg and vtx) -> use vector3 vector4 & vector2 ?
- 3d viewer : parse objects and place characters/entities
- 3d viewer : load all models and export then load 3d view with exported models
- 3d viewer : migrate to helixtoolkit.wpf.sharpDX (or some wpf DX12 library)
- export fonts , implement a way to transform vectors to ttf 

_________________________________________________________


todo for PC version (need to decompile code): 

- AI behavior
- game fixed resolution / borderless window
- extra classes
- modifiable progression tree 
- machine gun behavior (manual direction while autoshooting + recoil , -4hp & -8hp)
- extra weapons 
- new campaigns
- defense mode
