cmake_minimum_required(VERSION 3.12)
project(MSCOMTutorial VERSION 0.1.0)

set(CMAKE_EXPORT_COMPILE_COMMANDS TRUE)
set(CMAKE_C_STANDARD 17)
set(CMAKE_CXX_STANDARD 17)

include_directories(include)

add_executable(c11_atomic_test src/c11_atomic_test.c)
add_executable(pthread_support src/pthread_support.c)
target_link_libraries(pthread_support -pthread)
