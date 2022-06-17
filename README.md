# CardAPI built in F#
initilized using 'dotnet new giraffe -lang f#'
https://giraffe.wiki/#getting-started
use dotnet run and dotnet build to run

# MVC Architecture
The mvc architecture is used together with 
Railway Oriented Programming

# Dependencies
Project is dependent a MySQL database


# API Specification
Path: <code>/card</code>

Method: <code>POST</code>

URL Params: -

Body: Card JSON Data, E.g.:

```json 
    {"Question": "Capitol of France is?", "Answer":"Paris", "nDisplays": 6}
```
Success reponse: 
>Code: 200 CREATED<br>
>Body Content: -

Error Response: 
>Code: 400 REQUEST ERROR<br>
>Body Content: <code>"No answer given"</code>

Sample Call: 
```javascript
    let card = {
        Question: "Capitol of France is?", 
        Answer: "Paris",
        nDisplays: 6
    };
    let res = await fetch("/card",{
        method: 'POST'
        headers: {'Content-Type': 'application/json; charset=utf-8'},
        body: JSON.stringify(card)
    });
```
