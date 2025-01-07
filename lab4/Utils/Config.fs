module Utils.Config

open System
open System.IO
open Newtonsoft.Json

type AppConfig =
    { DockerEndpoint: string
      LogDirectory: string
      ReportDirectory: string
      ApiPort: int }

let loadConfig (filePath: string) : AppConfig =
    if not (File.Exists(filePath)) then
        failwithf "Configuration file '%s' not found." filePath

    let configJson = File.ReadAllText(filePath)
    JsonConvert.DeserializeObject<AppConfig>(configJson)

let getDefaultConfig () =
    { DockerEndpoint = "http://localhost:2375"
      LogDirectory = "./logs"
      ReportDirectory = "./reports"
      ApiPort = 8080 }

let saveConfig (filePath: string) (config: AppConfig) =
    let configJson = JsonConvert.SerializeObject(config, Formatting.Indented)
    File.WriteAllText(filePath, configJson)
