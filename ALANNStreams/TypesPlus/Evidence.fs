﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module Evidence

open System.Linq

let rec interleave xs ys acc =
    match xs, ys with
    |    [],    [] -> acc
    | x::xs,    [] -> interleave xs [] (x::acc)
    |    [], y::ys -> interleave [] ys (y::acc)
    | x::xs, y::ys -> interleave xs ys (y::x::acc)

let merge e1 e2 =
    interleave e1 e2 [] 
    |> List.rev
    |> List.truncate Params.TRAIL_LENGTH // Not necessary to check for distinct as the sets are non-overlapping

let nonOverlap (a: 'a seq) (b: 'a seq) =
  a .Intersect(b).Count() = 0