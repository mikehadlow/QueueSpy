#!/bin/bash
echo Running QueueSpy CI Bootstrapper

git pull origin master

ci/ci.sh
