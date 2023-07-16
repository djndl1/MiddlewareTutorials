#include "utest.h"

#include <pthread.h>

static void *threaded_print_func(void *arg)
{
    UTEST_PRINTF("Threaded Printing from %zu!\n", pthread_self());
    return (void*)0;
}

UTEST(THREADING, PTHREAD_START)
{
    pthread_t thrd[10];

    for (size_t idx = 0; idx < 10; idx++) {
        pthread_create(&thrd[idx], NULL, threaded_print_func, NULL);
    }

    for (size_t idx = 0; idx < 10; idx++) {
        int result = -1;
        pthread_join(thrd[idx], (void**)&result);

        ASSERT_EQ(result, 0);
    }
}


UTEST_MAIN();
