#!/bin/bash

ENDPOINT_URL=http://localhost:5228/documents/upload

if [[ ! -z "$NUM_DOCUMENTS_TO_UPLOAD" ]] && [[ "$NUM_DOCUMENTS_TO_UPLOAD" =~ ^[0-9]+$ ]]; then
    limit=$NUM_DOCUMENTS_TO_UPLOAD
else
    limit=-1
fi

counter=0

for file in "$DATA_DIR"/*
do
  if [ -f "$file" ]; then
    let counter=counter+1
    echo "uploading $file..."
    curl -F "file=@$file" $ENDPOINT_URL
    echo "Done uploading $file"

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