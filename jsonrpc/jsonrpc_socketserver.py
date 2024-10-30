#!/usr/bin/env python3

from jsonrpc import JSONRPCResponseManager
from jsonrpc.dispatcher import Dispatcher

from socketserver import ThreadingTCPServer, BaseRequestHandler

from numbers import Number

import struct

#from pydantic import dataclasses
from dataclasses import dataclass


import json

JSONRPC_MESSAGE_MAGIC_NUMBER = 0xAEEF

@dataclass
class JsonRpcSocketMessage:
    '''binary message

    magic number: 0xAEEF
    sequence: uint16
    data_length: uint32
    jsonrpc_data: undefined length
    '''
    magic_number: int = JSONRPC_MESSAGE_MAGIC_NUMBER # 2 bytes
    sequence: int = 0 # uint16
    data_length: int = 0   # uint32

    _jsonrpc_data: bytes = bytes()

    header_length = 8

    @classmethod
    def from_bytes(cls, buf):
        magic, seq, length = struct.unpack('!HHI', buf)

        if magic != JSONRPC_MESSAGE_MAGIC_NUMBER:
            return None

        data = buf[cls.header_length:cls.header_length + length] \
            if len(buf) > cls.header_length else None

        return JsonRpcSocketMessage(magic, seq, length, data)

    @property
    def jsonrpc_data(self):
        return self._jsonrpc_data

    @jsonrpc_data.setter
    def jsonrpc_data(self, value):
        if isinstance(value, bytes) or isinstance(value, bytearray):
            self._jsonrpc_data = bytes(value)
        if isinstance(value, str):
            self._jsonrpc_data = value.encode('utf-8')
        else:
            self._jsonrpc_data = json.dumps(value).encode('utf-8')
        self.data_length = len(self._jsonrpc_data)

    def __bytes__(self):
        buf = bytearray()
        buf.extend(struct.pack('!HHI',
                               self.magic_number,
                               self.sequence,
                               self.data_length))
        buf.extend(self._jsonrpc_data)

        return bytes(buf)


class JsonRpcService:
    def __init__(self):
        self.dispatcher = Dispatcher()
        self.dispatcher["upper"] = self.upper
        self.dispatcher["add"] = self.add
        self.dispatcher["echo"] = self.echo

    def upper(self, s: str) -> str:
        return s.upper()

    def add(self, a: Number, b: Number) -> Number:
        return a + b

    def echo(self):
        return "Yes!"

class JsonRpcSocketServer(ThreadingTCPServer):
    def __init__(self, service, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.service = service

    def handle_jsonrpc_request(self, json_data: str):
        response = JSONRPCResponseManager.handle(json_data,
                                                 self.service.dispatcher)
        return response.json

class JsonRpcRequestHandler(BaseRequestHandler):
    def handle(self):
        while True:
            try:
                buf = self.request.recv(JsonRpcSocketMessage.header_length)
            except ConnectionResetError:
                break
            if not buf:
                break

            message = JsonRpcSocketMessage.from_bytes(buf)

            received_len = 0
            data_buf = bytearray()
            while received_len < message.data_length:
                try:
                    remaining_len = message.data_length - received_len
                    bufsize = remaining_len if remaining_len < 1024 else 1024
                    buf = self.request.recv(bufsize)

                    data_buf.extend(buf)
                    received_len += len(buf)
                except ConnectionResetError:
                    return

            data = data_buf[:message.data_length]
            # supposedly UTF-8
            json_string = data.decode(encoding='utf-8')

            response_str = self.server.handle_jsonrpc_request(json_string)

            response = JsonRpcSocketMessage(sequence=message.sequence)
            response.jsonrpc_data = response_str

            bs = bytes(response)
            self.request.sendall(bs)

if __name__ == '__main__':
    with  JsonRpcSocketServer(JsonRpcService(), ("127.0.0.1", 9999), JsonRpcRequestHandler) as server:
        server.serve_forever()
