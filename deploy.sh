#!/bin/bash

DOCKER_IMAGE_NAME="weather-data-app"
DOCKER_CONTAINER_NAME="weather-data-container"
DOCKERFILE_PATH="./Dockerfile"
HTTP_PORT="8080"
HTTPS_PORT="9001"
PROJECT_ROOT="./"
APP_DLL_NAME="WeatherDataApp.dll"

docker build -t $DOCKER_IMAGE_NAME -f $DOCKERFILE_PATH $PROJECT_ROOT

if docker ps -a --format '{{.Names}}' | grep -Eq "^$DOCKER_CONTAINER_NAME$"; then
    docker stop $DOCKER_CONTAINER_NAME
    docker rm $DOCKER_CONTAINER_NAME
fi

docker run -d -p HTTP_PORT:80 HTTPS_PORT:443 --name $DOCKER_CONTAINER_NAME $DOCKER_IMAGE_NAME

sleep 5

docker ps --filter "name=$DOCKER_CONTAINER_NAME"

docker logs $DOCKER_CONTAINER_NAME

echo "A aplicação está sendo executada em HTTP: http://localhost:$HTTP_PORT/, HTTPS: https://localhost:$HTTPS_PORT/"
