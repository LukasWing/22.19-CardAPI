module _22._19_CardAPI.ROP
// This source is taken from the article on Railway Oriented Programming
// by Scott W, see
// https://fsharpforfunandprofit.com/posts/recipe-part2/#the-railway-track-functions-complete-code

// Idea: A user flow is defined as a sequence of functions each doing
// a part of the work. Each part can fail and we want a nice way to
// code for the happy path (no errors) and have errors being
// collected/handled nicely as we go along.

// Represent success and error as one value:

// An example function that do a small step that may also fail.

/// Generic result type incorporating both success and failure.
type Result<'TSuccess,'TFailure> =
| Success of 'TSuccess
| Failure of 'TFailure

// Converting switches to two-track inputs.
// ** Converting switches to two-track inputs **
// Given a function like validate that only takes a non-error input
// and turn it into a function also taking an error input which may
// happen if the function (step) before fails. In this case the error
// should just be passed on.

/// The switchFunction is the function only taking the non-error input.
let bind switchFunction = 
    fun twoTrackInput -> 
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> Failure f   

let (>>=) twoTrackInput switchFunction = 
    bind switchFunction twoTrackInput

/// ** An alternative to bind -- combining switches into bigger switches **
/// We can also define bind as an operator that glues two single input
/// value functions together directly. We call this operator >=>
let (>=>) switch1 switch2 x = 
    match switch1 x with
    | Success s -> switch2 s
    | Failure f -> Failure f

/// We can convert above normal function into a switch
let switch f x = f x |> Success

/// Maps onetrack function to a two track.
let map oneTrackFunction twoTrackInput = 
    match twoTrackInput with
    | Success s -> Success (oneTrackFunction s)
    | Failure f -> Failure f


/// ** Converting dead-end functions to two-track functions **
let tee f x = 
    f x |> ignore
    x

// ** Handling exceptions **
let tryCatch f x =
    try
        f x |> Success
    with
    | ex -> Failure ex.Message

/// ** Functions with two-track input, e.g., for logging. **
let doubleMap successFunc failureFunc twoTrackInput =
    match twoTrackInput with
    | Success s -> Success (successFunc s)
    | Failure f -> Failure (failureFunc f)

/// Example including a two-track logging function.
let log twoTrackInput = 
    let success x = printfn "DEBUG. Success so far: %A" x; x
    let failure x = printfn "ERROR. %A" x; x
    doubleMap success failure twoTrackInput 
  


/// ** Converting a single value to a two-track value **
let succeed x = Success x

/// ** Converting a single value to a two-track value **
let fail x = Failure x

/// ** Combining functions in parallel **
let plus addSuccess addFailure switch1 switch2 x = 
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> Success (addSuccess s1 s2)
    | Failure f1,Success _  -> Failure f1
    | Success _ ,Failure f2 -> Failure f2
    | Failure f1,Failure f2 -> Failure (addFailure f1 f2)

/// create a "plus" function for validation functions
let (&&&) v1 v2 = 
    let addSuccess r1 r2 = r1 // return first
    let addFailure s1 s2 = s1 + "; " + s2  // concat
    plus addSuccess addFailure v1 v2 

/// ** Dynamic injection of functions **
type Config = {debug:bool}

/// log succes so far
/// E.g. DEBUG. Success so far: {name = "Alice"; email = "good";}
let debugLogger twoTrackInput = 
    let success x = printfn "DEBUG. Success so far: %A" x; x
    let failure = id // don't log here
    doubleMap success failure twoTrackInput 

/// logs only if debug in config is true
let injectableLogger config = 
    if config.debug then debugLogger else id

let unpackRes res = 
    match res with 
    | Success s -> s
    | Failure f -> f