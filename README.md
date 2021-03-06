# GameP2P
![alt-text](https://camo.githubusercontent.com/2bcc9106bf8b9f021e536549084e9b48e461400d/687474703a2f2f692e696d6775722e636f6d2f645a4e456870772e706e67)

This is a Unity project demonstrating a peer-to-peer UDP game protocol. Relevant game code is at Assets/Scripts/Platformer/GameController.cs

The protocol works by first having the client contact a UDP rendezvous server hosted on an AWS EC2 instance. This is done so a connection is opened through any NAT's a user is acccessing the internet behind. When the rendezvous server has two open connections with clients, it sends the public address of the other client to each and then terminates the connections. After that the two clients establish a connection with each other using the open ports they used to contact the server.

The game itself is just meant to test this protocol and demonstrate that it works. It does this by sending x and y coordinates of your "player" to the other client. The other client then displays a sprite at that location, making each movement appear for both users at roughly the same time (<50 milliseconds latency).
