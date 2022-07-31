namespace Expresser.Fsharp

module Tokenizer =

    type Token =
        | OpeningBracket
        | ClosingBracket
        | Parameter of string
        | ParameterPropertyAccess of ParameterName: string * PropertyName: string
        | NumberConstant of double
        | StringConstant of string
        | BooleanConstant of bool
        | Negate
        | Not
        | Add
        | Subtract
        | Multiply
        | Divide
        | Or
        | And
        | GreaterThan
        | LessThan
        | GreaterThanOrEqual
        | LessThanOrEqual
        | Equal
        | NotEqual
        | Sin
        | Cos

    let (|BinaryOperatorToken|_|) token =
        match token with
        | Add | Subtract | Multiply | Divide | Or | And | GreaterThan | LessThan | GreaterThanOrEqual | LessThanOrEqual | Equal | NotEqual -> Some token
        | _ -> None

    let (|UnaryOperatorToken|_|) token =
        match token with
        | Negate | Not -> Some token
        | _ -> None

    let (|FunctionToken|_|) token =
        match token with
        | Sin | Cos -> Some token
        | _ -> None

    let (|ConstantToken|_|) token =
        match token with
        | NumberConstant _ | StringConstant _ | BooleanConstant _ -> Some token
        | _ -> None

    let Tokenize stringExpression = 

        let GetStringConstantToken (stringExpression: char list) tokens =
            let rec GetStringConstant stringExpression value = 
                match stringExpression with
                | '/'::'/'::tail -> GetStringConstant tail (value @ ['/'])
                | '/'::'"'::tail -> GetStringConstant tail (value @ ['"'])
                | '/'::unknown::_ -> failwith $"Unexpected escape sequence \\{unknown}. Expected \\\" or \\\\"
                | '"'::_-> value
                | char::tail -> GetStringConstant tail (value @ [char])
                | [] -> failwith "String constant has no end. Symbol \" expected in the end of string constant"
            let constant = GetStringConstant stringExpression[1..] [] |> List.map string |> List.reduce (+)
            let token = constant |> StringConstant
            (stringExpression[constant.Length+2..], tokens @ [token])

        let (|Double|_|) str =
            match System.Double.TryParse (str:string) with
            | true, double -> Some double
            | _ -> None

        let (|Digit|_|) char =
            match System.Char.IsDigit char with
            | true -> Some char
            | _ -> None

        let (|Letter|_|) char =
            match System.Char.IsLetter char with
            | true -> Some char
            | _ -> None

        let (|OperatorChar|_|) char =
            match List.contains char ['<'; '>'; '='; '+'; '-'; '*'; '/'] with
            | true -> Some char
            | _ -> None

        let GetNumberConstantToken stringExpression tokens =
            let rec GetNumberConstant stringExpression value = 
                match stringExpression with
                | Digit digit::tail -> GetNumberConstant tail (value @ [digit])
                | '.'::tail -> GetNumberConstant tail (value @ ['.'])
                | _ -> value
            let constant = GetNumberConstant stringExpression [] |> List.map string |> List.reduce (+)
            let number = 
                match constant with
                | Double double -> double
                | _ -> failwith $"Can`t parse {constant} to number constant."
            let token = number |> NumberConstant
            (stringExpression[constant.Length..], tokens @ [token])

        let (|Letters|_|) string =
            match String.forall (fun x -> System.Char.IsDigit x) string with
            | true -> Some string
            | _ -> None

        let (|ParameterPropertyAccess|_|) (str: string) =
            let splittedString = str.Split [|'.'|] |> Seq.toList
            match splittedString with
            | [ Letters parameter; Letters property ] -> Some (parameter, property)
            | _ -> None

        let GetTokenFromString stringExpression tokens =
            let rec GetTokenFromStringStep stringExpression value =
                match stringExpression with
                | Letter letter :: tail -> GetTokenFromStringStep tail (value @ [letter])
                | '.'::tail -> GetTokenFromStringStep tail (value @ ['.'])
                | _ -> value
            let string = GetTokenFromStringStep stringExpression [] |> List.map string |> List.reduce (+)
            let token =
                match string with
                | "true" -> BooleanConstant true
                | "false" -> BooleanConstant false
                | "not" -> Not
                | "Or" -> Or
                | "And" -> And
                | "sin" -> Sin
                | "cos" -> Cos
                | Letters parameter -> Parameter parameter
                | ParameterPropertyAccess parameterPropertyAccess -> ParameterPropertyAccess parameterPropertyAccess
                | _ -> failwith $"Can`t find token '{string}'"
            (stringExpression[string.Length..], tokens @ [token])

        let GetOperatorToken (stringExpression:char list) (tokens: Token list) =
            let IsNegate (tokens: Token list) =
                if tokens.Length = 0
                then false
                else
                match List.last tokens with 
                | BinaryOperatorToken _ | FunctionToken _ | OpeningBracket -> true
                | _ -> false
            let token = 
                match stringExpression with
                | '-'::_ when IsNegate tokens -> Negate
                | '<'::'>'::_ -> NotEqual
                | '>'::'='::_ -> GreaterThanOrEqual
                | '<'::'='::_ -> LessThanOrEqual
                | '='::_ -> Equal
                | '>'::_ -> GreaterThan
                | '<'::_ -> LessThan
                | '+'::_ -> Add
                | '-'::_ -> Subtract
                | '*'::_ -> Multiply
                | '/'::_ -> Divide
                | _ -> failwith "Unexpected operator token"
            let newStringExpression =
                match token with
                | NotEqual | GreaterThanOrEqual | LessThanOrEqual -> stringExpression[2..]
                | _ -> stringExpression[1..]
            (newStringExpression, tokens @ [token])

        let rec TokenizeStep (stringExpression: char list) (tokens: Token list) =
            if List.isEmpty stringExpression
            then tokens
            else 
            match stringExpression[0] with
            | '"' -> GetStringConstantToken stringExpression tokens ||> TokenizeStep
            | '(' -> TokenizeStep stringExpression[1..] (tokens @ [OpeningBracket])
            | ')' -> TokenizeStep stringExpression[1..] (tokens @ [ClosingBracket])
            | ' ' -> TokenizeStep stringExpression[1..] tokens
            | Digit _ -> GetNumberConstantToken stringExpression tokens ||> TokenizeStep
            | Letter _-> GetTokenFromString stringExpression tokens ||> TokenizeStep
            | OperatorChar _ -> GetOperatorToken stringExpression tokens ||> TokenizeStep
            | unknownChar -> invalidOp $"Unknown symbol {unknownChar}"
        
        TokenizeStep stringExpression []