module LogCollector.Parser

open System.Text.RegularExpressions
open System

type LogEntry =
    { Timestamp: DateTime
      LogLevel: string
      Message: string
      Source: string
      Thread: string }

let logPattern =
    @"^(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)\s+(?<level>[A-Z]+)\s+\d+\s+---\s+\[\s*(?<thread>[^\]]+)\]\s+(?<source>[^:]+)\s*:\s+(?<message>.+)"

let parseLog (log: string) =
    let matchResult = Regex.Match(log, logPattern)

    if matchResult.Success then
        Some
            { Timestamp = DateTime.Parse(matchResult.Groups.["timestamp"].Value)
              LogLevel = matchResult.Groups.["level"].Value
              Message = matchResult.Groups.["message"].Value
              Source = matchResult.Groups.["source"].Value.Trim()
              Thread = matchResult.Groups.["thread"].Value.Trim() }
    else
        None
