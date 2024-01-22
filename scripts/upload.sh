#!/bin/sh

ENDPOINT_URL=http://localhost:8000/documents/upload

limit=-1

if [[ ! -d "$1" ]]; then
  echo "Error: The provided directory was not a directory: $1." >&2
  exit 1
fi

DATA_DIR=$1

while [[ $# -gt 0 ]]; do
    case $1 in
        url)
            ENDPOINT_URL="$2"
            shift # Move past the value
            ;;    
        limit)
            limit=$2
            shift
            ;;
    esac
    shift # Move to the next argument
done

counter=0

for file in "$DATA_DIR"/*
do
  if [ -f "$file" ]; then
    let counter=counter+1
    echo "uploading $file..."
    curl -F "file=@$file" $ENDPOINT_URL > /dev/null 2>&1
    # echo "Done uploading $file"

    if [ ! -r "$file" ]; then
        echo "Cannot read file $file, skipping..."
        continue
    fi

    if [[ $limit -ne -1 ]] && [[ $counter -ge $limit ]]; then
      echo "Uploaded $limit files, stopping now."
      break
    fi

    if [[ $limit -ne 1 ]]; then
       echo "Uploaded $counter of $limit files"
    else
       echo  "Uploaded $counter files"
    fi
  fi
done