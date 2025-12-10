# HOGS OF WAR Map Editor

### A tool to edit, visualize and export maps, for PC version

Primary functions : display in a 2d or in 3D view all entities of each map of the game, 
customizing the game with modifing anything on the map and even adding new entities 

Extractor function : Convert any asset of the game into modern format : .GLB for models, .opus for audio, .png for textures...


found out how to swap map in the exe, i need to adapt the tool to be able to modify what map on what mission

<details>
  <summary>TODO :  </summary>



- stairs (prisms models) are rotated wrongly ?
	
- correct character animations exportations ( wierd stuff with arms and positions ?)
	
- create 3 different "modding .mad & .mtd", one for normal maps, one for snow maps, one for desert maps
	
- skyboxes : find witch map has what skybox? <- Decomp exe needed : found something but need investigation
	
- export 1 model with multiple skins (tents, shelter) -> possible but need GLB/GLTF extension?
	
- manage to export "heavy" character model with respective hat
	
- 3d viewer : place character according to team and class
	
- export an all-in-one character : Skeleton (hir) + animations (mcap) + all color skins (mtd) + attach hats 
	
- export FEBmps (ui)
	
- class triangle, plane, normal, vertices (pmg and vtx) -> use vector3 vector4 & vector2 ?
	
- 3d viewer : load all models and export then load 3d view with exported models

- 3D Viewer : Tile editor (slipery, mines, water)
	
</details>

<details>
  <summary>Features done : </summary>

- Map swapper : Create a window to modifiy map missions references

- Map swapper : read .exe at specific address to get current map mission order
	
- FileSystem managemenet : rewrite/improve 'file handling' sections and export folders manipulation 

- First use case (extracting editor ressources) 

- build entire application into a single Exe File (build.cmd)

- when adding new entity to map, create correct collision boxes

- manage to export character model with skeleton (.hir) 

- manage to export character model with skeleton + animations/motionCapture (.mcap)

- tempoary: export 7 dummy model with team colors (replacing characters to export)

- export models with attached barrel (pillbox, artillery, tank) 

- 3D editor : delete selected entity, insert when adding new entity

- 3D viewer : be able to move and rotate entities into the editor (turns out i'm such a genius 🤡)

- 3d viewer : parse objects and place / rotate entities

- 3D viewer : Migrate to webView2 + Babylon.js 

- ability to add any new entity into a pog (walls, bridges, flowers...)

- compress entire export folder with ZIP

- "export view window" with options + multithreading on exports processes (wasn't planned but still cool tho) 

- mega exporter, read all gamefiles and convert them (audio with ffmpeg, maps, skys & skyboxes, models,textures, some ui hats )

- HIR & MCAP to JSON 

- remove forced passing "extension" on PMG,PTG,etcc.. classes (use memorystream + FileReadallbyte instead of filestream)

- revamp the editor primary functions : rewrite "Additem" and save function 

- modding mode : build a verified working MAD + mtd containg all models of the game with correct fac indexes

- pigs are displayed with cross instead of squares

- PogToJson V2 (easy, c# JsonSerialiser since .net 7)

- export skyboxes

- TIM.ConvertToPng() function

- export map (terrain) with textures

</details>




Scrapped/ cancelled : 
- converting fonts images to TTF (no library available, too complex, useless)


_________________________________________________________


Planned features if one day we are able to decomple the code of the game :

- AI behavior 
- game fixed resolution / borderless window (d3d9.dll)
- extra classes
- modifiable progression tree 
- machine gun behavior (manual direction while autoshooting + recoil , -4hp & -8hp)
- extra weapons 
- new campaigns
- defense mode



