# Redis Rag Chat

Redis Rag Chat is a simple Chat application demonstrating how to use the RAG pattern with various frameworks using Redis as your Vector Database.
For our example, the default chat type is a conversation with an intelligent agent that can recommend different beers to you, however
swapping that out with whatever other RAG operation you want is as simple as updating the SystemPrompt and changing out the documents you
store in your Redis Database.

### Frontend

```
cd frontend
npm install
npm start
```

### Backend

#### dotnet backend

```
dotnet run --project dotnet
```

### Adding Documents to your Redis Database

To add documents to Redis, the backend must be started

#### Upload the Provided Data

To upload the provided Data just run `./scripts/setup_beers.sh`

#### Bring your own data

If you want to bring your own data files to add to Redis, you can do so by using the `scripts/upload.sh`, 
passing in the directory where your files are as the first argument, and then optionally passing in a `limit` for the number of files you want 
to upload and an optional `url` argument to define the upload URL.

## Accessing the Site

After the frontend and backend are running, you can access the site on `localhost:3000`