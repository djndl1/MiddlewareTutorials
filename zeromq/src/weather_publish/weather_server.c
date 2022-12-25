#include "zhelpers.h"

#include <stdbool.h>

int main(void)
{
	//  Prepare our context and publisher
	void *context = zmq_ctx_new();
	void *publisher = zmq_socket(context, ZMQ_PUB);
	int rc = zmq_bind(publisher, "tcp://*:5556");
	assert(rc == 0);

	//  Initialize random number generator
	srandom((unsigned)time(NULL));
	while (true) {
		int zipcode = randof(100000);
		int temperature = randof(215) - 80;
		int relhumidity = randof(50) + 10;

		//  Send message to all subscribers
		char update[20] = { '\0' };
		sprintf(update, "%05d %d %d", zipcode, temperature,
			relhumidity);
		s_send(publisher, update);
                s_sleep(1);
	}
	zmq_close(publisher);
	zmq_ctx_destroy(context);
	return 0;
}