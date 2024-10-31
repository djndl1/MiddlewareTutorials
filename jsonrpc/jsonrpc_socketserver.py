#!/usr/bin/env python3

from jsonrpc import JSONRPCResponseManager
from jsonrpc.dispatcher import Dispatcher

from socketserver import ThreadingTCPServer, BaseRequestHandler

from numbers import Number

from jsonrpc_socketmessage import JsonRpcSocketMessage
from socket_utils import receive_until

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
                buf = receive_until(self.request,
                                    JsonRpcSocketMessage.header_length,
                                    JsonRpcSocketMessage.header_length)
                if len(buf) < JsonRpcSocketMessage.header_length:
                    break
            except ConnectionResetError:
                return

            message = JsonRpcSocketMessage.from_bytes(buf)

            try:
                data_buf = receive_until(self.request, message.data_length, 2048)
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

        import threading
        print(f"EOT, Leaving the handler thread: {threading.current_thread().native_id}")

if __name__ == '__main__':
    with  JsonRpcSocketServer(JsonRpcService(), ("127.0.0.1", 9999), JsonRpcRequestHandler) as server:
        server.serve_forever()
