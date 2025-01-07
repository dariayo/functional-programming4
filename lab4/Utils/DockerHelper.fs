module Utils.DockerHelper

open System
open System.Net.Http
open System.Threading.Tasks
open Newtonsoft.Json.Linq

type DockerContainer = { Id: string; Name: string }

let dockerEndpoint = "http://localhost:2375"

let getActiveContainers () : Task<DockerContainer list> =
    task {
        use client = new HttpClient()

        let! response =
            client.GetStringAsync($"{dockerEndpoint}/containers/json")
            |> Async.AwaitTask

        let containers =
            JArray.Parse(response)
            |> Seq.map (fun item ->
                { Id = item.["Id"].ToString()
                  Name = item.["Names"].[0].ToString().TrimStart('/') })
            |> Seq.toList

        return containers
    }

let fetchContainerLogs containerId : Task<string> =
    task {
        use client = new HttpClient()

        let url =
            $"{dockerEndpoint}/containers/{containerId}/logs?stdout=true&stderr=true&timestamps=true"

        let! response = client.GetStreamAsync(url) |> Async.AwaitTask
        use reader = new System.IO.StreamReader(response)
        return! reader.ReadToEndAsync() |> Async.AwaitTask
    }
