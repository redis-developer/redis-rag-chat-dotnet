import datetime
import uvicorn
import semantic_kernel as sk
from semantic_kernel.orchestration.context_variables import ContextVariables
import os
from fastapi import FastAPI, File, UploadFile
from ulid import ULID
from webapi.models import AuthorRole, Ask, ChatMessage
from webapi.constants import CHAT_MESSAGE_INDEX_NAME
from fastapi.middleware.cors import CORSMiddleware
import redis
import uuid
import requests
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

origins = [
    "http://localhost",
    "http://localhost:8000",
    "http://localhost:3000"
]

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

kernel = sk.Kernel()


greeting = os.getenv("GREETING")

api_key = os.getenv("OPENAI_API_KEY")

kernel_memory_url = os.getenv("KERNEL_MEMORY_URL")

redis_client = redis.Redis(host='localhost', port=6379, db=0)

oai_chat_service = OpenAIChatCompletion(ai_model_id="gpt-3.5-turbo", api_key=api_key)
kernel.add_chat_service(service=oai_chat_service, service_id='completion')
plugins_dir = "skills"
utility_functions = kernel.import_semantic_skill_from_directory(plugins_dir, "utility")

ChatMessage.make_index(redis_client)


@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/testenv")
def test_env():
    return {"test_key": os.getenv("TEST_KEY")}


@app.post("/chat/startChat")
def start_chat():
    ulid = ULID()
    msg = ChatMessage(
        pk=uuid.uuid4(),
        message=greeting,
        chatId=str(ulid),
        author_role=AuthorRole.Bot,
        timestamp=int(datetime.datetime.now().timestamp())
    )

    msg.save(redis_client)

    return msg


def formatted_message_history(chat_id: str) -> str:
    query_str = f"@chatId:{{{chat_id}}}"
    messages = redis_client.ft(CHAT_MESSAGE_INDEX_NAME).search(query_str)
    lines = []
    for msg in messages.docs:
        if msg["author_role"] == "AuthorRole.User":
            lines.append(f"User: {msg['message']}")
        else:
            lines.append(f"Bot: {msg['message']}")

    result = '\n'.join(lines)
    return result


async def get_intent(summary: str, ask: Ask) -> str:
    variables = ContextVariables(ask.prompt, {"summary":summary})
    intent_function = utility_functions["intent"]
    intent = await intent_function.invoke_async(variables=variables)
    return intent.result


async def get_summary(chat_id: str) -> str:
    history = formatted_message_history(chat_id)
    variables = ContextVariables(history)
    summarize_function = utility_functions["summarize"]
    summary = await summarize_function.invoke_async(variables=variables)
    return summary.result


async def get_bot_message(question: str, memories:str, summary: str, chat_id: str) -> ChatMessage:
    variables = ContextVariables(question, {"memories": memories, "summary": summary})
    chat_function = utility_functions["chat"]
    response = await chat_function.invoke_async(variables=variables)

    return ChatMessage(
        pk=uuid.uuid4(),
        message=response.result,
        chatId=chat_id,
        author_role=AuthorRole.Bot,
        timestamp=int(datetime.datetime.now().timestamp()))


def get_memories(question: str) -> str:
    data = {
        "index": "km-py",
        "query": question,
        "limit": 5
    }

    response = requests.post(f"{kernel_memory_url}/search", json=data)

    if response.status_code == 200:
        response_json = response.json()
        memories = response_json.get('results',[])
        res = ""
        for memory in memories:
            res += "memory:"
            for partition in memory['partitions']:
                res += partition['text']
            res += '\n'
        return res

    raise Exception(response.text)


@app.post("/documents/upload")
async def upload_document(file: UploadFile = File(...)):
    file_content = await file.read()

    data = {
        "index": "km-py",
        "id": str(uuid.uuid4())
    }
    files = {'file': (file.filename, file_content, file.content_type)}

    response = requests.post(f"{kernel_memory_url}/upload", files=files, data=data)
    response.raise_for_status()
    return {"status": response.status_code, "response_data": response.text}


@app.post("/chat/{chat_id}")
async def chat(chat_id: str, ask: Ask):

    user_message = ChatMessage(
        pk = uuid.uuid4(),
        message=ask.prompt.strip(),
        chatId=chat_id,
        author_role=AuthorRole.User,
        timestamp=int(datetime.datetime.now().timestamp())
    )

    summary = await get_summary(chat_id)
    intent = await get_intent(summary, ask)
    memories = get_memories(intent)
    bot_response = await get_bot_message(question=ask.prompt, memories=memories, summary=summary, chat_id=chat_id)

    user_message.save(redis_client)
    bot_response.save(redis_client)

    return bot_response


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)