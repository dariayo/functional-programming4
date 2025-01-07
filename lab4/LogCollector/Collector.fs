module LogCollector.Collector

open System
open System.Net.Http
open System.IO
open Newtonsoft.Json.Linq

let dockerEndpoint = "http://localhost:2375"

let getActiveContainers () =
    task {
        use client = new HttpClient()
        let! response = client.GetStringAsync($"{dockerEndpoint}/containers/json")

        return
            JArray.Parse(response)
            |> Seq.map (fun item -> item.["Id"].ToString(), item.["Names"].[0].ToString().TrimStart('/'))
            |> Seq.toList
    }

let fetchLogs containerId =
    task {
        use client = new HttpClient()
        let url = $"{dockerEndpoint}/containers/{containerId}/logs?stdout=true&stderr=true"
        let! response = client.GetStreamAsync(url)
        use reader = new StreamReader(response)
        let! logs = reader.ReadToEndAsync()
        return logs
    }

let collectLogs () =
    task {
        let! containers = getActiveContainers ()

        for (id, name) in containers do
            let! logs = fetchLogs id
            printfn "Logs for container %s:\n%s" name logs
            ()
    }
