module Utils

type MaybeBuilder() =
    let bind value func =
        match value with
            | Some(x) -> func x
            | _ -> None

    let wrap value = Some(value)

    member this.Bind(x, f) = bind x f
    member this.Delay(f) = f()
    member this.Return(x) = wrap x
    member this.ReturnFrom(x) = x
    member this.Combine(a, b) = if Option.isSome a then a
                                else b 
    member this.Zero(a) = None

let maybe = new MaybeBuilder()

