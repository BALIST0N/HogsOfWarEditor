# Hogs_Game_Editor

a tool to edit maps data, planning to do much more 

Primary functions : display in a 2d format all entities of each map of the game, 
customizing the game with modifing stuff and even adding new ones 


done : 
- 3D viewer : Migrate to webView2 + Babylon.js 
- 3d viewer : parse objects and place & rotate entities
- ability to add any entity into a map (walls, bridges, flowers...)
- compress entire export folder with ZIP
- "export view window" with options + multithreading on exports processes (wasn't planned but still cool tho) 
- mega exporter, read all gamefiles and convert them (audio with ffmpeg, maps, skys & skyboxes, models,textures, some ui hats )
- HIR & MCAP to JSON 
- remove forced passing "extension" on PMG,PTG,etcc.. classes (use memorystream + FileReadallbyte instead of filestream)
- revamp the editor primary functions : rewrite "Additem" and save function 
- modding mode : build a verified working MAD + mtd with correct fac indexes, one for normal maps, one for snow maps 
- pigs are displayed with cross instead of squares
- PogToJson V2 (easy, c# JsonSerialiser since .net 7)
- export skyboxes 
- TIM.ConvertToPng() function
- export map (terrain) with textures


TODO : 
the tool : 
- 3d viewer : place character according to team and class
- manage to export character model with skeleton (.hir) 
- manage to export character model with animations/motionCapture (mcap)
- manage to export character model with a hat (combine two models)
- export 1 model with multiple skins (tents, shelter) 
- export 1 model with multiple skins and attached barrels (pillbox, artillery, tank) 
- export FEBmps (ui)
- export an all-in-one character : Skeleton (hir) + animations (mcap) + all color skins (mtd) + attach hats 
- class triangle, plane, normal, vertices (pmg and vtx) -> use vector3 vector4 & vector2 ?
- 3d viewer : load all models and export then load 3d view with exported models
- ultimate ! 3D viewer : be able to move and rotate entities into the editor (i'm not such genius so calm down)


Scrapped/ cancelled : 
- converting fonts images to TTF (no library available, too complex, useless)


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
