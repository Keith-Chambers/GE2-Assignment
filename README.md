# Game Engines Assignment 2

Name: Keith Chambers

Student Number: C15324461

# Description of the assignment

Assignment consists of a scene in which two space armies confront each other in front of earth. Earth itself is surrounded by an asteroid belt that rotates around it.

The project is written using Unity's Entity Component System development features. 

# Instructions

Just run the project as normal. There are settings to tweak in the GameManager script attached to the ECSStarter GameObject. 

# How it works

## Armies

The number of rows and columns for both the enemy and defenders formation is specified in the GameManager script to spawn spacecrafts. Each formation is given a target position to arrive to so the formations will travel towards this point. 

Spacecrafts are able to move in any direction and will automatically face the correct way as they travel. Like the rest of this project, this is done using ECS. In particular the ArriveSystem is responsible for taking all entities with Translation, Rotation, Velocity, BoidState (Which contains a seek target) and keeping the position and rotation updated. 

## Asteroid Belt

For this, a number of entities are rendered around the circumferance of the earth model. The AsteroidSystem then updates the orbit rotation of each asteroid so that it continually rotates around. 

The AsteroidSystem matches to all Entities with Translation, RotationPoint, OrbitSpeed, OrbitRadius and OrbitRotation.

+ **Translation** - The current position of the Asteroid
+ **RotationPoint** - The point around with the rotation will occur. The axis is assumed to be the y axis
+ **OrbitSpeed** - The speed in which an asteroid rotates around it's point of rotation
+ **OrbitRadius** - How far out it rotates from the point of rotation.
+ **OrbitRotation** - The current rotation angle around the point of rotation.

Seeing as Transform cannot be assessed from a Job, simple trigonometry is used to calculate the position of each asteroid. 

	position.Value.x = Mathf.Cos(orbitRotation.Value) * orbitRadius.Value;
	position.Value.z = Mathf.Sin(orbitRotation.Value) * orbitRadius.Value;

The y position is set upon being created, but not modified in the AsteroidSystem itself. 


# What I am most proud of in the assignment

I'm most proud of making the project out of ECS and of the asteroid belt. 


# Issues 

Unfortunely I was unable to get ECS fully working for me and as a result had to work under to limitation of not being able to reference other entities within my job system. 

As Jobs do not run on the main thread you cannot use an EntityManager inside one. The way around this, to my knowledge is to create a barrier (A hard sync point for updates) and request an EntityCommandBuffer which you pass along to the Job from the OnUpdate method in the JobComponentSystem derived class. However, I was unable to get this to work for me and I am still unsure why. As a result, after having spent a lot of time I had to work on features which could be made without needing to get or set other entities component data. This is a main reason why the project is so barebones and does not contain all the behaviours that were intended for the project. Even something like avoidance would require being able to access the locations for all objects that it could potentially collide with. 

Although ECS is very cool and definitely worth learning, I think the API is a little too unstable as I encounted a good few manaual pages from the documentation which had been deleted and almost all code I could find on it was obsolete for the version I was using. I managed to learn the core principles for how Unity is implmenting the paradigm though and I will be excited to make something else using it down the line as I think it will be a very powerful way of writing games with high object counts.

# Video Demo 

[![YouTube](https://www.youtube.com/watch?v=v9Gie_VwvOo&t/0.jpg)](https://www.youtube.com/watch?v=v9Gie_VwvOo&t)

