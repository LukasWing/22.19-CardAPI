module _22._19_CardAPI.Controller
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open Model
let postCard :HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
      let! card = ctx.BindJsonAsync<Submission>()
      let resultFunc =
        match insertCard card with
            | ROP.Success(msg) -> json msg >=> setStatusCode 201 
            | ROP.Failure(errorMsg) -> json errorMsg >=> setStatusCode 400
      return! resultFunc next ctx
  }