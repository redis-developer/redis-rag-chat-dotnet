#!/bin/sh

dir=$(realpath $(dirname "$0"))
rm -rf /tmp/beers
echo Downloading beers file.
curl -o /tmp/beers.zip https://storage.googleapis.com/slorello/beer_file.zip
echo Unzipping Beers file
unzip /tmp/beers.zip -d /tmp > /dev/null 2>&1
echo Uploading beers file.
$dir/upload.sh /tmp/beers limit 500
