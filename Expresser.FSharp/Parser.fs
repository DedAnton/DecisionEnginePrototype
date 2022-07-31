namespace Expresser.Fsharp

open Tokenizer

module Parser =

    type Expression =
        | NumberConstant of double
        | StringConstant of string
        | BooleanConstant of bool
        | Add of Expression * Expression
        | Subtract of Expression * Expression
        | Divide of Expression * Expression
        | Multiply of Expression * Expression
        | And of Expression * Expression
        | Or of Expression * Expression
        | Equal of Expression * Expression
        | NotEqual of Expression * Expression
        | GreaterThan of Expression * Expression
        | GreaterThanOrEqual of Expression * Expression
        | LessThan of Expression * Expression
        | LessThanOrEqual of Expression * Expression
        | Not of Expression
        | Negate of Expression
        | Sin of Expression
        | Cos of Expression
    
    let Parse (stringExpression: string) : Expression =
        
        let rec ParseNextToken (tokens: Token list) =

            let rec GetNextOperation (tokens: Token list) (depth: int) (position: int) (nextTokenPosition: int) (nextToken: Token option) = 

                let OperationPriority token =
                    match token with
                    | Token.Or -> 8
                    | Token.And -> 7
                    | Token.Equal -> 6
                    | Token.NotEqual -> 6
                    | Token.GreaterThan -> 5
                    | Token.GreaterThanOrEqual -> 5
                    | Token.LessThan -> 5
                    | Token.LessThanOrEqual -> 5
                    | Token.Add -> 4
                    | Token.Subtract -> 4
                    | Token.Multiply -> 3
                    | Token.Divide -> 3
                    | Token.Not -> 2
                    | Token.Negate -> 2
                    | FunctionToken _ -> 1
                    | _ -> failwith $"Token {token} is not a operator or a function and has no priority"

                match tokens with
                | OpeningBracket :: other -> GetNextOperation other (depth+1) (position+1) nextTokenPosition nextToken
                | ClosingBracket :: other -> GetNextOperation other (depth-1) (position+1) nextTokenPosition nextToken
                | UnaryOperatorToken token :: other | BinaryOperatorToken token :: other | FunctionToken token :: other when depth = 0 -> 
                    match nextToken, token with
                    | None, applicant -> applicant |> Some |> GetNextOperation other 0 (position+1) position
                    | Some current, applicant when OperationPriority applicant >= OperationPriority current -> 
                        applicant |> Some |> GetNextOperation other 0 (position+1) position
                    | Some current, _ -> current |> Some |> GetNextOperation other 0 (position+1) nextTokenPosition
                | [] -> (nextToken, nextTokenPosition)
                | _ :: other -> GetNextOperation other depth (position+1) nextTokenPosition nextToken

            let ParseOperation token tokenPosition =
                match token with
                | Token.Equal -> Equal (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.NotEqual -> NotEqual (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.GreaterThan -> GreaterThan (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.LessThan -> LessThan (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.GreaterThanOrEqual -> GreaterThanOrEqual (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.LessThanOrEqual -> LessThanOrEqual (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.And -> And (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Or -> Or (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Add -> Add (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Subtract -> Subtract (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Divide -> Divide (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Multiply -> Multiply (ParseNextToken tokens[0..tokenPosition-1], ParseNextToken tokens[tokenPosition+1..])
                | Token.Not -> Not(ParseNextToken tokens[0..tokens.Length - 1])
                | Token.Negate -> Negate(ParseNextToken tokens[1..tokens.Length - 1])
                | Token.Sin -> Sin(ParseNextToken tokens[1..tokens.Length - 1])
                | Token.Cos -> Cos(ParseNextToken tokens[1..tokens.Length - 1])
                | token -> failwith $"Can`t parse operation token {token} to expression"

            let ParseConstant token =
                match token with
                | Token.NumberConstant number -> NumberConstant number
                | Token.StringConstant string -> StringConstant string
                | Token.BooleanConstant bool -> BooleanConstant bool
                | token -> failwith $"Can`t parse constant token {token} to expression"

            match GetNextOperation tokens 0 0 0 None with
            | Some token, position -> ParseOperation token position
            | None, _ when tokens.Length = 1 -> ParseConstant tokens[0]
            | None, _ when tokens.Length > 2 -> 
                match tokens[0], tokens[tokens.Length-1] with
                | OpeningBracket, ClosingBracket -> ParseNextToken tokens[1..tokens.Length - 2]
                | _ -> failwith $"Can`t parse expression"
            | _ -> failwith $"Can`t parse expression"

        let tokens = stringExpression.ToCharArray() |> Seq.toList |> Tokenizer.Tokenize
        ParseNextToken tokens