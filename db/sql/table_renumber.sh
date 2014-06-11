#!/bin/bash

I=0
for f in "$@"
do
    printf -v J "%03d" $I
    n=$(echo $f | sed -e 's/[0-9][0-9][0-9]/'$J'0/g')
    mv $f $n
    echo $n
    let I++
done
