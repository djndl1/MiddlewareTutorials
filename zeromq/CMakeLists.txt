cmake_minimum_required(VERSION 3.12)
project(0MQTutorial VERSION 0.1.0)

set(CMAKE_EXPORT_COMPILE_COMMANDS TRUE)

find_package(PkgConfig REQUIRED)
pkg_check_modules(ZMQ REQUIRED libzmq)

add_compile_options(${ZMQ_CFLAGS})
include_directories(${ZMQ_INCLUDE_DIRS} ${0MQTutorial_SOURCE_DIR}/include)
link_directories(${ZMQ_LIBRARY_DIRS})
link_libraries(${ZMQ_LIBRARIES})

add_executable(hello_server src/hello_world/hello_server.c)
add_executable(hello_client src/hello_world/hello_client.c)

add_executable(weather_server src/weather_publish/weather_server.c)
add_executable(weather_client src/weather_publish/weather_client.c)


include(CTest)
enable_testing()

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)