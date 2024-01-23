from enum import Enum

import redis
from pydantic import BaseModel
from pydantic.types import UUID4
from redis.commands.search.field import (
    TagField,
    NumericField,
    TextField
)
from redis.commands.search.indexDefinition import IndexDefinition

from webapi.constants import CHAT_MESSAGE_INDEX_NAME, CHAT_MESSAGE_KEY_PREFIX


class AuthorRole(Enum):
    Bot = 1
    User = 2


class Ask(BaseModel):
    prompt: str


class ChatMessage(BaseModel):
    pk: UUID4
    chatId: str
    message: str
    author_role: AuthorRole
    timestamp: int

    def save(self, redis: redis.Redis):
        message_dict = self.model_dump()
        string_dict = dict({k: str(v) for k, v in message_dict.items()})
        redis_key = f"{CHAT_MESSAGE_KEY_PREFIX}{self.pk}"
        redis.hset(redis_key, mapping=string_dict)

    @staticmethod
    def make_index(redis : redis.Redis):
        try:
            definition = IndexDefinition(prefix=[CHAT_MESSAGE_KEY_PREFIX])
            try:
                redis.ft(CHAT_MESSAGE_INDEX_NAME).dropindex(delete_documents=True)
            except Exception:
                pass

            redis.ft(CHAT_MESSAGE_INDEX_NAME).create_index((TagField("chatId"), TextField("message"), NumericField("timestamp")), definition=definition)
        except Exception as e:
            print(e)


