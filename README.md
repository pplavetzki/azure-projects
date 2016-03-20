# Example of Various Azure Technologies
This project was developed to showcase a few pieces of Azure's Cloud platform.  It's by no means production ready and it's functionality is quite trivial, however, it was intended to show top to bottom technology stack that uses some of Azure's fundamental technology.

An overview...it uses Angular as it's frontend and allows you to upload an image directly to Blob Storage.  This is secured using Shared Access Tokens.  When the image is uploaded to the blob the app then posts a update to the server and stores the meta data for the image in a Redis List.  Also during this post a queue message is created and sent to a Azure Messaging Queue.  In the background there is a worker role that is continuously checking the queue to see if it has anything to do.  If it finds a message it pulls the newly uploaded image from the blob and creates a new thumbnail, stores the thumbnail in another blob, and dequeues the message.  The worker role also updates the Redis Cache with the new url for the thumbnail.  During this background processing the frontend is polling the web api to see if the thumbnail is available.  Once the thumbnail is processed it is displayed without an refresh of the page.

This project uses the following technologies (client-side):

1.  AngularJs 1.5
2.  Browserify
3.  Gulp
4.  Bootstrap
5.  npm

On the Server I'm using:

1.  C# Asp.Net 4.6.1
2.  Web API 2.2
3.  Unity Dependency Injection

Azure Tools:

1.  Azure Web App
2.  Azure Cloud Service
3.  Azure Blob Storage
4.  Azure Messaging Queue
5.  Redis Cache
6.  Shared Access Security

