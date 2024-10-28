#!/usr/bin/env python3

from jsonrpc import JSONRPCResponseManager
from jsonrpc.dispatcher import Dispatcher

from socketserver import ThreadingTCPServer, BaseRequestHandler

from numbers import Number

import struct

#from pydantic import dataclasses
from dataclasses import dataclass


import json

@dataclass
class JsonRpcSocketMessage:
    sequence: int # uint16
    data_length: int = 1   # uint32

    _jsonrpc_data: bytes = bytes()

    magic_number: int = 0xAEEF # 2 bytes

    @staticmethod
    def from_bytes(buf):
        seq, length = struct.unpack('!HI', buf)
        data = buf[8:8+length] if len(buf) >= 8 + length else None

        return JsonRpcSocketMessage(seq, length, data)

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
        buf.extend(struct.pack('!HI', self.sequence, self.data_length))
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
                buf = self.request.recv(6)
            except ConnectionResetError as e:
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
                except ConnectionResetError as e:
                    return

            data = data_buf[:message.data_length]
            # supposedly UTF-8
            json_string = data.decode(encoding='utf-8')

            print(json_string)
            response_str = self.server.handle_jsonrpc_request(json_string)

            response = JsonRpcSocketMessage(message.sequence)
            print(response_str)
            response.jsonrpc_data = response_str

            self.request.sendall(bytes(response))

if __name__ == '__main__':
    with  JsonRpcSocketServer(JsonRpcService(), ("127.0.0.1", 9999), JsonRpcRequestHandler) as server:
        server.serve_forever()
