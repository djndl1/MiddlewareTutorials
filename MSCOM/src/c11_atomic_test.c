#include "utest.h"

#include <stdatomic.h>
#include <stdbool.h>

#include <pthread.h>

#define THREAD_COUNT 1000

UTEST(STDATOMIC, FEATURE_TEST)
{

#ifdef __STDC_NO_ATOMICS__
    UTEST_PRINTF("This compiler's got no atomic support\n");
#else
    UTEST_PRINTF("This compiler has C11 atomic support\n");
#endif
}

static atomic_int started_count;

static void *threaded_print_func(void *arg)
{
    atomic_fetch_add(&started_count, 1);

    pthread_exit((void*)0);

    return (void*)0;
}

UTEST(STDATOMIC, INTEGER)
{

    for (int run = 0; run < 1000; run++) {
        started_count = 0;

        pthread_t thrd[THREAD_COUNT];

        for (size_t idx = 0; idx < THREAD_COUNT; idx++) {
            pthread_create(&thrd[idx], NULL, threaded_print_func, NULL);
        }

        for (size_t idx = 0; idx < THREAD_COUNT; idx++) {
            int result = -1;
            pthread_join(thrd[idx], (void**)&result);

            ASSERT_EQ(result, 0);
        }

        ASSERT_EQ(started_count, THREAD_COUNT);
    }
}

UTEST_MAIN();
