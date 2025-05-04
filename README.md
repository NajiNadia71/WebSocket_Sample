# WebSocket_Sample
A WebSocket server is nothing more than an application listening on any port of a TCP server that follows a specific protocol. 
Creating a custom server can seem overwhelming if you have never done it before. It can actually be quite straightforward to implement a
basic WebSocket server on your platform of choice.
--------------------------------------------------------------------------
Just use 

dotnet run command to run the Server Project


and then same command to run the Client Project.
--------------------------------------------------------------------------
The WebSocket protocol is an independent TCP-based protocol. Its only relationship to HTTP is that its
handshake is interpreted by HTTP servers as an Upgrade request. The WebSocket protocol makes possible more interaction 
between a browser and a web site, facilitating live content and the creation of real-time games. This is made possible
by providing a standardized way for the server to send content to the browser without being solicited by the client, and
allowing for messages to be passed back and forth while keeping the connection open. In this way, a two-way (bi-directional) 
ongoing conversation can take place between a browser and the server.
--------------------------------------------------------------------------
This project is a simple example of a WebSocket server and client. The server is implemented in C# using 
.NET Core and the client is implemented is a console application.
User has the possibility to send messages to the server and the server will respond with the same message.
--------------------------------------------------------------------------
In all the WebSocket server implementations, the server listens on a specific port. When a client connects to this port,would be choose an action
and server send same response structure but with different message based on the client request.
--------------------------------------------------------------------------
