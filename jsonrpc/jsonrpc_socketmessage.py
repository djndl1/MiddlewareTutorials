#!/usr/bin/env python3

from pydantic import dataclasses
import json
import struct


@dataclasses.dataclass
class JsonRpcSocketMessage:
    '''binary message

    magic number: 0xAEEF
    sequence: uint16
    data_length: uint32
    jsonrpc_data: undefined length
    '''
    JSONRPC_MESSAGE_MAGIC_NUMBER = 0xAEEF

    magic_number: int = JSONRPC_MESSAGE_MAGIC_NUMBER # 2 bytes
    sequence: int = 0 # uint16
    data_length: int = 0   # uint32

    _jsonrpc_data: bytes = bytes()

    header_length = 8

    @classmethod
    def from_bytes(cls, buf):
        magic, seq, length = struct.unpack('!HHI', buf)

        if magic != cls.JSONRPC_MESSAGE_MAGIC_NUMBER:
            return None

        data = buf[cls.header_length:cls.header_length + length] \
            if len(buf) > cls.header_length else bytes()

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
