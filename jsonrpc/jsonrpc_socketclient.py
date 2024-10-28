#!/usr/bin/env python3

from jsonrpc_socketserver import JsonRpcSocketMessage

import socket
import json

class JsonRpcSocketClient:
    def __init__(self):
        self._socket = socket.socket()
        self._id_counter = 0
        self._sequence_counter = 0

    @property
    def new_id(self):
        i = self._id_counter
        self._id_counter += 1
        return i

    @property
    def new_sequence_number(self):
        n = self._sequence_counter
        self._sequence_counter += 1
        return n

    def connect(self, host, port):
        self._socket.connect((host, port))

    def request(self, method, parameters):
        payload = {
            "method": method,
            "params": parameters,
            "jsonrpc": "2.0",
            "id": self.new_id
        }
        msg = JsonRpcSocketMessage(self.new_sequence_number)
        msg.jsonrpc_data = payload
        buf = bytes(msg)
        self._socket.sendall(buf)


if __name__ == '__main__':
    client = JsonRpcSocketClient()
    client.connect("127.0.0.1", 9999)

    client.request("add", [1, 2])
    client.request("upper", ["A"])
    client.request("upper", ["abc"])
    client.request("echo", [])
