#!/usr/bin/env python3

import socket


def receive_until(socket: socket.socket, data_len: int, bufsize: int) -> bytearray:
    received_len = 0
    data_buf = bytearray()
    while received_len < data_len:
        remaining_len = data_len - received_len
        recsize = min(remaining_len, bufsize)

        buf = socket.recv(recsize)

        if not buf:
            break

        data_buf.extend(buf)
        received_len += len(buf)

    return data_buf
