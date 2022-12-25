cmake_minimum_required(VERSION 3.12)
project(0MQTutorial VERSION 0.1.0)

set(CMAKE_EXPORT_COMPILE_COMMANDS TRUE)

include(CTest)
enable_testing()

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
