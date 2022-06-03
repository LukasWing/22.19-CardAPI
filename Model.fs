module _22._19_CardAPI.Model
open ROP
open System
open FSharp.Data.Sql
let hwm = hw

type MQ = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL, DbCredentials.connStr, Owner = DbCredentials.owner, ResolutionPath = DbCredentials.resolutionPath>
type Card = {
        Indeks:int; 
        Word: String; 
        Meaning: String; 
        Displayed: int;
        Remembered: int;
        Displaytime: int64;
        UserId: int;
        Id:int;
    }

let doSQL =
    Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")
    let ctx = MQ.GetDataContext()
    let db = ctx.MatquizDkDb
    let insertCard card = 
        let words = db.Words    
        let word = words.Create()
        word.Indeks <- card.Indeks
        word.Word <- card.Word
        word.Meaning <- card.Meaning
        word.Id <- card.Id
        word.Displayed <- card.Displayed
        word.Remembered <- card.Remembered
        word.UserId <- card.UserId
        word.DisplayTime <- card.Displaytime
        ctx.SubmitUpdates()
    
    let myCard = {
        Indeks=49684; 
        Word="What method to call after insertion in f#";
        Meaning="submitChanges";
        Displayed=0;
        Remembered=0;
        Displaytime=1654682400000L;
        UserId=2050;
        Id=1234;
    }
    insertCard myCard
// return an integer exit code
