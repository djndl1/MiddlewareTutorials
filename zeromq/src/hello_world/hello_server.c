#include <zmq.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <assert.h>
#include <stdbool.h>

int main(void)
{
	//  Socket to talk to clients
	void *context = zmq_ctx_new();
        // request-reply pattern
        // classic client/server RPC pattern
	void *responder = zmq_socket(context, ZMQ_REP);
	int rc = zmq_bind(responder, "tcp://*:5555");
	assert(rc == 0);

	while (true) {
		char buffer[10];
		zmq_recv(responder, buffer, 10, 0);
		printf("Server: Received Hello\n");
		sleep(1); //  Do some 'work'
		zmq_send(responder, "World", 5, 0);
	}
	return 0;
}
