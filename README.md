# Redis Rag Chat

Redis Rag Chat is a simple Chat application demonstrating how to use the RAG pattern with various frameworks using Redis as your Vector Database.
For our example, the default chat type is a conversation with an intelligent agent that can recommend different beers to you, however
swapping that out with whatever other RAG operation you want is as simple as updating the SystemPrompt and changing out the documents you
store in your Redis Database.

## Clone this repo

To clone this repo make sure you clone the whole repository with it's submodules:

```
git clone --recurse-submodules https://github.com/redis-developer/redis-rag-chat-dotnet
```

## Run with Docker

This app has a few different components, so to simplify standing it up, all you need to do is run:

```sh
docker compose build
OpenAIApiKey=YOUR_OPEN_AI_KEY docker compose up
```


## Running the Individual Components

You can also run the components individually:

### Frontend

```
cd frontend
npm install
npm start
```

### Backend

#### Configure it

To Configure your .NET app, configure the following in  `backend/appsettings.json` file:

| config var                       | Description                                                                                            |
|----------------------------------|--------------------------------------------------------------------------------------------------------|
| OpenAICompletionModelId          | ID for the selected language model version.                                                            |
| OpenAIApiKey                     | Key for API authentication and access.                                                                 |
| OpenAIEmbeddingGenerationModelId | ID for the text embedding generation model.                                                            |
| KernelMemoryEndpoint             | Endpoint for Kernel Memory (most likely localhost:9001)

#### Run Kernel Memory as a Service

Run `OpenAIApiKey=YOUR_OPEN_AI_KEY docker-compose -f kernel-memory/docker-compose.yml up`

### Run the backend project

You can run the .NET backend with the following command:

```
dotnet run --project backend
```

### Adding Documents to Redis

To add documents to Redis, the backend must be started

#### Upload the Provided Data

To upload the provided Data just run `./scripts/setup_beers.sh`

#### Bring your own data

If you want to bring your own data files to add to Redis, you can do so by using the `scripts/upload.sh`, 
passing in the directory where your files are as the first argument, and then optionally passing in a `limit` for the number of files you want 
to upload and an optional `url` argument to define the upload URL.

## Accessing the Site

After the frontend and backend are running, you can access the site on `localhost:3000`