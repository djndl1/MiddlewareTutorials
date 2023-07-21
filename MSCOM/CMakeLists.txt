cmake_minimum_required(VERSION 3.12)
project(MSCOMTutorial VERSION 0.1.0)

set(CMAKE_EXPORT_COMPILE_COMMANDS TRUE)
set(CMAKE_C_STANDARD 17)
set(CMAKE_CXX_STANDARD 17)

include_directories(include)

add_executable(c11_atomic_test src/c11_atomic_test.c)
add_executable(pthread_support src/pthread_support.c)
target_link_libraries(pthread_support -pthread)

add_executable(basic_com_support src/basic_com_support.c include/dotnet_i.c)
target_compile_definitions(basic_com_support PRIVATE COBJMACROS)
target_link_libraries(basic_com_support ole32 uuid oleaut32)
