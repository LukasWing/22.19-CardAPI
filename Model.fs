module _22._19_CardAPI.Model
open ROP
open System
open FSharp.Data.Sql
type MQ = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL, DbCredentials.connStr, Owner = DbCredentials.owner, ResolutionPath = DbCredentials.resolutionPath>
type Card = {
        Word: String; 
        Meaning: String; 
        Displayed: int;
        Remembered: int;
        Displaytime: int64;
        UserId: int;
        Id:int;
    }
type Submission = {
    Question: string;
    Answer: string;
    nDisplays: int;
}
type Message = {
    Text: string
}

let validateQASize sub = 
    printf "%A" sub
    if sub.Question.Length > 256 || sub.Answer.Length > 256 
    then Failure("Size of question or answer to big")
    else Success(sub)

let validateNDisps sub = 
    match 0 < sub.nDisplays && sub.nDisplays < 10 with 
    | true -> Success(sub)
    | false -> Failure("Number of displays is invalid")

let validateQANotEmpty = function 
        | {Question=""; Answer=""; nDisplays=_} -> Failure("No question or answer given")
        | {Question=q; Answer=""; nDisplays=_} when q<>"" -> Failure("No answer given")
        | {Question=""; Answer=a; nDisplays=_} when a<>"" -> Failure("No question given")
        | sub -> Success(sub)

let getDates nDisplays = 
    let targetDate n = System.DateTime.Today.AddDays(float(n)).AddHours(12.0)
    List.init 10 (fun i -> int(2.0 ** i) - 1) |> List.take nDisplays |> List.map targetDate

let prepC q a (d:DateTime) = 
    let unixTimestamp = int64(d.Subtract(new DateTime(1970, 1, 1)).TotalSeconds) * 1000L
    {   Word=q;
        Meaning=a;
        Displayed=0;
        Remembered=0;
        Displaytime=unixTimestamp;
        UserId=2050;
        Id=1234
        }
let prepareCards {Question=q; Answer=a; nDisplays=n} =
    getDates n 
    |> List.map (prepC q a)
    |> Success
    
let insertCards cards =
    Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")
    let ctx = MQ.GetDataContext()
    let db = ctx.MatquizDkDb
    let insertCard card = 
        let words = db.Words    
        let word = words.Create()
        word.Word <- card.Word
        word.Meaning <- card.Meaning
        word.Id <- card.Id
        word.Displayed <- card.Displayed
        word.Remembered <- card.Remembered
        word.UserId <- card.UserId
        word.DisplayTime <- card.Displaytime
        ctx.SubmitUpdates()
        sprintf "Card %s with succesfully inserted" (card.Word)
    cards 
        |> List.map insertCard 
        |> List.fold (+) ""  

let insertSub card = 
    card 
    |> (validateQASize &&& validateNDisps &&& validateQANotEmpty) 
    >>= prepareCards 
    >>= tryCatch insertCards
