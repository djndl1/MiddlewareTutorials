#include "zhelpers.h"

int main(int argc, char *argv[])
{
	//  Socket to talk to server
	printf("Collecting updates from weather server...\n");
	void *context = zmq_ctx_new();
	void *subscriber = zmq_socket(context, ZMQ_SUB);
	int rc = zmq_connect(subscriber, "tcp://localhost:5556");
	assert(rc == 0);

	//  Subscribe to zipcode, default is NYC, 10001
	const char *filter = (argc > 1) ? argv[1] : "10001 ";
        // the client must set subscriptions otherwise no messages will be received
	rc = zmq_setsockopt(subscriber, ZMQ_SUBSCRIBE, filter, strlen(filter));
	assert(rc == 0);

	//  Process 100 updates, the first one is always lost
        //  since the publisher might already send all messages
        // before the connection from the subscriber is established
        // the publisher doesn't care if a subscriber is connected
	long total_temp = 0;
        int update_nbr;
	for (update_nbr = 0; update_nbr < 100; update_nbr++) {
		char *string = s_recv(subscriber);

		int zipcode, temperature, relhumidity;
		sscanf(string, "%d %d %d", &zipcode, &temperature,
		       &relhumidity);
		total_temp += temperature;
		free(string);
	}
	printf("Average temperature for zipcode '%s' was %dF\n", filter,
	       (int)(total_temp / update_nbr));

	zmq_close(subscriber);
	zmq_ctx_destroy(context);
	return 0;
}