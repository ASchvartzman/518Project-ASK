# 518Project-ASK
Class Project for COS 518: Sumegha, Karan and Ariel. 

The first demo will feature streaming of objects with a simple prediction rule based on the speed and position of the user: we will stream all objects within the user's conical vision. The library of objects will include simple 3-D geometric shapes: cubes, spheres and such.

First task: 
* Download all relevant software: android studio + unity. 

Basic Goal:
* Set up server/client communication. 
* Code up k-d trees. 
* Test streaming. 
* User should be able to send point queries to the server and server either returns items or empty. 

Next Goal: 
* Mathematical discussion of conical thing.
* Code method to given phones speed and position, queries data structure for future possible objects to stream. 

Rendering with Unity:
* Need standard library of objects to work with. 
* Given a membership oracle for a convex body in 2 dimensions, given axis alligned rectangle. Want to know if rectangle has no intersection. Ideally, want in O(1) queries. 
* Saving objects, checking overlaps, etc. 

Test: 
* Phone Emulation (Thanksgiving)  
