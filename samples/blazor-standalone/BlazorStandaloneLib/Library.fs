namespace BlazorStandaloneLib

open System
open System.Net.Http
open Microsoft.AspNetCore.Blazor
open Microsoft.AspNetCore.Blazor.Components
open FSharp.Control.Tasks
open Flatware

type Msg =
    | Increment of n : int
    | LoadWeather

type WeatherForecast() =
    member val Date = DateTime.MinValue with get, set
    member val TemperatureC = 0 with get, set
    member val TemperatureF = 0 with get, set
    member val Summary = "" with get, set

type Mdl = { Count : int; Forecasts : WeatherForecast list } with
    static member init = { Count = 0; Forecasts = [] }

type MyAppComponent() =
    inherit FlatwareComponent<Msg, Mdl>()

    [<Inject>]
    member val Http = Unchecked.defaultof<HttpClient> with get, set

    override this.ReduceAsync(msg : Msg, mdl : Mdl) =
        task {
            match msg with
                | Increment n -> 
                    printfn "Loading weather ..."
                    let! forecasts = this.Http.GetJsonAsync<WeatherForecast[]>("/sample-data/weather.json") |> Async.AwaitTask
                    printfn "Done loading weather."
                    return { mdl with Count = mdl.Count + n; Forecasts = Array.toList forecasts }
                | LoadWeather -> 
                    let! forecasts = this.Http.GetJsonAsync<WeatherForecast list>("/sample-data/weather.json") |> Async.AwaitTask
                    return { mdl with Forecasts = forecasts }
        }