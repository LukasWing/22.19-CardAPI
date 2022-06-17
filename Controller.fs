module _22._19_CardAPI.Controller
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open Model
let postCard :HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let! sub = ctx.BindJsonAsync<Submission>()
        let resultFunc =
          match insertSub sub with
            | ROP.Success(msg) -> setStatusCode 201 >=> json msg
            | ROP.Failure(errorMsg) -> setStatusCode 400 >=> json errorMsg
        return! resultFunc next ctx
      } 