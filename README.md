# BEFORE GOING FURTHER

I did this project on my free time when I was a student. At this time I was very ambitious and had a poor understanding of noise functions.

Please do not use this code in your project, or use it as a reference. It is badly designed and bugged.

Since I have no plan to fix this code yet, I will leave it untouched.
I keep it online as a memory of that time, and a showcase.

I wish you the best in your project.

Zibe

# Dryade

Dryade is a toolbox for procedural generation, dedicated to Unity. Currently it provide :

  - Relief procedural generation, using fBm algorithm.
  - More realistic heterogenous relief procedural generation, using multifractals.

All these tools run using seed system, granting control on randomness.

![Alt Text](https://github.com/Zibe/Dryade/blob/master/HybridMultifractal%20Terrain.JPG)

## Quick Start

Include the scripts you want to use directly in your Unity project.

- ### fBm
    Attach this script to a Terrain GameObject. Call the SetHeightMap function from the update when you want to generate a new relief. 
    - The H parameter have to be set between 0 and 1.0 . The lower it is, the more sharpened your terrain will be.
    - The lacunarity is the frequency of your fractals, and may be set between 2 and 10. A low frequency result in many little relief, a high frequency will give you big compacts ones.
    - Octaves is the number of repetion of your fractals, and may be set between 1 and 10. A high octaves will generate more complexicate terrain.
    - Divider allow to flatern the whole terrain.
    - BaseGround is the level 0 of your terrain. An high BaseGround (close to 1.0) wil generate difference in height.
    - Seed provide randomness.

- ### HybridMultifractal
	Currently the best tool provided by Dryade, it generates heterogenous realistic and smooth terrains. The different parameter are quite the same that in fBm
	- H may be really low for better result.
	- Offset work in a way quite similar of BaseGround in fBm, but it's part of the algorithm, so be sure to not let it set to 0.
	
## Development

Want to contribute? Have any question/request/idea ? Fell free to contact me.


## Todos

 - Procedural river.
 - Relief based on hydrography.
 - Grant more control on final result.
 - Anything to create more realistic natural and beautiful landscape.

## License
Feel free to use thoses script in any of your creations. I will be really glad if you send me some screenshots of your Unity projects which use them.
