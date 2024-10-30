#!/usr/bin/env python3

from jsonrpc_socketserver import JsonRpcSocketMessage

import socket
import json
import jsonrpc

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
        payload = jsonrpc.jsonrpc2.JSONRPC20Request.from_data({
            "method": method,
            "params": parameters,
            "jsonrpc": "2.0",
            "id": self.new_id
        })
        msg = JsonRpcSocketMessage(sequence=self.new_sequence_number)
        msg.jsonrpc_data = payload.json
        buf = bytes(msg)
        self._socket.sendall(buf)

        return self.receive_response()

    def receive_response(self):
        buf = self._socket.recv(JsonRpcSocketMessage.header_length)
        message = JsonRpcSocketMessage.from_bytes(buf)

        received_len = 0
        data_buf = bytearray()
        while received_len < message.data_length:
            remaining_len = message.data_length - received_len
            bufsize = remaining_len if remaining_len < 1024 else 1024
            buf = self._socket.recv(bufsize)

            data_buf.extend(buf)
            received_len += len(buf)

        data = data_buf[:message.data_length]
        # supposedly UTF-8
        json_string = data.decode(encoding='utf-8')

        return json.loads(json_string)

if __name__ == '__main__':
    client = JsonRpcSocketClient()
    client.connect("127.0.0.1", 9999)

    print(client.request("add", [1, 2]))
    print(client.request("upper", ["A"]))
    print(client.request("upper", ["abc"]))
    print(client.request("upper", ["AGdwerdaspiueqwr"]))
    print(client.request("echo", []))
