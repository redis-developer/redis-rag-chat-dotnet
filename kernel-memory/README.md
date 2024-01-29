# Kernel Memory Service

The [Kernel Memory Service](https://github.com/microsoft/kernel-memory) is a stand alone service that can efficiently index and query unstructured multi-modal data. For our purposes we use Kernel Memory
with our Python example to manage the ingestion and querying of memories into our LLM.

## How to Run

To Run Kernel Memory move make a copy of `appsettings.Template.json` and rename it to `appsettings.json`, then set the `APIKey` field in it to match your OpenAI API key. Then you can run it in docker with:

```
docker-compose build
docker-compose up
```