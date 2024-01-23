# Python Example

The Python example leverages Semantic Kernel Python to perform chat while using Redis as the memory store for it's retriever. 

## How to Run

### Configuration

change directory into the `webapi` directory.

run 

```sh
mv .env.sample .env
```

And change the `OPENAI_API_KEY` to your Open AI API key

### Run the example

With poetry installed just run:

```sh
poetry run start
```