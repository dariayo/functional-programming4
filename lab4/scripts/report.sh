#!/bin/bash

echo "Generating report..."

if [ -z "$1" ]; then
  echo "Usage: $0 <container_name>"
  exit 1
fi

CONTAINER_NAME=$1
LOGS_DIR="./logs"
REPORTS_DIR="./reports"

mkdir -p $LOGS_DIR
mkdir -p $REPORTS_DIR

LOG_FILE="$LOGS_DIR/$CONTAINER_NAME.log"
docker logs $CONTAINER_NAME > $LOG_FILE

dotnet run --project ./lab4.fsproj "$CONTAINER_NAME" "$LOG_FILE" "$REPORTS_DIR"

if [ $? -eq 0 ]; then
  echo "Report generated successfully: $REPORTS_DIR"
else
  echo "Failed to generate report."
fi