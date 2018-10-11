#!/bin/bash

if [ $# -eq 0 ]; then
    echo "Usage: $0 <name of migration script>"
    exit 1
fi

name=$(echo $* | sed -e "s, ,_,g")
dir="$(pwd)/db/migrations"

if [ ! -d $dir ]; then
    mkdir $dir
fi

version=$(ls $dir | sed -E s,^\([0-9]+\).*,\\1,g | sed -E s,^0*,,g | sort -V | tail -1)
version=$((version + 1))
printf -v version "%04d" $version

file="$dir/${version}_$name.sql"

echo "-- $(date '+%Y-%m-%d %H:%M:%S') : $*" > $file

echo "Created migration script in: $file"
