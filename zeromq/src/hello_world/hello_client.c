//  Hello World client
#include <zmq.h>
#include <string.h>
#include <stdio.h>
#include <unistd.h>

int main(void)
{
	printf("Connecting to hello world server...\n");
	void *context = zmq_ctx_new();
	void *requester = zmq_socket(context, ZMQ_REQ);
	zmq_connect(requester, "tcp://localhost:5555");

	for (int request_nbr = 0; request_nbr != 10; request_nbr++) {
		char buffer[10] = { 0 };
		printf("Client: Sending Hello %d...\n", request_nbr);
		zmq_send(requester, "Hello", 5, 0);
		zmq_recv(requester, buffer, 10, 0);
		printf("Client: Received %s %d\n", buffer, request_nbr);
	}

	zmq_close(requester);
	zmq_ctx_destroy(context);
	return 0;
}